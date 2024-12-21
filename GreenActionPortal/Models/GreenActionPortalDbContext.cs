using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GreenActionPortal.Models;

public partial class GreenActionPortalDbContext : DbContext
{
    public GreenActionPortalDbContext()
    {
    }

    public GreenActionPortalDbContext(DbContextOptions<GreenActionPortalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutUsDetail> AboutUsDetails { get; set; }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<GarbageCollectionDatum> GarbageCollectionData { get; set; }

    public virtual DbSet<GarbageCollectionSchedule> GarbageCollectionSchedules { get; set; }

    public virtual DbSet<GarbageType> GarbageTypes { get; set; }

    public virtual DbSet<Official> Officials { get; set; }

    public virtual DbSet<Population> Populations { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=JULES-IRWIN\\SQLEXPRESS;Database=GreenActionPortalDB;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AboutUsDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.History).HasColumnName("history");
            entity.Property(e => e.Mission).HasColumnName("mission");
            entity.Property(e => e.Vision).HasColumnName("vision");
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PhotoPath).HasColumnName("photoPath");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        modelBuilder.Entity<GarbageCollectionDatum>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Can).HasColumnName("can");
            entity.Property(e => e.Cartons).HasColumnName("cartons");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Day)
                .HasMaxLength(50)
                .HasColumnName("day");
            entity.Property(e => e.FirstTrip).HasColumnName("firstTrip");
            entity.Property(e => e.Plastics).HasColumnName("plastics");
            entity.Property(e => e.SecondTrip).HasColumnName("secondTrip");
        });

        modelBuilder.Entity<GarbageCollectionSchedule>(entity =>
        {
            entity.ToTable("GarbageCollectionSchedule");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DayOfTheWeek)
                .HasMaxLength(50)
                .HasColumnName("dayOfTheWeek");
            entity.Property(e => e.GarbageTypeId).HasColumnName("garbageType_id");

            entity.HasOne(d => d.GarbageType).WithMany(p => p.GarbageCollectionSchedules)
                .HasForeignKey(d => d.GarbageTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_GarbageCollectionSchedule_GarbageType");
        });

        modelBuilder.Entity<GarbageType>(entity =>
        {
            entity.ToTable("GarbageType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GargabeType)
                .HasMaxLength(50)
                .HasColumnName("gargabeType");
        });

        modelBuilder.Entity<Official>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Position).WithMany(p => p.Officials)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Officials_Positions");

            entity.HasOne(d => d.User).WithMany(p => p.Officials)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Officials_Users");
        });

        modelBuilder.Entity<Population>(entity =>
        {
            entity.ToTable("Population");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.PopulationCensus).HasColumnName("populationCensus");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PositionName)
                .HasMaxLength(50)
                .HasColumnName("positionName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .HasColumnName("position");
            entity.Property(e => e.ProfilePicPath).HasColumnName("profilePicPath");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
