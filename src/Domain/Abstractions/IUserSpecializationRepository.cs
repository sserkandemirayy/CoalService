using Domain.Entities;

public interface IUserSpecializationRepository
{
    Task<IEnumerable<UserSpecialization>> GetAllAsync(CancellationToken ct = default);
    Task<UserSpecialization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<UserSpecialization>> GetByUserTypeIdAsync(Guid userTypeId, CancellationToken ct = default);

    Task AddAsync(UserSpecialization entity, CancellationToken ct = default);
    Task UpdateAsync(UserSpecialization entity, CancellationToken ct = default);
    Task RemoveAsync(Guid id, Guid performedBy, CancellationToken ct = default);

    Task<bool> HasUsersAsync(Guid specializationId, CancellationToken ct = default);
}