using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CircularSeas.Infrastructure.DB.Entities;

#nullable disable

namespace CircularSeas.Infrastructure.DB.Context
{
    public partial class CircularSeasContext : DbContext
    {
        public CircularSeasContext()
        {
        }

        public CircularSeasContext(DbContextOptions<CircularSeasContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Filament> Filaments { get; set; }
        public virtual DbSet<FilamentCompatibility> FilamentCompatibilities { get; set; }
        public virtual DbSet<FilamentSetting> FilamentSettings { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Node> Nodes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Print> Prints { get; set; }
        public virtual DbSet<PrintCompatibility> PrintCompatibilities { get; set; }
        public virtual DbSet<PrintSetting> PrintSettings { get; set; }
        public virtual DbSet<Printer> Printers { get; set; }
        public virtual DbSet<PrinterSetting> PrinterSettings { get; set; }
        public virtual DbSet<PropMat> PropMats { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=192.168.0.11;Initial Catalog=CircularSeas;User ID=sa;Password=a123.456;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Filament>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.MaterialFKNavigation)
                    .WithMany(p => p.Filaments)
                    .HasForeignKey(d => d.MaterialFK)
                    .HasConstraintName("FK_Filaments_Material");
            });

            modelBuilder.Entity<FilamentCompatibility>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.FilamentFKNavigation)
                    .WithMany(p => p.FilamentCompatibilities)
                    .HasForeignKey(d => d.FilamentFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FilamentCompatibility_Filaments");

                entity.HasOne(d => d.PrintFKNavigation)
                    .WithMany(p => p.FilamentCompatibilities)
                    .HasForeignKey(d => d.PrintFK)
                    .HasConstraintName("FK_FilamentCompatibility_Prints");

                entity.HasOne(d => d.PrinterFKNavigation)
                    .WithMany(p => p.FilamentCompatibilities)
                    .HasForeignKey(d => d.PrinterFK)
                    .HasConstraintName("FK_FilamentCompatibility_Printers");
            });

            modelBuilder.Entity<FilamentSetting>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.FilamentFKNavigation)
                    .WithMany(p => p.FilamentSettings)
                    .HasForeignKey(d => d.FilamentFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FilamentSettings_Filaments");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.Description).HasDefaultValueSql("('Material description')");

                entity.Property(e => e.Name).HasDefaultValueSql("('Material Name')");
            });

            modelBuilder.Entity<Node>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.MaterialFKNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.MaterialFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Material");

                entity.HasOne(d => d.NodeFKNavigation)
                    .WithMany(p => p.OrderNodeFKNavigations)
                    .HasForeignKey(d => d.NodeFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Node");

                entity.HasOne(d => d.ProviderFKNavigation)
                    .WithMany(p => p.OrderProviderFKNavigations)
                    .HasForeignKey(d => d.ProviderFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Node1");
            });

            modelBuilder.Entity<Print>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<PrintCompatibility>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.PrintFKNavigation)
                    .WithMany(p => p.PrintCompatibilities)
                    .HasForeignKey(d => d.PrintFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrintCompatibility_Prints");

                entity.HasOne(d => d.PrinterFKNavigation)
                    .WithMany(p => p.PrintCompatibilities)
                    .HasForeignKey(d => d.PrinterFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrintCompatibility_Printers");
            });

            modelBuilder.Entity<PrintSetting>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.PrintFKNavigation)
                    .WithMany(p => p.PrintSettings)
                    .HasForeignKey(d => d.PrintFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrintSettings_Prints");
            });

            modelBuilder.Entity<Printer>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<PrinterSetting>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.PrinterFKNavigation)
                    .WithMany(p => p.PrinterSettings)
                    .HasForeignKey(d => d.PrinterFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrinterSettings_Printers");
            });

            modelBuilder.Entity<PropMat>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.MaterialFKNavigation)
                    .WithMany(p => p.PropMats)
                    .HasForeignKey(d => d.MaterialFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropMat_Material");

                entity.HasOne(d => d.PropertyFKNavigation)
                    .WithMany(p => p.PropMats)
                    .HasForeignKey(d => d.PropertyFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropMat_Properties");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.MaterialFKNavigation)
                    .WithMany(p => p.Stocks)
                    .HasForeignKey(d => d.MaterialFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stock_Material");

                entity.HasOne(d => d.NodeFKNavigation)
                    .WithMany(p => p.Stocks)
                    .HasForeignKey(d => d.NodeFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stock_Node");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
