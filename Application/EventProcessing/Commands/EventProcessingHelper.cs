using Domain.Enums;
using System.Text.Json;

namespace Application.EventProcessing.Commands;

internal static class EventProcessingHelper
{
    public static DateTime FromUnixMilliseconds(long value)
        => DateTimeOffset.FromUnixTimeMilliseconds(value).UtcDateTime;

    public static string Serialize(object value)
        => JsonSerializer.Serialize(value);

    public static ImuEventType ParseImuEventType(string value)
        => Enum.Parse<ImuEventType>(value, true);

    public static ProximitySeverity ParseProximitySeverity(string value)
        => Enum.Parse<ProximitySeverity>(value, true);

    public static AnchorStatus ParseAnchorStatus(string value)
        => Enum.Parse<AnchorStatus>(value, true);
}