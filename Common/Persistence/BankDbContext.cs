using Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Persistence
{

    public class BankDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Manager> Managers { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;

        public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
        public DbSet<EmployeeBranch> EmployeeBranches { get; set; } = null!;
        public DbSet<ManagerBranch> ManagerBranches { get; set; } = null!;
        public DbSet<EmployeeAccountPermission> EmployeeAccountPermissions { get; set; } = null!;

        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"
                        Data Source=DESKTOP-4MG3GT1\SQLEXPRESS;
                        Initial Catalog=BankDb;
                        Integrated Security=True;
                        Encrypt=True;
                        TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Customer

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            #endregion

            #region Employee

            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeId);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            #endregion

            #region Manager

            modelBuilder.Entity<Manager>()
                .HasKey(m => m.ManagerId);

            modelBuilder.Entity<Manager>()
                .HasIndex(m => m.Email)
                .IsUnique();

            modelBuilder.Entity<Manager>()
                .HasData(new Manager
                {
                    ManagerId = 1,
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "stivanp3@gmail.com",
                    Password = "admin123",
                    Address = "Head Office"
                });

            #endregion

            #region Branch

            modelBuilder.Entity<Branch>()
                .HasKey(b => b.BranchId);

            #endregion

            #region Account

            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Iban)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Accounts)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region CustomerAccount

            modelBuilder.Entity<CustomerAccount>()
                .HasKey(ca => new { ca.CustomerId, ca.AccountId });

            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Customer)
                .WithMany(c => c.CustomerAccounts)
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Account)
                .WithMany(a => a.CustomerAccounts)
                .HasForeignKey(ca => ca.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region EmployeeBranch

            modelBuilder.Entity<EmployeeBranch>()
                .HasKey(eb => new { eb.EmployeeId, eb.BranchId });

            modelBuilder.Entity<EmployeeBranch>()
                .HasOne(eb => eb.Employee)
                .WithMany(e => e.EmployeeBranches)
                .HasForeignKey(eb => eb.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeBranch>()
                .HasOne(eb => eb.Branch)
                .WithMany(b => b.EmployeeBranches)
                .HasForeignKey(eb => eb.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region ManagerBranch

            modelBuilder.Entity<ManagerBranch>()
                .HasKey(mb => new { mb.ManagerId, mb.BranchId });

            modelBuilder.Entity<ManagerBranch>()
                .HasOne(mb => mb.Manager)
                .WithMany(m => m.ManagerBranches)
                .HasForeignKey(mb => mb.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManagerBranch>()
                .HasOne(mb => mb.Branch)
                .WithMany(b => b.ManagerBranches)
                .HasForeignKey(mb => mb.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region EmployeeAccountPermission

            modelBuilder.Entity<EmployeeAccountPermission>()
                .HasKey(eap => new { eap.EmployeeId, eap.AccountId });

            modelBuilder.Entity<EmployeeAccountPermission>()
                .HasOne(eap => eap.Employee)
                .WithMany(e => e.EmployeeAccountPermissions)
                .HasForeignKey(eap => eap.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeAccountPermission>()
                .HasOne(eap => eap.Account)
                .WithMany(a => a.EmployeeAccountPermissions)
                .HasForeignKey(eap => eap.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}

