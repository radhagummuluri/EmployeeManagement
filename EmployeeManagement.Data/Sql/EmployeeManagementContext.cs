using System;
using EmployeeManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data.Sql.Entities
{
    public class EmployeeManagementContext: DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<PayrollPreview> PayrollPreviews { get; set; }
        public DbSet<EmployeeAnnualDeduction> EmployeeAnnualDeductions { get; set; }

        public EmployeeManagementContext(DbContextOptions<EmployeeManagementContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dependent>()
            .HasOne(d => d.Employee)
            .WithMany(e => e.Dependents)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PayrollPreview>()
            .HasOne(d => d.EmployeeAnnualDeduction)
            .WithMany(e => e.PayrollPreviews)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
