using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Entitys.Models;

public class Tienda
{
    public int Id { get; set; }
    public string Sucursal { get; set; } = "";
    public string? Direccion { get; set; }
    public ICollection<ArticuloTienda> Articulos { get; set; } = new List<ArticuloTienda>();
}
