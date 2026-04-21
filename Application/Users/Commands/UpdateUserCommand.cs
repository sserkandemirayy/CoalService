using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace Application.Users.Commands;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string? Address,
    string? NationalId,
    string? Gender,
    DateTime? BirthDate,
    Guid UserTypeId,
    Guid? UserSpecializationId,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Address).MaximumLength(500);
    }
}

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;
    private readonly ICurrentUserService _current;
    private readonly IUserTypeRepository _userTypes;
    private readonly IUserSpecializationRepository _specializations;

    public UpdateUserHandler(
        IUserRepository users,
        IUnitOfWork uow,
        IAuditLogRepository audit,
        ICurrentUserService current,
        IUserTypeRepository userTypes,
        IUserSpecializationRepository specializations) 
    {
        _users = users;
        _uow = uow;
        _audit = audit;
        _current = current;
        _userTypes = userTypes;
        _specializations = specializations;
    }

    public async Task<Result<Unit>> Handle(UpdateUserCommand req, CancellationToken ct)
    {
        var user = await _users.GetByIdForUpdateAsync(req.Id, ct);
        if (user is null)
            return Result<Unit>.Failure("User not found");

        var currentUserId = _current.GetCurrentUserId();
        var currentPerms = _current.GetPermissions().ToHashSet();

        // Eski değerleri al (audit için)
        var oldData = new
        {
            user.FirstName,
            user.LastName,
            user.PhoneEncrypted,
            user.AddressEncrypted,
            user.UserTypeId,
            user.NationalIdEncrypted,
            user.Gender,
            user.BirthDate
        };

        // === Güncelleme işlemi ===
        var phone = user.PhoneEncrypted;
        var address = user.AddressEncrypted;
        var nationalId = user.NationalIdEncrypted;

        var canEditPII = currentPerms.Contains("edit_pii");
        if (canEditPII)
        {
            if (!string.IsNullOrWhiteSpace(req.Phone))
                phone = req.Phone;

            if (!string.IsNullOrWhiteSpace(req.Address))
                address = req.Address;

            if (!string.IsNullOrWhiteSpace(req.NationalId))
                nationalId = req.NationalId;
        }

        user.UpdateProfile(req.FirstName, req.LastName, phone ?? string.Empty, address ?? string.Empty, nationalId ?? string.Empty, req.Gender ?? string.Empty);

        // === (UserType değişikliği) ===
        if (req.UserTypeId != user.UserTypeId)
        {
            var type = await _userTypes.GetByIdAsync(req.UserTypeId, ct);
            if (type == null)
                return Result<Unit>.Failure("Invalid UserTypeId");

            user.SetUserType(req.UserTypeId);
        }
        if (req.BirthDate.HasValue && (req.BirthDate != oldData.BirthDate))
        {
            var birthProp = typeof(User).GetProperty("BirthDate",
               BindingFlags.Instance |
               BindingFlags.NonPublic |
               BindingFlags.Public);

            birthProp?.SetValue(user, req.BirthDate.Value);
            //user.SetBirthDate(req.BirthDate.Value);
        }

        if (req.UserSpecializationId.HasValue)
        {
            var spec = await _specializations.GetByIdAsync(req.UserSpecializationId.Value, ct);
            if (spec == null)
                return Result<Unit>.Failure("Specialization not found");

            // Specialization, kullanıcı tipine ait olmalı
            if (spec.UserTypeId != user.UserTypeId)
                return Result<Unit>.Failure("Specialization does not belong to the selected UserType");

            user.SetSpecialization(req.UserSpecializationId.Value);
        }
        else
        {
            // frontend boş gönderirse uzmanlığı temizle
            user.SetSpecialization(null);
        }

        await _users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        // === Audit ===
        await _audit.AddAsync(AuditLog.Create(
            req.PerformedByUserId,
            "user_update",
            "User",
            user.Id,
            _current.GetIpAddress(),
            new
            {
                ChangedBy = _current.GetCurrentUserId(),
                ChangedFields = new
                {
                    NameChanged = (oldData.FirstName != req.FirstName || oldData.LastName != req.LastName),
                    PiiChanged = canEditPII &&
                                 (oldData.PhoneEncrypted != phone || oldData.AddressEncrypted != address || oldData.NationalIdEncrypted != nationalId),
                    UserTypeChanged = oldData.UserTypeId != req.UserTypeId,
                    GenderChanged = oldData.Gender != req.Gender,
                    BirthDateChanged = oldData.BirthDate != req.BirthDate
                }
            }.ToString()
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}