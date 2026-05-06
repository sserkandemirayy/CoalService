using System.Text.Json;
using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.CommandManagement.Commands;

internal static class CommandManagementHelper
{
    public static string Serialize(object value) => JsonSerializer.Serialize(value);

    public static DioPinDirection ParseDioDirection(string value)
        => Enum.Parse<DioPinDirection>(value, true);

    public static TagAlertLedColor ParseLedColor(string value)
        => Enum.Parse<TagAlertLedColor>(value, true);

    public static async Task<Result<Tag>> GetTagByExternalIdAsync(
        ITagRepository tags,
        string externalId,
        CancellationToken ct)
    {
        var tag = await tags.GetByExternalIdAsync(externalId, ct);
        return tag is null
            ? Result<Tag>.Failure("Tag not found.")
            : Result<Tag>.Success(tag);
    }

    public static async Task<Result<Anchor>> GetAnchorByExternalIdAsync(
        IAnchorRepository anchors,
        string externalId,
        CancellationToken ct)
    {
        var anchor = await anchors.GetByExternalIdAsync(externalId, ct);
        return anchor is null
            ? Result<Anchor>.Failure("Anchor not found.")
            : Result<Anchor>.Success(anchor);
    }

    public static async Task AddInitialHistoryAsync(
        ICommandStatusHistoryRepository historyRepository,
        CommandRequest commandRequest,
        Guid changedByUserId,
        string? note,
        CancellationToken ct)
    {
        var history = CommandStatusHistory.Create(
            commandRequest.Id,
            null,
            commandRequest.Status,
            changedByUserId,
            note,
            commandRequest.PayloadJson);

        await historyRepository.AddAsync(history, ct);
    }

    public static string BuildOutboxPayload(CommandRequest command)
    {
        var payload = new CommandOutboxEnvelopeDto(
            command.Id,
            command.CommandType.ToString(),
            command.TargetType.ToString(),
            command.TagId,
            command.AnchorId,
            command.PayloadJson,
            command.RequestedAt);

        return Serialize(payload);
    }
}