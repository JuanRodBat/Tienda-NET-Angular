using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Company.Project.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=DESKTOP-3IIL7EM\\SQLEXPRESS01;Database=TiendaDb;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new AppDbContext(opts);
    }
}
