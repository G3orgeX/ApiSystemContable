using Microsoft.EntityFrameworkCore;
using ApiSystemContable.Models;

namespace ApiSystemContable.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Add your DbSets here
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Esto mapea automáticamente snake_case de Postgres a PascalCase de C#
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<TipoMovimiento> TipoMovimientos { get; set; } = null!;
    public DbSet<CierreMes> CierreMeses { get; set; } = null!;
    public DbSet<Tarjeta> Tarjetas { get; set; } = null!;
    public DbSet<Consumo> Consumos { get; set; } = null!;
    public DbSet<ResumenTarjeta> ResumenTarjetas { get; set; } = null!;
    public DbSet<ResumenConsumo> ResumenConsumos { get; set; } = null!;
    public DbSet<DetalleCierreMes> DetalleCierreMeses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Usuario table
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("idusuario");
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure TipoMovimiento table
        modelBuilder.Entity<TipoMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdTipoMovimiento);
            entity.Property(e => e.IdTipoMovimiento).HasColumnName("idtipomovimiento");
            entity.Property(e => e.Naturaleza).HasColumnType("char(1)");
        });

        // Configure CierreMes table
        modelBuilder.Entity<CierreMes>(entity =>
        {
            entity.HasKey(e => e.IdCierreMes);
            entity.Property(e => e.IdCierreMes).HasColumnName("idcierremes");
            entity.HasIndex(e => new { e.Mes, e.Anio }).IsUnique();
            entity.Property(e => e.Saldo)
                  .HasComputedColumnSql("total_debe - total_haber", stored: true)
                  .ValueGeneratedOnAddOrUpdate();
        });

        // Configure Tarjeta table
        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.IdTarjeta);
            entity.Property(e => e.IdTarjeta).HasColumnName("idtarjeta");
        });

        // Configure Consumo table
        modelBuilder.Entity<Consumo>(entity =>
        {
            entity.HasKey(e => e.IdConsumo);
            entity.Property(e => e.IdConsumo).HasColumnName("idconsumo");
            entity.Property(e => e.IdTarjeta).HasColumnName("idtarjeta");
        });

        // Configure ResumenTarjeta table
        modelBuilder.Entity<ResumenTarjeta>(entity =>
        {
            entity.HasKey(e => e.IdResumen);
            entity.Property(e => e.IdResumen).HasColumnName("idresumen");
            entity.Property(e => e.IdTarjeta).HasColumnName("idtarjeta");
            entity.HasIndex(e => new { e.IdTarjeta, e.Mes, e.Anio }).IsUnique();
        });

        // Configure ResumenConsumo table
        modelBuilder.Entity<ResumenConsumo>(entity =>
        {
            entity.HasKey(e => e.IdResumenConsumo);
            entity.Property(e => e.IdResumenConsumo).HasColumnName("idresumenconsumo");
            entity.Property(e => e.IdResumen).HasColumnName("idresumen");
            entity.Property(e => e.IdConsumo).HasColumnName("idconsumo");
        });

        // Configure DetalleCierreMes table
        modelBuilder.Entity<DetalleCierreMes>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);
            entity.Property(e => e.IdDetalle).HasColumnName("iddetalle");
            entity.Property(e => e.IdCierreMes).HasColumnName("idcierremes");
            entity.Property(e => e.IdTipoMovimiento).HasColumnName("idtipomovimiento");
            entity.Property(e => e.IdResumenTarjeta).HasColumnName("idresumentarjeta");
        });
    }
}
