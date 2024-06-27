using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SemestralProject.Models;

namespace SemestralProject.Data;

public partial class ProjectContext : DbContext
{
    public ProjectContext()
    {
    }

    public ProjectContext(DbContextOptions<ProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientDiscount> ClientDiscounts { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<SingleSale> SingleSales { get; set; }

    public virtual DbSet<Software> Softwares { get; set; }

    public virtual DbSet<SubscriptionSale> SubscriptionSales { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Default");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("Client_pk");

            entity.ToTable("Client");

            entity.Property(e => e.Adress).HasMaxLength(300);
            entity.Property(e => e.Email).HasMaxLength(300);
            entity.Property(e => e.IsDeleted).HasMaxLength(1);
            entity.Property(e => e.PhoneNumber).HasMaxLength(100);

            entity.HasOne(d => d.IdDiscountNavigation).WithMany(p => p.Clients)
                .HasForeignKey(d => d.IdDiscount)
                .HasConstraintName("Client_ClientDiscount");
        });

        modelBuilder.Entity<ClientDiscount>(entity =>
        {
            entity.HasKey(e => e.IdDiscount).HasName("ClientDiscount_pk");

            entity.ToTable("ClientDiscount");

            entity.Property(e => e.Value).HasColumnType("decimal(5, 4)");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("Company_pk");

            entity.ToTable("Company");

            entity.Property(e => e.IdClient).ValueGeneratedNever();
            entity.Property(e => e.Krs)
                .HasMaxLength(100)
                .HasColumnName("KRS");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.IdClientNavigation).WithOne(p => p.Company)
                .HasForeignKey<Company>(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Company_Client");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.IdDiscount).HasName("Discount_pk");

            entity.ToTable("Discount");

            entity.Property(e => e.DateFrom).HasColumnType("datetime");
            entity.Property(e => e.DateTo).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(1);
            entity.Property(e => e.Value).HasColumnType("decimal(5, 4)");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.IdPayment).HasName("Payment_pk");

            entity.ToTable("Payment");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Value).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdSaleNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdSale)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Single_Sale");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("Person_pk");

            entity.ToTable("Person");

            entity.Property(e => e.IdClient).ValueGeneratedNever();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Pesel)
                .HasMaxLength(20)
                .HasColumnName("PESEL");

            entity.HasOne(d => d.IdClientNavigation).WithOne(p => p.Person)
                .HasForeignKey<Person>(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Person_Client");
        });

        modelBuilder.Entity<SingleSale>(entity =>
        {
            entity.HasKey(e => e.IdSale).HasName("Single_Sale_pk");

            entity.ToTable("Single_Sale");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.EndOfSoftware).HasColumnType("datetime");
            entity.Property(e => e.ExpireDate).HasColumnType("datetime");
            entity.Property(e => e.IsSigned).HasMaxLength(1);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SoftwareVersion).HasMaxLength(300);
            entity.Property(e => e.UpdatesInfo).HasMaxLength(300);

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.SingleSales)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Sale_Client");

            entity.HasOne(d => d.IdSoftwareNavigation).WithMany(p => p.SingleSales)
                .HasForeignKey(d => d.IdSoftware)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Sale_Software");
        });

        modelBuilder.Entity<Software>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Software_pk");

            entity.ToTable("Software");

            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(300);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SoftwareVersion).HasMaxLength(300);

            entity.HasMany(d => d.IdDiscounts).WithMany(p => p.IdSoftwares)
                .UsingEntity<Dictionary<string, object>>(
                    "SoftwareDiscount",
                    r => r.HasOne<Discount>().WithMany()
                        .HasForeignKey("IdDiscount")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("Table_12_Discount"),
                    l => l.HasOne<Software>().WithMany()
                        .HasForeignKey("IdSoftware")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("Table_12_Software"),
                    j =>
                    {
                        j.HasKey("IdSoftware", "IdDiscount").HasName("Software_Discount_pk");
                        j.ToTable("Software_Discount");
                    });
        });

        modelBuilder.Entity<SubscriptionSale>(entity =>
        {
            entity.HasKey(e => e.IdSubSale).HasName("Subscription_Sale_pk");

            entity.ToTable("Subscription_Sale");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.SubscriptionSales)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Subscription_Client");

            entity.HasOne(d => d.IdSoftwareNavigation).WithMany(p => p.SubscriptionSales)
                .HasForeignKey(d => d.IdSoftware)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Subscription_Software");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("Users_pk");

            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(2000);
            entity.Property(e => e.RefreshToken).HasMaxLength(2000);
            entity.Property(e => e.Salt).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
