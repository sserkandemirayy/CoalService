using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Queries;

public record GetUserCompaniesQuery(Guid UserId) : IRequest<Result<IEnumerable<Company>>>;

public class GetUserCompaniesHandler : IRequestHandler<GetUserCompaniesQuery, Result<IEnumerable<Company>>>
{
    private readonly IUserCompanyRepository _repo;

    public GetUserCompaniesHandler(IUserCompanyRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<Company>>> Handle(GetUserCompaniesQuery req, CancellationToken ct)
    {
        var companies = await _repo.GetCompaniesByUserIdAsync(req.UserId, ct);
        return Result<IEnumerable<Company>>.Success(companies);
    }
}