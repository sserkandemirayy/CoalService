using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Companies.Queries;

public record GetCompanyUsersQuery(Guid CompanyId) : IRequest<Result<IEnumerable<User>>>;

public class GetCompanyUsersHandler : IRequestHandler<GetCompanyUsersQuery, Result<IEnumerable<User>>>
{
    private readonly IUserCompanyRepository _repo;

    public GetCompanyUsersHandler(IUserCompanyRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<User>>> Handle(GetCompanyUsersQuery req, CancellationToken ct)
    {
        var users = await _repo.GetUsersByCompanyIdAsync(req.CompanyId, ct);
        return Result<IEnumerable<User>>.Success(users);
    }
}