using Microsoft.AspNetCore.Mvc;
using Shared.Grains;
using Shared.Models;

namespace GameServicesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;
    private readonly IClusterClient _client;
    
    public TodoController(ILogger<TodoController> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;
    }
    

    [HttpGet("{ownerKey:guid}")]
    public async Task<ActionResult<List<TodoItem>>> Get(Guid ownerKey)
    {
        var grain = _client.GetGrain<ITodoGrain>(ownerKey);
        var todos = await grain.GetTodos();

        return todos;
    }

    [HttpPost("{ownerKey:guid}")]
    public async Task<IActionResult> Post(Guid ownerKey, [FromBody] CreateTodoDto todoDto)
    {
        var todoItem = new TodoItem(Guid.NewGuid(), todoDto.Title, todoDto.IsDone, ownerKey);
        var grain = _client.GetGrain<ITodoGrain>(ownerKey);
        await grain.AddTodo(todoItem);
        _logger.LogInformation("Adding {TodoItem}", todoItem);
        return Ok();
    }
    
    [HttpPut("{ownerKey:guid}")]
    public async Task<IActionResult> Put(Guid ownerKey, [FromBody] TodoItem todo)
    {
        var grain = _client.GetGrain<ITodoGrain>(ownerKey);
        await grain.UpdateTodo(todo);
        _logger.LogInformation("Update {TodoItem}", todo);
        return Ok();
    }

    [HttpDelete("{ownerKey:guid}/{id:guid}")]
    public async Task<IActionResult> Delete(Guid ownerKey, Guid id)
    {
        var grain = _client.GetGrain<ITodoGrain>(ownerKey);
        await grain.RemoveTodoByGuid(id);
        _logger.LogInformation("Delete {Id}", id);
        return Ok();
    }

    [HttpDelete("{ownerKey:guid}/clear")]
    public async Task<IActionResult> Clear(Guid ownerKey)
    {
        var grain = _client.GetGrain<ITodoGrain>(ownerKey);
        await grain.ClearTodos();
        _logger.LogInformation("Delete all Todo of owner {Id}", ownerKey);
        return Ok();
    }
    
    // [HttpGet("/list")]
    // public async Task<IActionResult> List()
    // {
    //     var grains = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAsync();
    //     return Ok(grains);
    // }
    //
    // [HttpGet("{key:guid}")]
    // public async Task<IActionResult> Get(Guid key)
    // {
    //     var grain = await _client.GetGrain<ITodoGrain>(key).GetAsync();
    //     return Ok(grain);
    // }

    // [HttpPost()]
    // public async Task<IActionResult> Create(TodoItem item)
    // {
    //     _logger.LogInformation("Adding {@item}.", item);
    //     await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);
    //
    //     return Ok();
    // }
    
    // [HttpPut()]
    // public async Task<IActionResult> Update(TodoItem item)
    // {
    //     _logger.LogInformation("Updating {@item}.", item);
    //     await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);
    //
    //     return Ok();
    // }
    
    // [HttpDelete()]
    // public async Task<IActionResult> Delete(Guid key)
    // {
    //     _logger.LogInformation("Deleting {@item}.", item);
    //     await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);
    //
    //     return Ok();
    // }
    //
    // [HttpDelete("clear")]
    // public async Task<IActionResult> Clear(TodoItem item)
    // {
    //     _logger.LogInformation("Deleting {@item}.", item);
    //     await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);
    //
    //     return Ok();
    // }
}