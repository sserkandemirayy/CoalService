using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Commands;

public record ChangeUserTypeCommand(
    Guid UserId,
    Guid UserTypeId,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;

public class ChangeUserTypeHandler : IRequestHandler<ChangeUserTypeCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly IUserTypeRepository _userTypes;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _current;

    public ChangeUserTypeHandler(
        IUserRepository users,
        IUserTypeRepository userTypes,
        IAuditLogRepository audit,
        IUnitOfWork uow,
        ICurrentUserService current)
    {
        _users = users;
        _userTypes = userTypes;
        _audit = audit;
        _uow = uow;
        _current = current;
    }

    public async Task<Result<Unit>> Handle(ChangeUserTypeCommand req, CancellationToken ct)
    {
        // === Kullanıcı doğrulama ===
        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user == null)
            return Result<Unit>.Failure("User not found");

        // === Yeni UserType doğrulama ===
        var newType = await _userTypes.GetByIdAsync(req.UserTypeId, ct);
        if (newType == null)
            return Result<Unit>.Failure("UserType not found");

        if (user.UserTypeId == req.UserTypeId)
            return Result<Unit>.Success(Unit.Value); // zaten aynı tip, işlem yapma

        var oldTypeId = user.UserTypeId;

        // === Güncelleme ===
        user.SetUserType(req.UserTypeId);

        // type değişince mevcut uzmanlığı da sıfırla:
        user.SetSpecialization(null);

        await _users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        // === Audit Log ===
        await _audit.AddAsync(AuditLog.Create(
            req.PerformedByUserId,
            "user_type_change",
            "User",
            user.Id,
            _current.GetIpAddress(),
            new
            {
                OldUserTypeId = oldTypeId,
                NewUserTypeId = req.UserTypeId,
                ChangedBy = req.PerformedByUserId
            }.ToString()
        ), ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}