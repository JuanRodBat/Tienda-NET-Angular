using Company.Project.Data.Interfaces;
using Company.Project.Data;
using Company.Project.Entitys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Company.Project.Front.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IRepository<Cliente> _repo;
    private readonly AppDbContext _ctx;

    public ClientesController(IRepository<Cliente> repo, AppDbContext ctx)
    {
        _repo = repo;
        _ctx = ctx;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetById(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> Create([FromBody] Cliente c)
    {
        if (string.IsNullOrWhiteSpace(c.Email) || string.IsNullOrWhiteSpace(c.Nombre))
            return BadRequest("Nombre y Email son requeridos.");
        var created = await _repo.AddAsync(c);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Cliente c)
    {
        if (id != c.Id) return BadRequest("Id de ruta y cuerpo no coinciden.");
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Nombre = c.Nombre;
        existing.Apellidos = c.Apellidos;
        existing.Direccion = c.Direccion;
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

    [Authorize] //Requiere JWT solo el cliente puede ver su carrito
    [HttpGet("{id:int}/compras")]
    public async Task<IActionResult> GetCompras(int id)
    {
        var cidClaim = User.FindFirstValue("cid");
        if (cidClaim is null || cidClaim != id.ToString())
            return Forbid();

        var q = from ca in _ctx.ClienteArticulos
                join a in _ctx.Articulos on ca.ArticuloId equals a.Id
                where ca.ClienteId == id
                orderby ca.Fecha descending
                select new
                {
                    ca.Fecha,
                    ca.Cantidad,
                    a.Codigo,
                    a.Descripcion,
                    a.Precio
                };

        return Ok(await q.AsNoTracking().ToListAsync());
    }
}
