using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands;

public record SyncUserCompaniesCommand(
    Guid UserId,
    List<Guid> Add,
    List<Guid> Remove,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;

public class SyncUserCompaniesHandler : IRequestHandler<SyncUserCompaniesCommand, Result<Unit>>
{
    private readonly IUserCompanyRepository _repo;
    private readonly IUnitOfWork _uow;

    public SyncUserCompaniesHandler(IUserCompanyRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(SyncUserCompaniesCommand req, CancellationToken ct)
    {
        // mevcut ilişkiler
        var current = (await _repo.GetCompaniesByUserIdAsync(req.UserId, ct))
            .Select(x => x.Id)
            .ToList();

        // Eklenmesi gerekenler
        var toAdd = req.Add.Except(current).ToList();
        // Silinmesi gerekenler
        var toRemove = req.Remove.Intersect(current).ToList();

        foreach (var companyId in toAdd)
            await _repo.AddOrReactivateAsync(req.UserId, companyId, ct);

        foreach (var companyId in toRemove)
            await _repo.RemoveAsync(req.UserId, companyId, req.PerformedByUserId, ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}