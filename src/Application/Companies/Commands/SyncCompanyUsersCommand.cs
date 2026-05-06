using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.Companies.Commands;

public record SyncCompanyUsersCommand(
    Guid CompanyId,
    List<Guid> Add,
    List<Guid> Remove,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;

public class SyncCompanyUsersHandler : IRequestHandler<SyncCompanyUsersCommand, Result<Unit>>
{
    private readonly IUserCompanyRepository _repo;
    private readonly IUnitOfWork _uow;

    public SyncCompanyUsersHandler(IUserCompanyRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;       
    }

    public async Task<Result<Unit>> Handle(SyncCompanyUsersCommand req, CancellationToken ct)
    {
        // mevcut kullanıcılar
        var current = (await _repo.GetUsersByCompanyIdAsync(req.CompanyId, ct))
            .Select(x => x.Id)
            .ToList();

        var toAdd = req.Add.Except(current).ToList();
        var toRemove = req.Remove.Intersect(current).ToList();

        foreach (var userId in toAdd)
            await _repo.AddOrReactivateAsync(userId, req.CompanyId, ct);

        foreach (var userId in toRemove)
            await _repo.RemoveAsync(userId, req.CompanyId, req.PerformedByUserId, ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}