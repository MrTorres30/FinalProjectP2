using GastosPersonales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuración de las relaciones y restricciones de las entidades

            //Usuarios
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario> ()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(125);
            //Categorias
            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Descripcion)
                .HasMaxLength(250);

            modelBuilder.Entity<Categoria>()
                .Property(c => c.EsActivo)
                .HasDefaultValue(true);

            //Gastos

            modelBuilder.Entity<Gasto>()
                .Property(g => g.Monto)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Gasto>()
               .Property(g => g.Descripcion)
               .HasMaxLength(250);

            //Metodos de pago

            modelBuilder.Entity<MetodoPago>()
                .Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<MetodoPago>()
                .Property(m => m.Icono)
                .HasMaxLength(50);

            modelBuilder.Entity<MetodoPago>()
               .Property(m => m.EsActivo)
               .HasDefaultValue(true);

            //PRESUPPUESTOS

            modelBuilder.Entity<Presupuesto>()
                .Property(p => p.MontoLimite)
                .HasColumnType("decimal(18,2)");

            // 2. CONFIGURACIÓN DE RELACIONES Y EVITACIÓN DE CICLOS (ON DELETE RESTRICT)

            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Usuario)
                .WithMany(u => u.Gastos)
                .HasForeignKey(g => g.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.MetodoPago)
                .WithMany()
                .HasForeignKey(g => g.MetodoPagoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Presupuesto>()
                .HasOne(p => p.Usuario) 
                 .WithMany(u => u.Presupuestos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Presupuesto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}