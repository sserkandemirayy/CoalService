using Domain.Entities;

namespace Domain.Abstractions;

public interface ISettingRepository
{
    Task<Setting?> GetAsync(string key, Guid? userId = null, CancellationToken ct = default);
    Task<IEnumerable<Setting>> GetAllAsync(SettingScope scope, Guid? userId = null, CancellationToken ct = default);
    Task AddAsync(Setting setting, CancellationToken ct = default);
    Task UpdateAsync(Setting setting, CancellationToken ct = default);
    Task DeleteAsync(Setting setting, CancellationToken ct = default);
}
