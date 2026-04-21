
using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Abstractions;
using MediatR;

public record GetAllUserSpecializationsQuery()
    : IRequest<Result<IEnumerable<UserSpecializationDto>>>;

public class GetAllUserSpecializationsHandler
    : IRequestHandler<GetAllUserSpecializationsQuery, Result<IEnumerable<UserSpecializationDto>>>
{
    private readonly IUserSpecializationRepository _repo;

    public GetAllUserSpecializationsHandler(IUserSpecializationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<UserSpecializationDto>>> Handle(GetAllUserSpecializationsQuery req, CancellationToken ct)
    {
        var entities = await _repo.GetAllAsync(ct);

        var dtos = entities.Select(UserSpecializationDto.FromEntity).ToList();

        return Result<IEnumerable<UserSpecializationDto>>.Success(dtos);
    }
}

public record GetUserSpecializationByIdQuery(Guid Id)
    : IRequest<Result<UserSpecializationDto>>;

public class GetUserSpecializationByIdHandler
    : IRequestHandler<GetUserSpecializationByIdQuery, Result<UserSpecializationDto>>
{
    private readonly IUserSpecializationRepository _repo;

    public GetUserSpecializationByIdHandler(IUserSpecializationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<UserSpecializationDto>> Handle(GetUserSpecializationByIdQuery req, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(req.Id, ct);
        if (entity is null)
            return Result<UserSpecializationDto>.Failure("Not found");

        return Result<UserSpecializationDto>.Success(
            UserSpecializationDto.FromEntity(entity)
        );
    }
}

public record GetUserSpecializationsByUserTypeQuery(Guid UserTypeId)
    : IRequest<Result<IEnumerable<UserSpecializationDto>>>;

public class GetUserSpecializationsByUserTypeHandler
    : IRequestHandler<GetUserSpecializationsByUserTypeQuery, Result<IEnumerable<UserSpecializationDto>>>
{
    private readonly IUserSpecializationRepository _repo;

    public GetUserSpecializationsByUserTypeHandler(IUserSpecializationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<UserSpecializationDto>>> Handle(GetUserSpecializationsByUserTypeQuery req, CancellationToken ct)
    {
        var list = await _repo.GetByUserTypeIdAsync(req.UserTypeId, ct);

        return Result<IEnumerable<UserSpecializationDto>>.Success(
            list.Select(UserSpecializationDto.FromEntity).ToList()
        );
    }
}

