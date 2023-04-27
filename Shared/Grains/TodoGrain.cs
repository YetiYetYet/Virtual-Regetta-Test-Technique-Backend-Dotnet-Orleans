using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Shared.Grains;
using Shared.Models;

namespace Silo.Grains;


public class TodoGrain : Grain, ITodoGrain
{
    private readonly ILogger<TodoGrain> _logger;
    private readonly IPersistentState<TodoGrainState> _state;
    
    private List<TodoItem> _todos = new();

    private static string GrainType => nameof(TodoGrain);
    private Guid GrainKey => this.GetPrimaryKey();

    public TodoGrain(ILogger<TodoGrain> logger, [PersistentState("State")] IPersistentState<TodoGrainState> state)
    {
        _logger = logger;
        _state = state;
    }
    public async Task AddTodo(TodoItem todo)
    {
        _todos.Add(todo);
        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Todo}",
            GrainType, GrainKey, todo);
        await SaveChanges();
    }

    public async Task UpdateTodo(TodoItem todo)
    {
        var index = _todos.FindIndex(t => t.Key == todo.Key);
        if (index >= 0)
        {
            _logger.LogInformation(
                "{@GrainType} {@GrainKey} now update {@Todo} to {@UpdatedTodo}",
                GrainType, GrainKey, _todos[index], todo);
            _todos[index] = todo;
            await SaveChanges();
        }
    }

    public async Task RemoveTodoByGuid(Guid id)
    {
        _todos.RemoveAll(t => t.Key == id);
        _logger.LogInformation(
            "{@GrainType} {@GrainKey} remove with  {Id}",
            GrainType, GrainKey, id);
        await SaveChanges();
    }

    public async Task ClearTodos()
    {
        _todos.Clear();
        await SaveChanges();
    }

    public Task<List<TodoItem>> GetTodos()
    {
        return Task.FromResult(_todos);
    }
    
    private async Task SaveChanges()
    {
        _state.State.Todos = _todos;
        await _state.WriteStateAsync();
    }
    

    [GenerateSerializer]
    public class TodoGrainState
    {
        [Id(0)] public List<TodoItem> Todos { get; set; } = new();
    }
}