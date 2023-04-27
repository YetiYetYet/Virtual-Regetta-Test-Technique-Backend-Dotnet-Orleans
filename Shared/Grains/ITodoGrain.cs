using Shared.Models;

namespace Shared.Grains;

public interface ITodoGrain : IGrainWithGuidKey
{
    Task AddTodo(TodoItem todo);
    Task UpdateTodo(TodoItem todo);
    Task RemoveTodoByGuid(Guid id);
    Task ClearTodos();
    Task<List<TodoItem>> GetTodos();
    
}