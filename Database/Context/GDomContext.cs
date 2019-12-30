using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Database.Entity;

namespace Database.Context
{
    public partial class GDomContext : DbContext
    {
        public GDomContext()
        {
        }

        public GDomContext(DbContextOptions<GDomContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Facilities> Facilities { get; set; }
        public virtual DbSet<FacilityTypes> FacilityTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(local); Database=GlobalDomination;integrated security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Facilities>(entity =>
            {
                entity.HasKey(e => e.FacilityId);

                entity.Property(e => e.FacilityId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ActivationDate).HasColumnType("datetime");

                entity.Property(e => e.AddressLine1).HasMaxLength(60);

                entity.Property(e => e.AddressLine2).HasMaxLength(60);

                entity.Property(e => e.City).HasMaxLength(40);

                entity.Property(e => e.County)
                    .HasMaxLength(40)
                    .IsFixedLength();

                entity.Property(e => e.DeactivationDate).HasColumnType("datetime");

                entity.Property(e => e.Directions)
                    .HasMaxLength(250)
                    .IsFixedLength();

                entity.Property(e => e.LastModifiedUtc)
                    .HasColumnName("LastModifiedUTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PostalCode).HasMaxLength(10);

                entity.Property(e => e.State).HasMaxLength(40);

                entity.Property(e => e.Telephone).HasMaxLength(10);
            });

            modelBuilder.Entity<FacilityTypes>(entity =>
            {
                entity.HasKey(e => e.FacilityTypeId);

                entity.Property(e => e.FacilityTypeId).ValueGeneratedNever();

                entity.Property(e => e.ActivationDate).HasColumnType("datetime");

                entity.Property(e => e.DeactivationDate).HasColumnType("datetime");

                entity.Property(e => e.LastModifiedUtc).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
