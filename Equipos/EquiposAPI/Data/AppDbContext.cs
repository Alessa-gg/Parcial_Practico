using EquiposAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EquiposAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Categoria>(e =>
        {
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Nombre).IsUnique();
            e.HasData(
                new Categoria { Id = 1, Nombre = "Laptops" },
                new Categoria { Id = 2, Nombre = "Proyectores" },
                new Categoria { Id = 3, Nombre = "Kits de IoT" },
                new Categoria { Id = 4, Nombre = "Arduino" });
        });

        mb.Entity<Equipo>(e =>
        {
            e.Property(x => x.CodigoInventario).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.CodigoInventario).IsUnique();
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(200);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.Categoria).WithMany(c => c.Equipos)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Usuario>(e =>
        {
            e.Property(x => x.Username).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Rol).IsRequired().HasMaxLength(20);
        });

        mb.Entity<Prestamo>(e =>
        {
            e.HasOne(x => x.Equipo).WithMany(q => q.Prestamos)
                .HasForeignKey(x => x.EquipoId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Usuario).WithMany(u => u.Prestamos)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}