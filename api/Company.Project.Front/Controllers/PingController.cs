using Company.Project.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company.Project.Front.Controllers;

[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    private readonly AppDbContext _ctx;
    public PingController(AppDbContext ctx) => _ctx = ctx;

    [HttpGet("clientes")]
    public async Task<IActionResult> GetClientes() =>
        Ok(await _ctx.Clientes.Select(c => new { c.Id, c.Email }).ToListAsync());

    [HttpGet("catalogo/{tiendaId:int}")]
    public async Task<IActionResult> Catalogo(int tiendaId) =>
        Ok(await _ctx.CatalogoPorTienda.Where(v => v.TiendaId == tiendaId).ToListAsync());
}