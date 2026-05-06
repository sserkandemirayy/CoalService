using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record CreateRequestConfigCommand(
    RequestConfigCommandPayloadDto Payload,
    Guid RequestedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateRequestConfigCommandHandler : IRequestHandler<CreateRequestConfigCommand, Result<Guid>>
{
    private readonly ITagRepository _tags;
    private readonly IAnchorRepository _anchors;
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public CreateRequestConfigCommandHandler(
        ITagRepository tags,
        IAnchorRepository anchors,
        ICommandRequestRepository commands,
        ICommandStatusHistoryRepository history,
        IUnitOfWork uow)
    {
        _tags = tags;
        _anchors = anchors;
        _commands = commands;
        _history = history;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateRequestConfigCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Payload.AnchorId) && string.IsNullOrWhiteSpace(request.Payload.TagId))
            return Result<Guid>.Failure("anchorId or tagId is required.");

        Guid? anchorDbId = null;
        Guid? tagDbId = null;
        RtlsCommandTargetType targetType;

        if (!string.IsNullOrWhiteSpace(request.Payload.AnchorId))
        {
            var anchorResult = await CommandManagementHelper.GetAnchorByExternalIdAsync(_anchors, request.Payload.AnchorId!, ct);
            if (!anchorResult.IsSuccess || anchorResult.Value is null)
                return Result<Guid>.Failure(anchorResult.Error!);

            anchorDbId = anchorResult.Value.Id;
            targetType = RtlsCommandTargetType.Anchor;
        }
        else
        {
            var tagResult = await CommandManagementHelper.GetTagByExternalIdAsync(_tags, request.Payload.TagId!, ct);
            if (!tagResult.IsSuccess || tagResult.Value is null)
                return Result<Guid>.Failure(tagResult.Error!);

            tagDbId = tagResult.Value.Id;
            targetType = RtlsCommandTargetType.Tag;
        }

        var command = CommandRequest.Create(
            RtlsCommandType.RequestConfig,
            targetType,
            request.RequestedByUserId,
            CommandManagementHelper.Serialize(request.Payload),
            tagDbId,
            anchorDbId);

        await _commands.AddAsync(command, ct);
        await CommandManagementHelper.AddInitialHistoryAsync(_history, command, request.RequestedByUserId, "Command request created.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}