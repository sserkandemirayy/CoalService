using Application.DTOs.AlarmManagement;
using Domain.Entities;

namespace Application.AlarmManagement.Mappings;

public static class AlarmMappingExtensions
{
    public static AlarmDto ToDto(this Alarm alarm)
    {
        ArgumentNullException.ThrowIfNull(alarm);

        return new AlarmDto(
            alarm.Id,
            alarm.RawEventId,
            alarm.AlarmType.ToString(),
            alarm.Severity.ToString(),
            alarm.Status.ToString(),
            alarm.TagId,
            alarm.PeerTagId,
            alarm.AnchorId,
            alarm.UserId,
            alarm.StartedAt,
            alarm.EndedAt,
            alarm.AcknowledgedAt,
            alarm.AcknowledgedBy,
            alarm.Title,
            alarm.Description,
            alarm.DataJson);
    }

    public static IReadOnlyList<AlarmDto> ToDtoList(
        this IEnumerable<Alarm> alarms)
    {
        ArgumentNullException.ThrowIfNull(alarms);

        return alarms
            .Select(x => x.ToDto())
            .ToList();
    }

    public static AlarmNoteDto ToDto(this AlarmNote alarmNote)
    {
        ArgumentNullException.ThrowIfNull(alarmNote);

        var userName = alarmNote.User is null
            ? string.Empty
            : $"{alarmNote.User.FirstName} {alarmNote.User.LastName}".Trim();

        return new AlarmNoteDto(
            alarmNote.Id,
            alarmNote.AlarmId,
            alarmNote.UserId,
            userName,
            alarmNote.Note,
            alarmNote.CreatedAt);
    }

    public static IReadOnlyList<AlarmNoteDto> ToDtoList(
        this IEnumerable<AlarmNote> alarmNotes)
    {
        ArgumentNullException.ThrowIfNull(alarmNotes);

        return alarmNotes
            .Select(x => x.ToDto())
            .ToList();
    }
}