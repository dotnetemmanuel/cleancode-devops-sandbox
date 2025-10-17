using TodoDemo.Models;

namespace TodoDemo.Repositories;

public interface ITodoRepository
{
    Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoItem> CreateAsync(TodoItem item, CancellationToken ct = default);
    Task<bool> UpdateAsync(TodoItem item, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}