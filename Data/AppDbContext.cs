using Microsoft.EntityFrameworkCore;
using SportsSebastianVargas.Models;

namespace SportsSebastianVargas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<EspacioDeportivo> EspacioDeportivo { get; set; }
    public DbSet<Reserva> Reserva { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.DocumentoIdentidad).IsUnique();
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.CorreoElectronico).IsUnique();
        modelBuilder.Entity<EspacioDeportivo>()
            .HasIndex(e => e.Nombre).IsUnique();
    }
}