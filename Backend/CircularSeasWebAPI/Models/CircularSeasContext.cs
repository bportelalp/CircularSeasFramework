using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CircularSeasWebAPI.Models
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
        public virtual DbSet<PropMat> PropMats { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<VistaDatosMcdm> VistaDatosMcdms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=192.168.0.10;Database=CircularSeas;User Id=sa;Password=a123.456;");
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
                    .HasMaxLength(30)
                    .IsFixedLength(true);
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
                    .HasMaxLength(25)
                    .HasDefaultValueSql("('Material Name')")
                    .IsFixedLength(true);
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
                    .HasMaxLength(30)
                    .IsFixedLength(true);

                entity.Property(e => e.Units)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);
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

            modelBuilder.Entity<VistaDatosMcdm>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VistaDatosMCDM");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsFixedLength(true);

                entity.Property(e => e.Prop)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsFixedLength(true);

                entity.Property(e => e.Units)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
