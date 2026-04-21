using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

public record UpsertSettingCommand(string Key, string Value, SettingScope Scope, Guid? UserId) : IRequest<Result<Unit>>;

public class UpsertSettingHandler : IRequestHandler<UpsertSettingCommand, Result<Unit>>
{
    private readonly ISettingRepository _repo;
    private readonly IUnitOfWork _uow;
    public UpsertSettingHandler(ISettingRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(UpsertSettingCommand r, CancellationToken ct)
    {
        var existing = await _repo.GetAsync(r.Key, r.UserId, ct);
        if (existing != null)
            existing.UpdateValue(r.Value);
        else
            await _repo.AddAsync(Setting.Create(r.Key, r.Value, r.Scope, r.UserId), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
