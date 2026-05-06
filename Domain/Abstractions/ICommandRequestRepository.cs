using Domain.Entities;
using Domain.Enums;

namespace Domain.Abstractions;

public interface ICommandRequestRepository
{
    Task<CommandRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(CommandRequest commandRequest, CancellationToken ct = default);
    Task UpdateAsync(CommandRequest commandRequest, CancellationToken ct = default);

    Task<(IReadOnlyList<CommandRequest> Items, int Total)> GetPagedAsync(
        RtlsCommandType? commandType,
        RtlsCommandStatus? status,
        RtlsCommandTargetType? targetType,
        Guid? tagId,
        Guid? anchorId,
        Guid? requestedByUserId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<(IReadOnlyList<CommandRequest> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<(IReadOnlyList<CommandRequest> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<CommandRequest> Query();
}