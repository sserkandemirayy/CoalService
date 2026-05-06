using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

// === Commands ===

public record CreateUserSpecializationCommand(
    string Code,
    string Name,
    string? Description,
    Guid UserTypeId
) : IRequest<Result<Guid>>;

public record UpdateUserSpecializationCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest<Result<Unit>>;

public record DeleteUserSpecializationCommand(
    Guid Id,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;


// === Validators ===

public class CreateUserSpecializationValidator : AbstractValidator<CreateUserSpecializationCommand>
{
    public CreateUserSpecializationValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.UserTypeId).NotEmpty();
    }
}

public class UpdateUserSpecializationValidator : AbstractValidator<UpdateUserSpecializationCommand>
{
    public UpdateUserSpecializationValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}


// === Handlers ===

public class CreateUserSpecializationHandler
    : IRequestHandler<CreateUserSpecializationCommand, Result<Guid>>
{
    private readonly IUserSpecializationRepository _repo;
    private readonly IUserTypeRepository _types;
    private readonly IUnitOfWork _uow;

    public CreateUserSpecializationHandler(
        IUserSpecializationRepository repo,
        IUserTypeRepository types,
        IUnitOfWork uow)
    {
        _repo = repo;
        _types = types;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateUserSpecializationCommand req, CancellationToken ct)
    {
        var type = await _types.GetByIdAsync(req.UserTypeId, ct);
        if (type == null)
            return Result<Guid>.Failure("UserType not found");

        var entity = UserSpecialization.Create(req.UserTypeId,  req.Code, req.Name,  req.Description, false);

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(entity.Id);
    }
}

public class UpdateUserSpecializationHandler
    : IRequestHandler<UpdateUserSpecializationCommand, Result<Unit>>
{
    private readonly IUserSpecializationRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateUserSpecializationHandler(
        IUserSpecializationRepository repo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(UpdateUserSpecializationCommand req, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(req.Id, ct);
        if (entity == null)
            return Result<Unit>.Failure("Specialization not found");

        entity.Update(req.Name, req.Description);

        await _repo.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

public class DeleteUserSpecializationHandler
    : IRequestHandler<DeleteUserSpecializationCommand, Result<Unit>>
{
    private readonly IUserSpecializationRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteUserSpecializationHandler(
        IUserSpecializationRepository repo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }



    public async Task<Result<Unit>> Handle(DeleteUserSpecializationCommand req, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(req.Id, ct);
        if (entity == null)
            return Result<Unit>.Failure("Not found");

        if (entity.IsSystem)
            return Result<Unit>.Failure("System specialization cannot be deleted."); 

        if (await _repo.HasUsersAsync(req.Id, ct))
            return Result<Unit>.Failure("Cannot delete: Assigned users exist.");

        await _repo.RemoveAsync(req.Id, req.PerformedByUserId, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}