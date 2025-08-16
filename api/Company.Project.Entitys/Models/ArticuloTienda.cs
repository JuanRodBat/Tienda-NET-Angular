using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Entitys.Models;

public class ArticuloTienda
{
    public int ArticuloId { get; set; }
    public int TiendaId { get; set; }
    public int Stock { get; set; }
    public DateTime Fecha { get; set; }
    public Articulo Articulo { get; set; } = null!;
    public Tienda Tienda { get; set; } = null!;
}
