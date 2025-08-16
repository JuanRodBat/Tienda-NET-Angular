using Company.Project.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company.Project.Front.Controllers;

[ApiController]
[Route("api/tiendas/{tiendaId:int}/articulos")]
public class CatalogoController : ControllerBase
{
    private readonly AppDbContext _ctx;
    public CatalogoController(AppDbContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> Get(int tiendaId) =>
        Ok(await _ctx.CatalogoPorTienda
            .Where(v => v.TiendaId == tiendaId)
            .ToListAsync());
}
