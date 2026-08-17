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

public class SyncCompanyUsersHandler
    : IRequestHandler<SyncCompanyUsersCommand, Result<Unit>>
{
    private readonly IUserCompanyRepository _repo;
    private readonly IUnitOfWork _uow;

    public SyncCompanyUsersHandler(
        IUserCompanyRepository repo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(
        SyncCompanyUsersCommand req,
        CancellationToken ct)
    {
        // Şirkete şu anda atanmış kullanıcılar
        var current = (await _repo.GetUsersByCompanyIdAsync(
                req.CompanyId,
                ct))
            .Select(x => x.Id)
            .ToList();

        // Gerçekten eklenecek kullanıcılar
        var toAdd = req.Add
            .Distinct()
            .Except(current)
            .ToList();

        // Gerçekten çıkarılacak kullanıcılar
        var toRemove = req.Remove
            .Distinct()
            .Intersect(current)
            .ToList();

        foreach (var userId in toAdd)
        {
            // Burada manuel kullanıcı kodu verilmediği için null gönderiyoruz.
            // Repository şirket bazında otomatik U0000001, U0000002... üretir.
            var userCode = await _repo.AddOrReactivateAsync(
                userId,
                req.CompanyId,
                requestedUserCode: null,
                ct: ct);

            if (string.IsNullOrWhiteSpace(userCode))
            {
                return Result<Unit>.Failure(
                    $"User '{userId}' could not be assigned to company '{req.CompanyId}'.");
            }
        }

        foreach (var userId in toRemove)
        {
            await _repo.RemoveAsync(
                userId,
                req.CompanyId,
                req.PerformedByUserId,
                ct);
        }

        await _uow.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}