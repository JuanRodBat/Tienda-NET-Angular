using Company.Project.Data;
using Company.Project.Entitys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Company.Project.Data.Interfaces;

namespace Company.Project.Front.Controllers;

[ApiController]
[Route("api/articulos")]
public class ArticulosController : ControllerBase
{
    private readonly IRepository<Articulo> _repo;
    private readonly AppDbContext _ctx;

    public ArticulosController(IRepository<Articulo> repo, AppDbContext ctx)
    {
        _repo = repo;
        _ctx = ctx;
    }

    // ---- CRUD Articulo ----
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Articulo>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Articulo>> GetById(int id)
    {
        var a = await _repo.GetByIdAsync(id);
        return a is null ? NotFound() : Ok(a);
    }

    [HttpPost]
    public async Task<ActionResult<Articulo>> Create([FromBody] Articulo a)
    {
        if (string.IsNullOrWhiteSpace(a.Codigo) || string.IsNullOrWhiteSpace(a.Descripcion))
            return BadRequest("Código y Descripción son requeridos.");
        var created = await _repo.AddAsync(a);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Articulo a)
    {
        if (id != a.Id) return BadRequest("Id de ruta y cuerpo no coinciden.");
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Codigo = a.Codigo;
        existing.Descripcion = a.Descripcion;
        existing.Precio = a.Precio;
        existing.Imagen = a.Imagen;
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

    // ---- Stock por tienda (ArticuloTienda) ----

    public record StockDto(int TiendaId, int Stock);

    [HttpGet("{id:int}/stock")]
    public async Task<IActionResult> GetStock(int id)
    {
        var rows = await _ctx.ArticuloTiendas
            .Where(at => at.ArticuloId == id)
            .Select(at => new { at.TiendaId, at.Stock, at.Fecha })
            .ToListAsync();
        return Ok(rows);
    }

    // Upsert de stock: crea o actualiza el registro ArticuloTienda
    [HttpPost("{id:int}/stock")]
    public async Task<IActionResult> UpsertStock(int id, [FromBody] StockDto dto)
    {
        var articulo = await _repo.GetByIdAsync(id);
        if (articulo is null) return NotFound("Artículo no existe.");

        var tienda = await _ctx.Tiendas.FindAsync(dto.TiendaId);
        if (tienda is null) return BadRequest("Tienda inválida.");

        var at = await _ctx.ArticuloTiendas.FindAsync(id, dto.TiendaId);
        if (at is null)
        {
            at = new ArticuloTienda { ArticuloId = id, TiendaId = dto.TiendaId, Stock = dto.Stock, Fecha = DateTime.UtcNow };
            _ctx.ArticuloTiendas.Add(at);
        }
        else
        {
            at.Stock = dto.Stock;
            at.Fecha = DateTime.UtcNow;
            _ctx.ArticuloTiendas.Update(at);
        }
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}
