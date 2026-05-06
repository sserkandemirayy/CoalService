using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands;

public record RemoveUserFromCompanyCommand(Guid UserId, Guid CompanyId, Guid PerformedByUserId)
    : IRequest<Result<Unit>>;

public class RemoveUserFromCompanyHandler : IRequestHandler<RemoveUserFromCompanyCommand, Result<Unit>>
{
    private readonly IUserCompanyRepository _repo;
    private readonly IUnitOfWork _uow;

    public RemoveUserFromCompanyHandler(IUserCompanyRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(RemoveUserFromCompanyCommand req, CancellationToken ct)
    {
        await _repo.RemoveAsync(req.UserId, req.CompanyId, req.PerformedByUserId, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}