using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Entitys.Models;

public class Articulo
{
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public string? Imagen { get; set; }
    public ICollection<ArticuloTienda> Tiendas { get; set; } = new List<ArticuloTienda>();
}
