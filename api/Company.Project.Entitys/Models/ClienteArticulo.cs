using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Entitys.Models;

public class ClienteArticulo
{
    public int ClienteId { get; set; }
    public int ArticuloId { get; set; }
    public DateTime Fecha { get; set; }
    public int Cantidad { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Articulo Articulo { get; set; } = null!;
}