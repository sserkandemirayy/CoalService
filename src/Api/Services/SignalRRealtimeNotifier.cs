using Api.Hubs;
using Application.Common.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services;

public sealed class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<TrackingHub> _hub;

    public SignalRRealtimeNotifier(IHubContext<TrackingHub> hub)
    {
        _hub = hub;
    }

    public async Task LocationUpdatedAsync(LocationUpdatedRealtimeDto payload, CancellationToken ct = default)
    {
        await _hub.Clients.Group("dashboard")
            .SendAsync("LocationUpdated", payload, ct);

        await _hub.Clients.Group($"tag:{payload.TagId}")
            .SendAsync("LocationUpdated", payload, ct);
    }

    public async Task BatteryUpdatedAsync(BatteryUpdatedRealtimeDto payload, CancellationToken ct = default)
    {
        await _hub.Clients.Group("dashboard")
            .SendAsync("BatteryUpdated", payload, ct);

        await _hub.Clients.Group($"tag:{payload.TagId}")
            .SendAsync("BatteryUpdated", payload, ct);
    }

    public async Task AlarmRaisedAsync(AlarmRaisedRealtimeDto payload, CancellationToken ct = default)
    {
        await _hub.Clients.Group("dashboard")
            .SendAsync("AlarmRaised", payload, ct);

        if (payload.TagId.HasValue)
            await _hub.Clients.Group($"tag:{payload.TagId.Value}")
                .SendAsync("AlarmRaised", payload, ct);

        if (payload.PeerTagId.HasValue)
            await _hub.Clients.Group($"tag:{payload.PeerTagId.Value}")
                .SendAsync("AlarmRaised", payload, ct);

        if (payload.AnchorId.HasValue)
            await _hub.Clients.Group($"anchor:{payload.AnchorId.Value}")
                .SendAsync("AlarmRaised", payload, ct);
    }

    public async Task AnchorStatusChangedAsync(AnchorStatusChangedRealtimeDto payload, CancellationToken ct = default)
    {
        await _hub.Clients.Group("dashboard")
            .SendAsync("AnchorStatusChanged", payload, ct);

        await _hub.Clients.Group($"anchor:{payload.AnchorId}")
            .SendAsync("AnchorStatusChanged", payload, ct);
    }

    public async Task TagStatusChangedAsync(TagStatusChangedRealtimeDto payload, CancellationToken ct = default)
    {
        await _hub.Clients.Group("dashboard")
            .SendAsync("TagStatusChanged", payload, ct);

        await _hub.Clients.Group($"tag:{payload.TagId}")
            .SendAsync("TagStatusChanged", payload, ct);
    }
}