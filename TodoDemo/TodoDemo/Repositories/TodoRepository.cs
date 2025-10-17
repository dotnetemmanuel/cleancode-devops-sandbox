using Microsoft.EntityFrameworkCore;
using TodoDemo.Data;
using TodoDemo.Models;

namespace TodoDemo.Repositories;

public class TodoRepository: ITodoRepository
{
    private readonly TodoDbContext _db;

    public TodoRepository(TodoDbContext db) => _db = db;

    public async Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Todos
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Todos.FindAsync(new object[] { id }, ct);
    }

    public async Task<TodoItem> CreateAsync(TodoItem item, CancellationToken ct = default)
    {
        item.CreatedAt = DateTime.UtcNow;
        _db.Todos.Add(item);
        await _db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<bool> UpdateAsync(TodoItem item, CancellationToken ct = default)
    {
        var existing = await _db.Todos.FindAsync(new object[] { item.Id }, ct);
        if (existing == null) return false;

        existing.Title = item.Title;
        existing.Description = item.Description;
        existing.IsDone = item.IsDone;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _db.Todos.FindAsync(new object[] { id }, ct);
        if (existing == null) return false;

        _db.Todos.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task EnsureCreatedAsync(CancellationToken ct = default)
    {
        await _db.Database.EnsureCreatedAsync(ct);
    }
}