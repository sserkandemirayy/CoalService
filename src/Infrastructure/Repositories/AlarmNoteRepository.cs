using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlarmNoteRepository : IAlarmNoteRepository
{
    private readonly AppDbContext _db;

    public AlarmNoteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(
        AlarmNote note,
        CancellationToken ct = default)
    {
        await _db.AlarmNotes.AddAsync(note, ct);
    }

    public async Task<IReadOnlyList<AlarmNote>> GetByAlarmIdAsync(
        Guid alarmId,
        CancellationToken ct = default)
    {
        return await _db.AlarmNotes
            .Include(x => x.User)
            .Where(x => x.AlarmId == alarmId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);
    }
}