using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Entitys.Models;

public class CarritoItem
{
    public int Id { get; set; }
    public int CarritoId { get; set; }
    public int ArticuloId { get; set; }
    public int TiendaId { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public Carrito Carrito { get; set; } = null!;
    public Articulo Articulo { get; set; } = null!;
    public Tienda Tienda { get; set; } = null!;
}