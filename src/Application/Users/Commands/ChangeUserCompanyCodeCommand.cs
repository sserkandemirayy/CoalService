using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands;

public sealed record ChangeUserCompanyCodeCommand(
    Guid UserId,
    Guid CompanyId,
    string UserCode,
    Guid PerformedByUserId)
    : IRequest<Result<Unit>>;

public sealed class ChangeUserCompanyCodeCommandHandler
    : IRequestHandler<
        ChangeUserCompanyCodeCommand,
        Result<Unit>>
{
    private readonly IUserCompanyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeUserCompanyCodeCommandHandler(
        IUserCompanyRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(
        ChangeUserCompanyCodeCommand request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(
                request.UserCode))
        {
            return Result<Unit>.Failure(
                "UserCode is required.");
        }

        try
        {
            var updated =
                await _repository.SetUserCodeAsync(
                    request.UserId,
                    request.CompanyId,
                    request.UserCode,
                    ct);

            if (!updated)
            {
                return Result<Unit>.Failure(
                    "User-company relation not found or you do not have access to the company.");
            }

            await _unitOfWork
                .SaveChangesAsync(ct);

            return Result<Unit>.Success(
                Unit.Value);
        }
        catch (ArgumentException ex)
        {
            return Result<Unit>.Failure(
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Result<Unit>.Failure(
                ex.Message);
        }
    }
}