using Application.Common.Models;
using Application.DTOs.Companies;
using Domain.Abstractions;
using MediatR;

public record GetUserBranchesByCompanyQuery(Guid UserId, Guid? CompanyId)
    : IRequest<Result<IEnumerable<BranchDto>>>;

public class GetUserBranchesByCompanyHandler
    : IRequestHandler<GetUserBranchesByCompanyQuery, Result<IEnumerable<BranchDto>>>
{
    private readonly IUserBranchRepository _branches;
    private readonly ICompanyRepository _companies;

    public GetUserBranchesByCompanyHandler(
        IUserBranchRepository branches,
        ICompanyRepository companies)
    {
        _branches = branches;
        _companies = companies;
    }

    public async Task<Result<IEnumerable<BranchDto>>> Handle(
        GetUserBranchesByCompanyQuery req, CancellationToken ct)
    {
        var allBranches = await _branches.GetBranchesByUserIdAsync(req.UserId, ct);

        if (req.CompanyId != null)
            allBranches = allBranches
                .Where(b => b.CompanyId == req.CompanyId)
                .ToList();

        return Result<IEnumerable<BranchDto>>.Success(
            allBranches.Select(BranchDto.FromEntity)
        );
    }
}