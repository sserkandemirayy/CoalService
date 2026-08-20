using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class TimescaleDbInitializer
{
    public static async Task EnsureMovementEventsHypertableAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken ct = default)
    {
        var connection =
            db.Database.GetDbConnection();

        var shouldClose =
            connection.State != ConnectionState.Open;

        try
        {
            if (shouldClose)
                await connection.OpenAsync(ct);

            // ========================================================
            // TIMESCALE EXTENSION SERVER'DA MEVCUT MU?
            // ========================================================

            await using var availabilityCommand =
                connection.CreateCommand();

            availabilityCommand.CommandText =
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM pg_available_extensions
                    WHERE name = 'timescaledb'
                );
                """;

            var availabilityResult =
                await availabilityCommand
                    .ExecuteScalarAsync(ct);

            var isAvailable =
                availabilityResult is bool available &&
                available;

            if (!isAvailable)
            {
                logger.LogWarning(
                    "TimescaleDB extension is not available on this PostgreSQL server. " +
                    "MovementEvents will continue as a regular PostgreSQL table.");

                return;
            }

            // ========================================================
            // CREATE EXTENSION
            // ========================================================

            try
            {
                await using var extensionCommand =
                    connection.CreateCommand();

                extensionCommand.CommandText =
                    """
                    CREATE EXTENSION IF NOT EXISTS timescaledb;
                    """;

                await extensionCommand
                    .ExecuteNonQueryAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "TimescaleDB extension is available but could not be enabled. " +
                    "MovementEvents will continue as a regular PostgreSQL table.");

                return;
            }

            // ========================================================
            // MOVEMENT EVENTS TABLE VAR MI?
            // ========================================================

            await using var tableCommand =
                connection.CreateCommand();

            tableCommand.CommandText =
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = 'MovementEvents'
                );
                """;

            var tableResult =
                await tableCommand
                    .ExecuteScalarAsync(ct);

            var tableExists =
                tableResult is bool exists &&
                exists;

            if (!tableExists)
            {
                logger.LogWarning(
                    "MovementEvents table does not exist yet. " +
                    "TimescaleDB hypertable initialization was skipped.");

                return;
            }

            // ========================================================
            // CREATE HYPERTABLE
            // ========================================================

            try
            {
                await using var hypertableCommand =
                    connection.CreateCommand();

                hypertableCommand.CommandText =
                    """
                    SELECT create_hypertable(
                        '"MovementEvents"',
                        'EventTimestamp',
                        if_not_exists => TRUE,
                        migrate_data => TRUE
                    );
                    """;

                await hypertableCommand
                    .ExecuteNonQueryAsync(ct);

                logger.LogInformation(
                    "MovementEvents TimescaleDB hypertable initialization completed.");
            }
            catch (Exception ex)
            {
                // Timescale problemi uygulamayı öldürmesin.
                // Normal PostgreSQL table olarak çalışmaya devam eder.
                logger.LogWarning(
                    ex,
                    "MovementEvents could not be converted to a TimescaleDB hypertable. " +
                    "The application will continue using the regular PostgreSQL table.");
            }
        }
        finally
        {
            if (shouldClose &&
                connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }
}