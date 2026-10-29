using EquiposReportes.Models;
using Microsoft.EntityFrameworkCore;

namespace EquiposReportes.Data;

// Contexto generado con Scaffold-DbContext a partir de la base existente.
// La cadena de conexión NO está en OnConfiguring: se registra por inyección de dependencias
// en Program.cs usando ConnectionStrings:ReportesDb de User Secrets.
public partial class EquiposDbContext : DbContext
{
    public EquiposDbContext(DbContextOptions<EquiposDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Equipo> Equipos { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasIndex(e => e.Nombre, "IX_Categorias_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasIndex(e => e.CategoriaId, "IX_Equipos_CategoriaId");

            entity.HasIndex(e => e.CodigoInventario, "IX_Equipos_CodigoInventario").IsUnique();

            entity.Property(e => e.CodigoInventario).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Categoria).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipos_Categorias_CategoriaId");
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasIndex(e => e.EquipoId, "IX_Prestamos_EquipoId");

            entity.HasIndex(e => e.UsuarioId, "IX_Prestamos_UsuarioId");

            entity.HasOne(d => d.Equipo).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_Equipos_EquipoId");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_Usuarios_UsuarioId");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(e => e.Username, "IX_Usuarios_Username").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Rol).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
