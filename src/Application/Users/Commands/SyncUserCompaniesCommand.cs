using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands;

public record SyncUserCompaniesCommand(
    Guid UserId,
    List<Guid> Add,
    List<Guid> Remove,
    Guid PerformedByUserId,
    Dictionary<Guid, string?>? UserCodes = null
) : IRequest<Result<Unit>>;

public class SyncUserCompaniesHandler
    : IRequestHandler<SyncUserCompaniesCommand, Result<Unit>>
{
    private readonly IUserCompanyRepository _repo;
    private readonly IUnitOfWork _uow;

    public SyncUserCompaniesHandler(
        IUserCompanyRepository repo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(
        SyncUserCompaniesCommand req,
        CancellationToken ct)
    {
        var current =
            (await _repo.GetCompaniesByUserIdAsync(
                req.UserId,
                ct))
            .Select(x => x.Id)
            .ToList();

        var toAdd =
            req.Add
                .Distinct()
                .Except(current)
                .ToList();

        var toRemove =
            req.Remove
                .Distinct()
                .Intersect(current)
                .ToList();

        foreach (var companyId in toAdd)
        {
            string? requestedUserCode = null;

            if (req.UserCodes is not null &&
                req.UserCodes.TryGetValue(
                    companyId,
                    out var code))
            {
                requestedUserCode = code;
            }

            string? generatedUserCode;

            try
            {
                generatedUserCode =
                    await _repo.AddOrReactivateAsync(
                        req.UserId,
                        companyId,
                        requestedUserCode,
                        ct);
            }
            catch (InvalidOperationException ex)
            {
                return Result<Unit>.Failure(
                    ex.Message);
            }

            if (string.IsNullOrWhiteSpace(
                    generatedUserCode))
            {
                return Result<Unit>.Failure(
                    $"User could not be assigned to company {companyId}");
            }
        }

        foreach (var companyId in toRemove)
        {
            await _repo.RemoveAsync(
                req.UserId,
                companyId,
                req.PerformedByUserId,
                ct);
        }

        await _uow.SaveChangesAsync(ct);

        return Result<Unit>.Success(
            Unit.Value);
    }
}