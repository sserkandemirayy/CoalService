using Domain.Entities;

namespace Application.Common.Movement;

public sealed record MovementRecordDecision(
    bool ShouldRecord,
    string? Reason);

public interface IMovementRecordingPolicy
{
    MovementRecordDecision Evaluate(
        MovementEvent? previousMovement,
        Guid? floorMapId,
        Guid? floorMapZoneId,
        decimal x,
        decimal y,
        decimal z,
        DateTime eventTimestamp);
}