using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeManagement.Web.Migrations
{
    public partial class AddedPayRateToPayPreview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EmployeePayPerHour",
                table: "PayrollPreviews",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NumberOfWorkHoursForPayPeriod",
                table: "PayrollPreviews",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeePayPerHour",
                table: "PayrollPreviews");

            migrationBuilder.DropColumn(
                name: "NumberOfWorkHoursForPayPeriod",
                table: "PayrollPreviews");
        }
    }
}
