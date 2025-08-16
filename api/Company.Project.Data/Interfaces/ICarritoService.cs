using Company.Project.Entitys.Models;

namespace Company.Project.Data.Interfaces;

public interface ICarritoService
{
    Task<int> GetOrCreateCarritoIdAsync(int clienteId);
    Task AddItemAsync(int clienteId, int articuloId, int tiendaId, int cantidad);
    Task<List<CarritoItem>> GetItemsAsync(int clienteId);
    Task CheckoutAsync(int clienteId); 
    Task ClearAsync(int clienteId);
    Task RemoveItemAsync(int clienteId, int articuloId, int tiendaId);
}
