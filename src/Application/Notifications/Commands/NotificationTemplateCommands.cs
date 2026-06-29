using Application.Common.Models;
using Application.DTOs.Notifications;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Notifications.Commands;

public sealed record CreateNotificationTemplateCommand(CreateNotificationTemplateDto Dto)
    : IRequest<Result<Guid>>;

public sealed class CreateNotificationTemplateCommandHandler
    : IRequestHandler<CreateNotificationTemplateCommand, Result<Guid>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationTemplateCommandHandler(
        INotificationTemplateRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateNotificationTemplateCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<NotificationType>(request.Dto.Type, true, out var type))
            return Result<Guid>.Failure("Invalid notification type.");

        if (!Enum.TryParse<NotificationSeverity>(request.Dto.Severity, true, out var severity))
            return Result<Guid>.Failure("Invalid notification severity.");

        var existing = await _repository.GetByCodeAsync(request.Dto.Code, ct);
        if (existing is not null)
            return Result<Guid>.Failure("Template code already exists.");

        var template = NotificationTemplate.Create(
            request.Dto.Code,
            request.Dto.Name,
            request.Dto.TitleTemplate,
            request.Dto.MessageTemplate,
            type,
            severity,
            request.Dto.ActionUrlTemplate);

        await _repository.AddAsync(template, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(template.Id);
    }
}

public sealed record UpdateNotificationTemplateCommand(Guid Id, UpdateNotificationTemplateDto Dto)
    : IRequest<Result<Unit>>;

public sealed class UpdateNotificationTemplateCommandHandler
    : IRequestHandler<UpdateNotificationTemplateCommand, Result<Unit>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNotificationTemplateCommandHandler(
        INotificationTemplateRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(UpdateNotificationTemplateCommand request, CancellationToken ct)
    {
        var template = await _repository.GetByIdAsync(request.Id, ct);
        if (template is null)
            return Result<Unit>.Failure("Template not found.");

        if (!Enum.TryParse<NotificationType>(request.Dto.Type, true, out var type))
            return Result<Unit>.Failure("Invalid notification type.");

        if (!Enum.TryParse<NotificationSeverity>(request.Dto.Severity, true, out var severity))
            return Result<Unit>.Failure("Invalid notification severity.");

        template.Update(
            request.Dto.Name,
            request.Dto.TitleTemplate,
            request.Dto.MessageTemplate,
            type,
            severity,
            request.Dto.ActionUrlTemplate);

        await _repository.UpdateAsync(template, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

public sealed record DeleteNotificationTemplateCommand(Guid Id, Guid PerformedByUserId)
    : IRequest<Result<Unit>>;

public sealed class DeleteNotificationTemplateCommandHandler
    : IRequestHandler<DeleteNotificationTemplateCommand, Result<Unit>>
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNotificationTemplateCommandHandler(
        INotificationTemplateRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(DeleteNotificationTemplateCommand request, CancellationToken ct)
    {
        var template = await _repository.GetByIdAsync(request.Id, ct);
        if (template is null)
            return Result<Unit>.Failure("Template not found.");

        template.SoftDelete(request.PerformedByUserId);
        await _repository.UpdateAsync(template, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}