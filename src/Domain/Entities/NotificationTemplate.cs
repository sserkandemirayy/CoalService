using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class NotificationTemplate : BaseEntity
{
    protected NotificationTemplate() { }

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    public string TitleTemplate { get; private set; } = default!;
    public string MessageTemplate { get; private set; } = default!;
    public string? ActionUrlTemplate { get; private set; }

    public NotificationType Type { get; private set; }
    public NotificationSeverity Severity { get; private set; }

    public bool IsActive { get; private set; } = true;

    public static NotificationTemplate Create(
        string code,
        string name,
        string titleTemplate,
        string messageTemplate,
        NotificationType type,
        NotificationSeverity severity,
        string? actionUrlTemplate = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(titleTemplate))
            throw new ArgumentException("TitleTemplate is required.", nameof(titleTemplate));

        if (string.IsNullOrWhiteSpace(messageTemplate))
            throw new ArgumentException("MessageTemplate is required.", nameof(messageTemplate));

        return new NotificationTemplate
        {
            Code = code.Trim(),
            Name = name.Trim(),
            TitleTemplate = titleTemplate.Trim(),
            MessageTemplate = messageTemplate.Trim(),
            Type = type,
            Severity = severity,
            ActionUrlTemplate = string.IsNullOrWhiteSpace(actionUrlTemplate) ? null : actionUrlTemplate.Trim(),
            IsActive = true
        };
    }

    public void Update(
        string name,
        string titleTemplate,
        string messageTemplate,
        NotificationType type,
        NotificationSeverity severity,
        string? actionUrlTemplate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(titleTemplate))
            throw new ArgumentException("TitleTemplate is required.", nameof(titleTemplate));

        if (string.IsNullOrWhiteSpace(messageTemplate))
            throw new ArgumentException("MessageTemplate is required.", nameof(messageTemplate));

        Name = name.Trim();
        TitleTemplate = titleTemplate.Trim();
        MessageTemplate = messageTemplate.Trim();
        Type = type;
        Severity = severity;
        ActionUrlTemplate = string.IsNullOrWhiteSpace(actionUrlTemplate) ? null : actionUrlTemplate.Trim();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}