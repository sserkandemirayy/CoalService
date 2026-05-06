using Domain.Abstractions;

namespace Domain.Entities;

public enum SettingScope { System = 1, User = 2 }

public class Setting : BaseEntity
{
    public string Key { get; private set; } = default!;
    public string Value { get; private set; } = default!;
    public SettingScope Scope { get; private set; }

    public Guid? UserId { get; private set; }  // sadece User scope için
    public User? User { get; private set; }

    protected Setting() { }

    public static Setting Create(string key, string value, SettingScope scope, Guid? userId = null)
        => new()
        {
            Key = key,
            Value = value,
            Scope = scope,
            UserId = userId
        };

    public void UpdateValue(string newValue)
        => Value = newValue;
}
