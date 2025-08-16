using Company.Project.Data.Interfaces;
using Company.Project.Entitys.Models;
using Microsoft.AspNetCore.Mvc;

namespace Company.Project.Front.Controllers;

[ApiController]
[Route("api/tiendas")]
public class TiendasController : ControllerBase
{
    private readonly IRepository<Tienda> _repo;
    public TiendasController(IRepository<Tienda> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tienda>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Tienda>> GetById(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpPost]
    public async Task<ActionResult<Tienda>> Create([FromBody] Tienda tienda)
    {
        if (string.IsNullOrWhiteSpace(tienda.Sucursal))
            return BadRequest("Sucursal es requerida.");

        var created = await _repo.AddAsync(tienda);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Tienda tienda)
    {
        if (id != tienda.Id) return BadRequest("Id de ruta y cuerpo no coinciden.");
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Sucursal = tienda.Sucursal;
        existing.Direccion = tienda.Direccion;
        await _repo.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return NotFound();

        await _repo.DeleteAsync(existing);
        return NoContent();
    }
}
