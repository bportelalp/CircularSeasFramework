using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CircularSeasWebAPI.Entities
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
        public virtual DbSet<Printer> Printers { get; set; }
        public virtual DbSet<PrinterProfile> PrinterProfiles { get; set; }
        public virtual DbSet<PropMat> PropMats { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=192.168.0.10;Database=CircularSeas;User Id=sa;Password=a123.456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.ToTable("Feature");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<FeatureMat>(entity =>
            {
                entity.ToTable("FeatureMat");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdFeature).HasColumnName("idFeature");

                entity.Property(e => e.IdMaterial).HasColumnName("idMaterial");

                entity.HasOne(d => d.IdFeatureNavigation)
                    .WithMany(p => p.FeatureMats)
                    .HasForeignKey(d => d.IdFeature)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeatureMat_Feature");

                entity.HasOne(d => d.IdMaterialNavigation)
                    .WithMany(p => p.FeatureMats)
                    .HasForeignKey(d => d.IdMaterial)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeatureMat_Material");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("Material");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasDefaultValueSql("('Material description')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Material Name')");
            });

            modelBuilder.Entity<Printer>(entity =>
            {
                entity.ToTable("Printer");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<PrinterProfile>(entity =>
            {
                entity.ToTable("PrinterProfile");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PrinterId).HasColumnName("PrinterID");

                entity.Property(e => e.Profile)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Printer)
                    .WithMany(p => p.PrinterProfiles)
                    .HasForeignKey(d => d.PrinterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrinterProfile_Printer");
            });

            modelBuilder.Entity<PropMat>(entity =>
            {
                entity.ToTable("PropMat");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdMaterial).HasColumnName("idMaterial");

                entity.Property(e => e.IdProperty).HasColumnName("idProperty");

                entity.HasOne(d => d.IdMaterialNavigation)
                    .WithMany(p => p.PropMats)
                    .HasForeignKey(d => d.IdMaterial)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropMat_Material");

                entity.HasOne(d => d.IdPropertyNavigation)
                    .WithMany(p => p.PropMats)
                    .HasForeignKey(d => d.IdProperty)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropMat_Property");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Property");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Units)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("Stock");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdMaterial).HasColumnName("idMaterial");

                entity.Property(e => e.IdNode).HasColumnName("idNode");

                entity.HasOne(d => d.IdMaterialNavigation)
                    .WithMany(p => p.Stocks)
                    .HasForeignKey(d => d.IdMaterial)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stock_Material");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
