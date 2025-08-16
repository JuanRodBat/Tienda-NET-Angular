using Company.Project.Entitys.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Project.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Tienda> Tiendas => Set<Tienda>();
    public DbSet<Articulo> Articulos => Set<Articulo>();
    public DbSet<ArticuloTienda> ArticuloTiendas => Set<ArticuloTienda>();
    public DbSet<ClienteArticulo> ClienteArticulos => Set<ClienteArticulo>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();

    // Vista
    public DbSet<CatalogoPorTiendaView> CatalogoPorTienda => Set<CatalogoPorTiendaView>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Cliente
        mb.Entity<Cliente>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(150);
            e.Property(x => x.Nombre).HasMaxLength(100);
            e.Property(x => x.Apellidos).HasMaxLength(150);
            e.Property(x => x.Direccion).HasMaxLength(250);
            e.Property(x => x.PasswordHash).HasMaxLength(200);
        });

        // Tienda
        mb.Entity<Tienda>(e =>
        {
            e.Property(x => x.Sucursal).HasMaxLength(120);
            e.Property(x => x.Direccion).HasMaxLength(250);
        });

        // Articulo
        mb.Entity<Articulo>(e =>
        {
            e.HasIndex(x => x.Codigo).IsUnique();
            e.Property(x => x.Codigo).HasMaxLength(50);
            e.Property(x => x.Descripcion).HasMaxLength(200);
            e.Property(x => x.Imagen).HasMaxLength(512);
            e.Property(x => x.Precio).HasColumnType("decimal(12,2)");
        });

        // ArticuloTienda
        mb.Entity<ArticuloTienda>(e =>
        {
            e.ToTable("ArticuloTienda");
            e.HasKey(x => new { x.ArticuloId, x.TiendaId });
            e.HasOne(x => x.Articulo).WithMany(a => a.Tiendas).HasForeignKey(x => x.ArticuloId);
            e.HasOne(x => x.Tienda).WithMany(t => t.Articulos).HasForeignKey(x => x.TiendaId);
            e.HasIndex(x => x.TiendaId).HasDatabaseName("IX_AT_Tienda");
            e.HasIndex(x => x.ArticuloId).HasDatabaseName("IX_AT_Articulo");
        });

        // ClienteArticulo
        mb.Entity<ClienteArticulo>(e =>
        {
            e.ToTable("ClienteArticulo");
            e.HasKey(x => new { x.ClienteId, x.ArticuloId, x.Fecha });
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId);
            e.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloId);
            e.HasIndex(x => new { x.ClienteId, x.Fecha }).HasDatabaseName("IX_CA_Cliente_Fecha");
        });

        // Carrito
        mb.Entity<Carrito>(e =>
        {
            e.HasIndex(x => x.ClienteId).IsUnique();
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId);
        });

        // CarritoItem
        mb.Entity<CarritoItem>(e =>
        {
            e.HasOne(x => x.Carrito).WithMany(c => c.Items).HasForeignKey(x => x.CarritoId);
            e.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloId);
            e.HasOne(x => x.Tienda).WithMany().HasForeignKey(x => x.TiendaId);
            e.HasIndex(x => new { x.CarritoId, x.ArticuloId, x.TiendaId }).IsUnique().HasDatabaseName("UQ_CI");
        });

        // Vista READ-ONLY
        mb.Entity<CatalogoPorTiendaView>(e =>
        {
            e.HasNoKey();
            e.ToView("vwCatalogoPorTienda");
            e.Property(x => x.Sucursal).HasMaxLength(120);
            e.Property(x => x.Codigo).HasMaxLength(50);
            e.Property(x => x.Descripcion).HasMaxLength(200);
            e.Property(x => x.Imagen).HasMaxLength(512);
            e.Property(x => x.Precio).HasColumnType("decimal(12,2)");
        });
    }
}
