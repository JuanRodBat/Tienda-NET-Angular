using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.Entitys.Models;

public class Carrito
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public ICollection<CarritoItem> Items { get; set; } = new List<CarritoItem>();
}
