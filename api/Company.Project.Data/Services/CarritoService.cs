using Microsoft.EntityFrameworkCore;
using Company.Project.Data.Interfaces;
using Company.Project.Entitys.Models;

namespace Company.Project.Data;

public class CarritoService : ICarritoService
{
    private readonly AppDbContext _ctx;
    public CarritoService(AppDbContext ctx) => _ctx = ctx;

    public async Task<int> GetOrCreateCarritoIdAsync(int clienteId)
    {
        var car = await _ctx.Carritos.FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        if (car is null)
        {
            car = new Carrito { ClienteId = clienteId, FechaCreacion = DateTime.UtcNow };
            _ctx.Carritos.Add(car);
            await _ctx.SaveChangesAsync();
        }
        return car.Id;
    }

    public async Task AddItemAsync(int clienteId, int articuloId, int tiendaId, int cantidad)
    {
        var carritoId = await GetOrCreateCarritoIdAsync(clienteId);

        var at = await _ctx.ArticuloTiendas.FindAsync(articuloId, tiendaId)
                 ?? throw new InvalidOperationException("Artículo no disponible en la tienda.");
        if (at.Stock < cantidad) throw new InvalidOperationException("Stock insuficiente.");

        var item = await _ctx.CarritoItems
            .FirstOrDefaultAsync(i => i.CarritoId == carritoId && i.ArticuloId == articuloId && i.TiendaId == tiendaId);

        if (item is null)
        {
            item = new CarritoItem { CarritoId = carritoId, ArticuloId = articuloId, TiendaId = tiendaId, Cantidad = cantidad, Fecha = DateTime.UtcNow };
            _ctx.CarritoItems.Add(item);
        }
        else
        {
            item.Cantidad += cantidad;
            _ctx.CarritoItems.Update(item);
        }
        await _ctx.SaveChangesAsync();
    }

    public Task<List<CarritoItem>> GetItemsAsync(int clienteId) =>
        _ctx.CarritoItems
            .Where(i => i.Carrito.ClienteId == clienteId)
            .Include(i => i.Articulo).Include(i => i.Tienda)
            .AsNoTracking()
            .ToListAsync();

    public async Task CheckoutAsync(int clienteId)
    {
        var carrito = await _ctx.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId)
            ?? throw new InvalidOperationException("No hay carrito.");

        foreach (var i in carrito.Items)
        {
            var at = await _ctx.ArticuloTiendas.FindAsync(i.ArticuloId, i.TiendaId)
                     ?? throw new InvalidOperationException("Artículo/Tienda inválidos.");
            if (at.Stock < i.Cantidad) throw new InvalidOperationException("Stock insuficiente.");
            at.Stock -= i.Cantidad;

            _ctx.ClienteArticulos.Add(new ClienteArticulo
            {
                ClienteId = clienteId,
                ArticuloId = i.ArticuloId,
                Cantidad = i.Cantidad,
                Fecha = DateTime.UtcNow
            });
        }

        _ctx.CarritoItems.RemoveRange(carrito.Items);
        await _ctx.SaveChangesAsync();
    }
    public async Task ClearAsync(int clienteId)
    {
        var car = await _ctx.Carritos.Include(c => c.Items)
                     .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        if (car is null) return;
        _ctx.CarritoItems.RemoveRange(car.Items);
        await _ctx.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(int clienteId, int articuloId, int tiendaId)
    {
        var car = await _ctx.Carritos.FirstOrDefaultAsync(c => c.ClienteId == clienteId)
                  ?? throw new InvalidOperationException("No hay carrito.");
        var item = await _ctx.CarritoItems.FirstOrDefaultAsync(i =>
            i.CarritoId == car.Id && i.ArticuloId == articuloId && i.TiendaId == tiendaId);
        if (item is null) return;
        _ctx.CarritoItems.Remove(item);
        await _ctx.SaveChangesAsync();
    }
}
