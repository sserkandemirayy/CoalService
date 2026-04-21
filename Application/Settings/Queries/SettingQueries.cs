using Domain.Abstractions;
using Domain.Entities;
using MediatR;

public record GetSettingsQuery(SettingScope Scope, Guid? UserId) : IRequest<IEnumerable<Setting>>;

public class GetSettingsHandler : IRequestHandler<GetSettingsQuery, IEnumerable<Setting>>
{
    private readonly ISettingRepository _repo;
    public GetSettingsHandler(ISettingRepository repo) => _repo = repo;

    public async Task<IEnumerable<Setting>> Handle(GetSettingsQuery r, CancellationToken ct)
        => await _repo.GetAllAsync(r.Scope, r.UserId, ct);
}
