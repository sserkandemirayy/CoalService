using Domain.Abstractions;

namespace Domain.Entities;

public class User : BaseEntity
{
    protected User() { }

    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;

    public string? Identifier { get; private set; }
    public string? Gender { get; private set; }

    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? NationalIdEncrypted { get; private set; }
    public string? PhoneEncrypted { get; private set; }
    public string? AddressEncrypted { get; private set; }
    public DateTime? BirthDate { get; private set; }

    public string? EmergencyContactNameEncrypted { get; private set; }
    public string? EmergencyContactPhoneEncrypted { get; private set; }

    public DateTime? KvkConsentAt { get; private set; }
    public string? KvkConsentVersion { get; private set; }

    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }

    // === UserType (zorunlu) ===
    public Guid UserTypeId { get; private set; }
    public UserType UserType { get; private set; } = default!;

    public Guid? SpecializationId { get; private set; }
    public UserSpecialization? Specialization { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<UserCompany> UserCompanies { get; private set; } = new List<UserCompany>();
    public ICollection<UserBranch> UserBranches { get; private set; } = new List<UserBranch>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    public ICollection<CommandRequest> RequestedCommands { get; private set; } = new List<CommandRequest>();
    public ICollection<CommandStatusHistory> CommandStatusChanges { get; private set; } = new List<CommandStatusHistory>();

    public static User Create(
        string email,
        string passwordHash,
        string? firstName = null,
        string? lastName = null,
        Guid userTypeId = default)
        => new User
        {
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            UserTypeId = userTypeId != Guid.Empty
                          ? userTypeId
                          : throw new ArgumentNullException(nameof(userTypeId), "UserType is required")
        };

    public void SetUserType(Guid typeId)
    {
        if (typeId == Guid.Empty)
            throw new ArgumentException("UserType is required", nameof(typeId));

        UserTypeId = typeId;
    }

    public void AssignRole(Role role)
    {
        var existing = UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (existing != null)
        {
            if (existing.DeletedAt != null)
            {
                existing.DeletedAt = null;
                existing.DeletedBy = null;
            }
            return;
        }

        UserRoles.Add(UserRole.Create(Id, role.Id));
    }

    public void RemoveRole(Role role)
    {
        var ur = UserRoles.FirstOrDefault(r => r.RoleId == role.Id && r.DeletedAt == null);
        if (ur != null)
        {
            ur.DeletedAt = DateTime.UtcNow;
            ur.DeletedBy = Id;
        }
    }

    public void RecordLoginSuccess()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    public void RecordLoginFailure(int maxAttempts, TimeSpan lockoutDuration)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
            LockoutEnd = DateTime.UtcNow.Add(lockoutDuration);
    }

    public void GiveKvkConsent(string version)
    {
        KvkConsentAt = DateTime.UtcNow;
        KvkConsentVersion = version;
    }

    public void UpdateProfile(string firstName, string lastName, string phone, string address, string nationalId, string gender)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneEncrypted = phone;
        AddressEncrypted = address;
        NationalIdEncrypted = nationalId;
        Gender = gender;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void SetSpecialization(Guid? specializationId)
    {
        SpecializationId = specializationId;
    }

    public void SetBirthDate(DateTime? birthDate)
    {
        BirthDate = birthDate;
    }

}