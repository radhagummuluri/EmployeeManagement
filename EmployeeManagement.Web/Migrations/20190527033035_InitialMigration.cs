using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeManagement.Web.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeAnnualDeductions",
                columns: table => new
                {
                    EmployeeAnnualDeductionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeId = table.Column<int>(nullable: false),
                    PayYear = table.Column<int>(nullable: false),
                    TotalExpectedEEDeduction = table.Column<decimal>(nullable: false),
                    TotalExpectedDepDeductionWODiscount = table.Column<decimal>(nullable: false),
                    TotalExpectedDepDeductionWithDiscount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAnnualDeductions", x => x.EmployeeAnnualDeductionId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Position = table.Column<string>(type: "varchar(100)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "date", nullable: false),
                    OfficeLocation = table.Column<string>(type: "varchar(100)", nullable: true),
                    CalculatePayrollPreview = table.Column<bool>(nullable: false),
                    Salary = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "PayrollPreviews",
                columns: table => new
                {
                    PayrollPreviewId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeId = table.Column<int>(nullable: false),
                    DependentId = table.Column<int>(nullable: false),
                    PayrollStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    PayRollEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    GrossSalaryForPayPeriod = table.Column<decimal>(nullable: false),
                    TotalDeductionForPayPeriod = table.Column<decimal>(nullable: false),
                    RateOfDeductionPerWorkDay = table.Column<decimal>(nullable: false),
                    NameBasedDiscount = table.Column<bool>(nullable: false),
                    EmployeeAnnualDeductionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPreviews", x => x.PayrollPreviewId);
                    table.ForeignKey(
                        name: "FK_PayrollPreviews_EmployeeAnnualDeductions_EmployeeAnnualDeductionId",
                        column: x => x.EmployeeAnnualDeductionId,
                        principalTable: "EmployeeAnnualDeductions",
                        principalColumn: "EmployeeAnnualDeductionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dependents",
                columns: table => new
                {
                    DependentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Relationship = table.Column<string>(type: "varchar(250)", nullable: false),
                    EmployeeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependents", x => x.DependentId);
                    table.ForeignKey(
                        name: "FK_Dependents_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_EmployeeId",
                table: "Dependents",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPreviews_EmployeeAnnualDeductionId",
                table: "PayrollPreviews",
                column: "EmployeeAnnualDeductionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dependents");

            migrationBuilder.DropTable(
                name: "PayrollPreviews");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeAnnualDeductions");
        }
    }
}
