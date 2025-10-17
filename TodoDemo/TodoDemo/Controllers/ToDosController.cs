using Microsoft.AspNetCore.Mvc;
using TodoDemo.Models;
using TodoDemo.Repositories;

namespace TodoDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToDosController(ITodoRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
    {
        var list = await repo.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> Get(int id)
    {
        var item = await repo.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] TodoItem dto)
    {
        var created = await repo.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TodoItem dto)
    {
        if (id != dto.Id) return BadRequest();
        var ok = await repo.UpdateAsync(dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await repo.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}