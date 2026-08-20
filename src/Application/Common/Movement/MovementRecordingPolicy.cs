using Domain.Entities;

namespace Application.Common.Movement;

public sealed class MovementRecordingPolicy
    : IMovementRecordingPolicy
{
    private const decimal DistanceThreshold = 0.50m;

    private static readonly TimeSpan SamplingInterval =
        TimeSpan.FromSeconds(5);

    public MovementRecordDecision Evaluate(
        MovementEvent? previousMovement,
        Guid? floorMapId,
        Guid? floorMapZoneId,
        decimal x,
        decimal y,
        decimal z,
        DateTime eventTimestamp)
    {
        eventTimestamp =
            EnsureUtc(eventTimestamp);

        // İlk movement noktası
        if (previousMovement is null)
        {
            return new MovementRecordDecision(
                true,
                "FirstPoint");
        }

        var previousTimestamp =
            EnsureUtc(
                previousMovement.EventTimestamp);

        // Daha eski event geldiyse movement history'ye
        // yeni snapshot olarak yazmıyoruz.
        if (eventTimestamp < previousTimestamp)
        {
            return new MovementRecordDecision(
                false,
                null);
        }

        // Harita değişmiş
        if (previousMovement.FloorMapId != floorMapId)
        {
            return new MovementRecordDecision(
                true,
                "FloorMapChanged");
        }

        // Zone değişmiş
        if (previousMovement.FloorMapZoneId != floorMapZoneId)
        {
            return new MovementRecordDecision(
                true,
                "ZoneChanged");
        }

        // Son kaydedilmiş movement noktasından
        // en az 50 cm uzaklaşmış
        if (HasMovedEnough(
                previousMovement.X,
                previousMovement.Y,
                previousMovement.Z,
                x,
                y,
                z))
        {
            return new MovementRecordDecision(
                true,
                "Distance");
        }

        // Hareket threshold'un altında olsa bile
        // son movement kaydından 5 saniye geçmiş.
        if (eventTimestamp - previousTimestamp >=
            SamplingInterval)
        {
            return new MovementRecordDecision(
                true,
                "Interval");
        }

        return new MovementRecordDecision(
            false,
            null);
    }

    private static bool HasMovedEnough(
        decimal previousX,
        decimal previousY,
        decimal previousZ,
        decimal currentX,
        decimal currentY,
        decimal currentZ)
    {
        var dx =
            currentX - previousX;

        var dy =
            currentY - previousY;

        var dz =
            currentZ - previousZ;

        var distanceSquared =
            (dx * dx) +
            (dy * dy) +
            (dz * dz);

        var thresholdSquared =
            DistanceThreshold *
            DistanceThreshold;

        return distanceSquared >=
               thresholdSquared;
    }

    private static DateTime EnsureUtc(
        DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
            return value;

        if (value.Kind == DateTimeKind.Local)
            return value.ToUniversalTime();

        return DateTime.SpecifyKind(
            value,
            DateTimeKind.Utc);
    }
}