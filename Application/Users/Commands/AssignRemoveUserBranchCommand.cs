using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

public record AssignRemoveUserBranchCommand(Guid UserId, Guid BranchId)
    : IRequest<Result<Unit>>;

public class AssignUserBranchHandler
    : IRequestHandler<AssignRemoveUserBranchCommand, Result<Unit>>
{
    private readonly IUserBranchRepository _repo;
    private readonly IBranchRepository _branches;
    private readonly IUserCompanyRepository _userCompanies;
    private readonly ICurrentUserService _current;

    public AssignUserBranchHandler(
        IUserBranchRepository repo,
        IBranchRepository branches,
        IUserCompanyRepository userCompanies,
        ICurrentUserService current)
    {
        _repo = repo;
        _branches = branches;
        _userCompanies = userCompanies;
        _current = current;
    }

    public async Task<Result<Unit>> Handle(AssignRemoveUserBranchCommand cmd, CancellationToken ct)
    {
        // Branch var mı kontrol et
        var branch = await _branches.GetByIdAsync(cmd.BranchId, ct);
        if (branch == null)
            return Result<Unit>.Failure("Branch not found.");

        // User bu branch'in şirketine bağlı mı?
        var companyId = branch.CompanyId;

        var isUserInCompany = await _userCompanies
            .IsUserInCompanyAsync(cmd.UserId, companyId, ct);

        if (!isUserInCompany)
            return Result<Unit>.Failure("User is not assigned to the company of this branch.");

        // User Branch ilişkisinde ekleme veya reaktif etme
        await _repo.AddOrReactivateAsync(
            cmd.UserId,
            cmd.BranchId,
            _current.GetCurrentUserId(),
            ct
        );

        return Result<Unit>.Success(Unit.Value);
    }
}

public record RemoveBranchCommand(Guid UserId, Guid BranchId)
    : IRequest<Result<Unit>>;

public class RemoveBranchHandler : IRequestHandler<RemoveBranchCommand, Result<Unit>>
{
    private readonly IUserBranchRepository _repo;
    private readonly ICurrentUserService _current;

    public RemoveBranchHandler(IUserBranchRepository repo, ICurrentUserService current)
    {
        _repo = repo;
        _current = current;
    }

    public async Task<Result<Unit>> Handle(RemoveBranchCommand cmd, CancellationToken ct)
    {
        await _repo.RemoveAsync(
            cmd.UserId,
            cmd.BranchId,
            _current.GetCurrentUserId(),
            ct
        );

        return Result<Unit>.Success(Unit.Value);
    }
}