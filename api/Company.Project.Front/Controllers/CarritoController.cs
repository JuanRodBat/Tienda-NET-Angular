using System.Security.Claims;
using Company.Project.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Project.Front.Controllers;

[ApiController]
[Route("api/carrito")]
[Authorize] // requiere JWT
public class CarritoController : ControllerBase
{
    private readonly ICarritoService _svc;
    public CarritoController(ICarritoService svc) => _svc = svc;

    int ClienteId() => int.Parse(User.FindFirstValue("cid")!);

    public record AddDto(int ArticuloId, int TiendaId, int Cantidad);

    [HttpGet("items")]
    public async Task<IActionResult> GetItems() =>
        Ok((await _svc.GetItemsAsync(ClienteId())).Select(i => new {
            i.Id,
            i.ArticuloId,
            i.TiendaId,
            i.Cantidad,
            Articulo = new { i.Articulo.Descripcion, i.Articulo.Precio, i.Articulo.Imagen },
            Tienda = new { i.Tienda.Sucursal }
        }));

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddDto dto)
    {
        await _svc.AddItemAsync(ClienteId(), dto.ArticuloId, dto.TiendaId, dto.Cantidad);
        return NoContent();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        await _svc.CheckoutAsync(ClienteId());
        return NoContent();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        await _svc.ClearAsync(ClienteId());
        return NoContent();
    }

    public record RemoveDto(int ArticuloId, int TiendaId);

    [HttpDelete("item")]
    public async Task<IActionResult> Remove([FromBody] RemoveDto dto)
    {
        await _svc.RemoveItemAsync(ClienteId(), dto.ArticuloId, dto.TiendaId);
        return NoContent();
    }
}
