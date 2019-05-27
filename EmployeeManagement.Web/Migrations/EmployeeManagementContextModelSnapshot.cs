﻿// <auto-generated />
using System;
using EmployeeManagement.Data.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmployeeManagement.Web.Migrations
{
    [DbContext(typeof(EmployeeManagementContext))]
    partial class EmployeeManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EmployeeManagement.Data.Entities.Dependent", b =>
                {
                    b.Property<int>("DependentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EmployeeId");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Relationship")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.HasKey("DependentId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Dependents");
                });

            modelBuilder.Entity("EmployeeManagement.Data.Entities.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("CalculatePayrollPreview");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime>("HireDate")
                        .HasColumnType("date");

                    b.Property<string>("OfficeLocation")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Position")
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Salary");

                    b.HasKey("EmployeeId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("EmployeeManagement.Data.Entities.EmployeeAnnualDeduction", b =>
                {
                    b.Property<int>("EmployeeAnnualDeductionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EmployeeId");

                    b.Property<int>("PayYear");

                    b.Property<decimal>("TotalExpectedDepDeductionWODiscount");

                    b.Property<decimal>("TotalExpectedDepDeductionWithDiscount");

                    b.Property<decimal>("TotalExpectedEEDeduction");

                    b.HasKey("EmployeeAnnualDeductionId");

                    b.ToTable("EmployeeAnnualDeductions");
                });

            modelBuilder.Entity("EmployeeManagement.Data.Entities.PayrollPreview", b =>
                {
                    b.Property<int>("PayrollPreviewId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DependentId");

                    b.Property<int>("EmployeeAnnualDeductionId");

                    b.Property<int>("EmployeeId");

                    b.Property<decimal>("GrossSalaryForPayPeriod");

                    b.Property<bool>("NameBasedDiscount");

                    b.Property<DateTime>("PayRollEndDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("PayrollStartDate")
                        .HasColumnType("date");

                    b.Property<decimal>("RateOfDeductionPerWorkDay");

                    b.Property<decimal>("TotalDeductionForPayPeriod");

                    b.HasKey("PayrollPreviewId");

                    b.HasIndex("EmployeeAnnualDeductionId");

                    b.ToTable("PayrollPreviews");
                });

            modelBuilder.Entity("EmployeeManagement.Data.Entities.Dependent", b =>
                {
                    b.HasOne("EmployeeManagement.Data.Entities.Employee", "Employee")
                        .WithMany("Dependents")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EmployeeManagement.Data.Entities.PayrollPreview", b =>
                {
                    b.HasOne("EmployeeManagement.Data.Entities.EmployeeAnnualDeduction", "EmployeeAnnualDeduction")
                        .WithMany("PayrollPreviews")
                        .HasForeignKey("EmployeeAnnualDeductionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}