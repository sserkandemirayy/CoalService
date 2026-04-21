using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

public record SyncUserBranchesCommand(
    Guid UserId,
    Guid CompanyId,
    List<Guid> BranchIds,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;


public class SyncUserBranchesHandler : IRequestHandler<SyncUserBranchesCommand, Result<Unit>>
{
    private readonly IUserBranchRepository _repo;
    private readonly IUserCompanyRepository _userCompanies;
    private readonly IBranchRepository _branches;
    private readonly IUnitOfWork _uow;

    public SyncUserBranchesHandler(
        IUserBranchRepository repo,
        IUserCompanyRepository userCompanies,
        IBranchRepository branches,
        IUnitOfWork uow)
    {
        _repo = repo;
        _userCompanies = userCompanies;
        _branches = branches;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(SyncUserBranchesCommand req, CancellationToken ct)
    {
        // --- 1) Kullanıcı bu company'e bağlı mı? ---
        if (!await _userCompanies.IsUserInCompanyAsync(req.UserId, req.CompanyId, ct))
            return Result<Unit>.Failure("User is not assigned to this company.");

        // --- 2) Bu company’e ait tüm branchler ---
        var companyBranches = await _branches.GetByCompanyIdAsync(req.CompanyId, ct);
        var companyBranchIds = companyBranches.Select(x => x.Id).ToHashSet();

        // --- 3) Kullanıcının sadece bu şirketteki branchleri ---
        var userBranchIds = (await _repo.GetUserBranchIdsAsync(req.UserId, ct))
            .Where(id => companyBranchIds.Contains(id))
            .ToHashSet();

        // --- 4) Eklenmesi gerekenler ---
        var toAdd = req.BranchIds.Except(userBranchIds).ToList();

        // --- 5) Silinmesi gerekenler (sadece bu şirket için!) ---
        var toRemove = userBranchIds.Except(req.BranchIds).ToList();

        // --- 6) Ekle ---
        foreach (var branchId in toAdd)
        {
            await _repo.AddOrReactivateAsync(
                req.UserId,
                branchId,
                req.PerformedByUserId,
                ct
            );
        }

        // --- 7) Sil ---
        foreach (var branchId in toRemove)
        {
            await _repo.RemoveAsync(
                req.UserId,
                branchId,
                req.PerformedByUserId,
                ct
            );
        }

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}