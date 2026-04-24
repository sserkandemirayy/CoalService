using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class CommandStatusHistory : BaseEntity
{
    protected CommandStatusHistory() { }

    public Guid CommandRequestId { get; private set; }
    public CommandRequest CommandRequest { get; private set; } = default!;

    public RtlsCommandStatus? OldStatus { get; private set; }
    public RtlsCommandStatus NewStatus { get; private set; }

    public Guid? ChangedByUserId { get; private set; }
    public User? ChangedByUser { get; private set; }

    public DateTime ChangedAt { get; private set; }
    public string? Note { get; private set; }
    public string? DataJson { get; private set; }

    public static CommandStatusHistory Create(
        Guid commandRequestId,
        RtlsCommandStatus? oldStatus,
        RtlsCommandStatus newStatus,
        Guid? changedByUserId = null,
        string? note = null,
        string? dataJson = null,
        DateTime? changedAt = null)
    {
        if (commandRequestId == Guid.Empty)
            throw new ArgumentException("CommandRequestId is required.", nameof(commandRequestId));

        return new CommandStatusHistory
        {
            CommandRequestId = commandRequestId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedByUserId = changedByUserId,
            ChangedAt = changedAt ?? DateTime.UtcNow,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            DataJson = string.IsNullOrWhiteSpace(dataJson) ? null : dataJson
        };
    }
}