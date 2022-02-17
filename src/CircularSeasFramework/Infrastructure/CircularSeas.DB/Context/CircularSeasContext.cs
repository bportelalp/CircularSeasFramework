using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CircularSeas.DB.Entities;

#nullable disable

namespace CircularSeas.DB.Context
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

        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<FeatureMat> FeatureMats { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Node> Nodes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Printer> Printers { get; set; }
        public virtual DbSet<PrinterProfile> PrinterProfiles { get; set; }
        public virtual DbSet<PropMat> PropMats { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<TestTwinCAT> TestTwinCATs { get; set; }

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

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<FeatureMat>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.FeatureFKNavigation)
                    .WithMany(p => p.FeatureMats)
                    .HasForeignKey(d => d.FeatureFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeatureMat_Feature");

                entity.HasOne(d => d.MaterialFKNavigation)
                    .WithMany(p => p.FeatureMats)
                    .HasForeignKey(d => d.MaterialFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeatureMat_Material");
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
                entity.HasOne(d => d.MaterialFKNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.MaterialFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Material");

                entity.HasOne(d => d.NodeFKNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.NodeFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Node");

                entity.HasOne(d => d.ProviderFKNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.ProviderFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Node1");
            });

            modelBuilder.Entity<Printer>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<PrinterProfile>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.HasOne(d => d.PrinterFKNavigation)
                    .WithMany(p => p.PrinterProfiles)
                    .HasForeignKey(d => d.PrinterFK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrinterProfile_Printer");
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
                    .HasConstraintName("FK_PropMat_Property");
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
