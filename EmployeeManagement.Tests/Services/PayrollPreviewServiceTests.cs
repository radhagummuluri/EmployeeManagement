using EmployeeManagement.Data.Sql.Entities;
using EmployeeManagement.Services;
using EmployeeManagement.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Services
{
    public class PayrollPreviewServiceTests
    {
        private readonly EmployeeManagementContext _context;
        private readonly PayrollPreviewService _sut;

        public PayrollPreviewServiceTests()
        {
            _context = DatabaseHelpers.GetTesEmployeeManagementContext();
            _sut = new PayrollPreviewService(_context);
        }

        [Fact]
        public async Task CalculatePayrollPreview_GivenEmployeeWithNoDependents_26Payments_VerifyPayRollPreviews()
        {
            var pastHireDate = Convert.ToDateTime("01/31/2018");
            var employee = _context.GivenAnEmployeeExists("Radha Gumm", pastHireDate);

            await _sut.CalculatePayrollPreview(employee, Convert.ToDateTime("12/31/2018"), 2019);

            var employeeAnnualDed = _context.EmployeeAnnualDeductions.Where(d => d.EmployeeId == employee.EmployeeId && d.PayYear == 2019).FirstOrDefault();

            Assert.NotNull(employeeAnnualDed);
            Assert.Equal(2019, employeeAnnualDed.PayYear);
            Assert.Equal(26, employeeAnnualDed.PayrollPreviews.Count());
            Assert.Equal(500, employeeAnnualDed.TotalExpectedDepDeductionWODiscount);
            Assert.Equal(450, employeeAnnualDed.TotalExpectedDepDeductionWithDiscount);
            Assert.Equal(1000, employeeAnnualDed.TotalExpectedEEDeduction);

            var payrollPreviews = await _sut.GetPayrollPreviewForEmployee(employee.EmployeeId, 2019);

            Assert.Equal(26, payrollPreviews.Count());
            Assert.Equal(Convert.ToDateTime("2018-12-31").Date, payrollPreviews.First().PayrollStartDate.Date);
            Assert.Equal(Convert.ToDateTime("2019-01-11").Date, payrollPreviews.First().PayRollEndDate.Date);
            Assert.Equal(2000, payrollPreviews.First().GrossSalaryForPayPeriod);
            Assert.Equal(38.50m, payrollPreviews.First().TotalDeductionForPayPeriod);
            Assert.Equal(3.85m, payrollPreviews.First().RateOfDeductionPerWorkDay);
            Assert.False(payrollPreviews.First().NameBasedDiscount);
            Assert.Equal(employeeAnnualDed.EmployeeAnnualDeductionId, payrollPreviews.First().EmployeeAnnualDeductionId);
            Assert.Equal(25, payrollPreviews.First().EmployeePayPerHour);
            Assert.Equal(80, payrollPreviews.First().NumberOfWorkHoursForPayPeriod);

            Assert.Equal(Convert.ToDateTime("2019-12-16").Date, payrollPreviews.Last().PayrollStartDate.Date);
            Assert.Equal(Convert.ToDateTime("2019-12-27").Date, payrollPreviews.Last().PayRollEndDate.Date);
            Assert.Equal(2000, payrollPreviews.Last().GrossSalaryForPayPeriod);
            Assert.Equal(37.50m, payrollPreviews.Last().TotalDeductionForPayPeriod);
            Assert.Equal(3.750m, payrollPreviews.Last().RateOfDeductionPerWorkDay);
            Assert.Equal(25, payrollPreviews.Last().EmployeePayPerHour);
            Assert.Equal(80, payrollPreviews.Last().NumberOfWorkHoursForPayPeriod);

            Assert.Equal(employee.Salary, payrollPreviews.Sum(x => x.GrossSalaryForPayPeriod));
            Assert.Equal(1000.00m, payrollPreviews.Sum(x => x.TotalDeductionForPayPeriod));
        }
        
        [Fact]
        public async Task CalculatePayrollPreview_GivenEmployeeWithNoDependents_NameStartsWithA_26Payments_VerifyPayRollPreviews()
        {
            var pastHireDate = Convert.ToDateTime("01/31/2018");
            var employee = _context.GivenAnEmployeeExists("Adha Gumm", pastHireDate);

            await _sut.CalculatePayrollPreview(employee, Convert.ToDateTime("12/31/2018"), 2019);

            var employeeAnnualDed = _context.EmployeeAnnualDeductions.Where(d => d.EmployeeId == employee.EmployeeId && d.PayYear == 2019).FirstOrDefault();

            Assert.NotNull(employeeAnnualDed);
            Assert.Equal(900, employeeAnnualDed.TotalExpectedEEDeduction);

            var payrollPreviews = await _sut.GetPayrollPreviewForEmployee(employee.EmployeeId, 2019);

            Assert.Equal(26, payrollPreviews.Count());
            Assert.Equal(2000, payrollPreviews.First().GrossSalaryForPayPeriod);
            Assert.Equal(34.70m, payrollPreviews.First().TotalDeductionForPayPeriod);
            Assert.Equal(3.47m, payrollPreviews.First().RateOfDeductionPerWorkDay);
            Assert.True(payrollPreviews.First().NameBasedDiscount);
            Assert.Equal(25, payrollPreviews.First().EmployeePayPerHour);
            Assert.Equal(80, payrollPreviews.First().NumberOfWorkHoursForPayPeriod);

            Assert.Equal(2000, payrollPreviews.Last().GrossSalaryForPayPeriod);
            Assert.Equal(32.50m, payrollPreviews.Last().TotalDeductionForPayPeriod);
            Assert.Equal(3.250m, payrollPreviews.Last().RateOfDeductionPerWorkDay);
            Assert.Equal(25, payrollPreviews.Last().EmployeePayPerHour);
            Assert.Equal(80, payrollPreviews.Last().NumberOfWorkHoursForPayPeriod);

            Assert.Equal(employee.Salary, payrollPreviews.Sum(x => x.GrossSalaryForPayPeriod));
            Assert.Equal(900.00m, payrollPreviews.Sum(x => x.TotalDeductionForPayPeriod));
        }

        [Fact]
        public async Task CalculatePayrollPreview_GivenEmployeeWithNoDependents_LessThan26Payments_VerifyPayRollPreviews()
        {
            var hireDate = Convert.ToDateTime("06/24/2019");
            var employee = _context.GivenAnEmployeeExists("Radha Gumm",hireDate);

            await _sut.CalculatePayrollPreview(employee, Convert.ToDateTime("12/31/2018"), 2019);

            var employeeAnnualDed = _context.EmployeeAnnualDeductions.Where(d => d.EmployeeId == employee.EmployeeId && d.PayYear == 2019).FirstOrDefault();

            Assert.NotNull(employeeAnnualDed);
            Assert.Equal(2019, employeeAnnualDed.PayYear);
            Assert.Equal(14, employeeAnnualDed.PayrollPreviews.Count());
            Assert.Equal(259.62m, employeeAnnualDed.TotalExpectedDepDeductionWODiscount);
            Assert.Equal(233.66m, employeeAnnualDed.TotalExpectedDepDeductionWithDiscount);
            Assert.Equal(519.24m, employeeAnnualDed.TotalExpectedEEDeduction);

            var payrollPreviews = await _sut.GetPayrollPreviewForEmployee(employee.EmployeeId, 2019);

            Assert.Equal(14, payrollPreviews.Count());
            Assert.Equal(Convert.ToDateTime("2019-06-24").Date, payrollPreviews.First().PayrollStartDate.Date);
            Assert.Equal(Convert.ToDateTime("2019-06-28").Date, payrollPreviews.First().PayRollEndDate.Date);
            Assert.Equal(1000, payrollPreviews.First().GrossSalaryForPayPeriod);
            Assert.Equal(19.25m, payrollPreviews.First().TotalDeductionForPayPeriod);
            Assert.Equal(3.850m, payrollPreviews.First().RateOfDeductionPerWorkDay);
            Assert.False(payrollPreviews.First().NameBasedDiscount);
            Assert.Equal(employeeAnnualDed.EmployeeAnnualDeductionId, payrollPreviews.First().EmployeeAnnualDeductionId);
            Assert.Equal(25, payrollPreviews.First().EmployeePayPerHour);
            Assert.Equal(40, payrollPreviews.First().NumberOfWorkHoursForPayPeriod);

            Assert.Equal(Convert.ToDateTime("2019-12-16").Date, payrollPreviews.Last().PayrollStartDate.Date);
            Assert.Equal(Convert.ToDateTime("2019-12-27").Date, payrollPreviews.Last().PayRollEndDate.Date);
            Assert.Equal(2000, payrollPreviews.Last().GrossSalaryForPayPeriod);
            Assert.Equal(37.99m, payrollPreviews.Last().TotalDeductionForPayPeriod);
            Assert.Equal(3.799m, payrollPreviews.Last().RateOfDeductionPerWorkDay);
            Assert.Equal(25, payrollPreviews.Last().EmployeePayPerHour);
            Assert.Equal(80, payrollPreviews.Last().NumberOfWorkHoursForPayPeriod);

            Assert.Equal(27000, payrollPreviews.Sum(x => x.GrossSalaryForPayPeriod));
            Assert.Equal(519.24m, payrollPreviews.Sum(x => x.TotalDeductionForPayPeriod));
        }

        [Fact]
        public async Task CalculatePayrollPreview_GivenEmployeeWithOneDependent_26Payments_VerifyPayRollPreviews()
        {
            var pastHireDate = Convert.ToDateTime("01/31/2018");
            var employee = _context.GivenAnEmployeeExists("Radha Gumm", pastHireDate, "Krishna");

            await _sut.CalculatePayrollPreview(employee, Convert.ToDateTime("12/31/2018"), 2019);

            var employeeAnnualDed = _context.EmployeeAnnualDeductions.Where(d => d.EmployeeId == employee.EmployeeId && d.PayYear == 2019).FirstOrDefault();

            var payrollPreviews = await _sut.GetPayrollPreviewForEmployee(employee.EmployeeId, 2019);

            Assert.Equal(52, payrollPreviews.Count());
            Assert.Equal(Convert.ToDateTime("2018-12-31").Date, payrollPreviews.First(x => x.DependentId > 0).PayrollStartDate.Date);
            Assert.Equal(Convert.ToDateTime("2019-01-11").Date, payrollPreviews.First(x => x.DependentId > 0).PayRollEndDate.Date);
            Assert.Equal(0, payrollPreviews.First(x => x.DependentId > 0).GrossSalaryForPayPeriod);
            Assert.Equal(19.3m, payrollPreviews.First(x => x.DependentId > 0).TotalDeductionForPayPeriod);
            Assert.Equal(1.93m, payrollPreviews.First(x => x.DependentId > 0).RateOfDeductionPerWorkDay);
            Assert.False(payrollPreviews.First(x => x.DependentId > 0).NameBasedDiscount);
            Assert.Equal(employeeAnnualDed.EmployeeAnnualDeductionId, payrollPreviews.First(x => x.DependentId > 0).EmployeeAnnualDeductionId);
            Assert.Equal(0, payrollPreviews.First(x => x.DependentId > 0).EmployeePayPerHour);
            Assert.Equal(0, payrollPreviews.First(x => x.DependentId > 0).NumberOfWorkHoursForPayPeriod);

            Assert.Equal(Convert.ToDateTime("2019-12-16").Date, payrollPreviews.Last(x => x.DependentId > 0).PayrollStartDate.Date);
            Assert.Equal(Convert.ToDateTime("2019-12-27").Date, payrollPreviews.Last(x => x.DependentId > 0).PayRollEndDate.Date);
            Assert.Equal(0, payrollPreviews.Last(x => x.DependentId > 0).GrossSalaryForPayPeriod);
            Assert.Equal(17.50m, payrollPreviews.Last(x => x.DependentId > 0).TotalDeductionForPayPeriod);
            Assert.Equal(1.750m, payrollPreviews.Last(x => x.DependentId > 0).RateOfDeductionPerWorkDay);
            Assert.Equal(0, payrollPreviews.Last(x => x.DependentId > 0).EmployeePayPerHour);
            Assert.Equal(0, payrollPreviews.Last(x => x.DependentId > 0).NumberOfWorkHoursForPayPeriod);

            Assert.Equal(500.00m, payrollPreviews.Where(x => x.DependentId > 0).Sum(x => x.TotalDeductionForPayPeriod));
        }

        [Fact]
        public async Task CalculatePayrollPreview_GivenEmployeeWithOneDependent_NameStartsWithA_VerifyPayRollPreviews()
        {
            var pastHireDate = Convert.ToDateTime("01/31/2018");
            var employee = _context.GivenAnEmployeeExists("Radha Gumm", pastHireDate, "Aeve");

            await _sut.CalculatePayrollPreview(employee, Convert.ToDateTime("12/31/2018"), 2019);

            var employeeAnnualDed = _context.EmployeeAnnualDeductions.Where(d => d.EmployeeId == employee.EmployeeId && d.PayYear == 2019).FirstOrDefault();

            var payrollPreviews = await _sut.GetPayrollPreviewForEmployee(employee.EmployeeId, 2019);

            Assert.Equal(52, payrollPreviews.Count());
            Assert.Equal(17.40m, payrollPreviews.First(x => x.DependentId > 0).TotalDeductionForPayPeriod);
            Assert.Equal(1.74m, payrollPreviews.First(x => x.DependentId > 0).RateOfDeductionPerWorkDay);
            Assert.True(payrollPreviews.First(x => x.DependentId > 0).NameBasedDiscount);

            Assert.Equal(15.0m, payrollPreviews.Last(x => x.DependentId > 0).TotalDeductionForPayPeriod);
            Assert.Equal(1.50m, payrollPreviews.Last(x => x.DependentId > 0).RateOfDeductionPerWorkDay);

            Assert.Equal(450.00m, payrollPreviews.Where(x => x.DependentId > 0).Sum(x => x.TotalDeductionForPayPeriod));
        }
    }
}
