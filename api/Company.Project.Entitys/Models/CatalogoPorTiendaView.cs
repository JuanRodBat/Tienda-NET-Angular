using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Entitys.Models;

public class CatalogoPorTiendaView
{
    public int TiendaId { get; set; }
    public string Sucursal { get; set; } = "";
    public int ArticuloId { get; set; }
    public string Codigo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? Imagen { get; set; }
}
