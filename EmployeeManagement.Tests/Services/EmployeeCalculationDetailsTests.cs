using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services;
using EmployeeManagement.Services.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Services
{
    public class EmployeeCalculationDetailsTests
    {
        [Fact]
        public void VerifyEmployeeCalculationDetails_ForFull26Weeks_WithoutDiscount()
        {
            var employee = new Employee() {
                  Salary = 52000,
                  EmployeeId  = 1,
                  FullName = "Radha Gumm",
                  HireDate = Convert.ToDateTime("01/15/2018") 
            };

            var payPeriodRangesForCurrentYear = Convert.ToDateTime("12/31/2018").GetPayPeriodRanges(26);

            EmployeeCalculationDetails employeeCalculationDetails = new EmployeeCalculationDetails(employee, payPeriodRangesForCurrentYear);
            Assert.Equal(450, employeeCalculationDetails.DepDedDiscountVal);
            Assert.Equal(1.93m, employeeCalculationDetails.DepRateOfDedPerWorkDay);
            Assert.Equal(1.74m, employeeCalculationDetails.DepRateOfDedWithDiscPerWorkDay);
            Assert.Equal(1000, employeeCalculationDetails.EeDed);
            Assert.Equal(25, employeeCalculationDetails.EePayPerHr);
            Assert.Equal(3.85m, employeeCalculationDetails.EeRateOfDedPerWorkDay);
            Assert.False(employeeCalculationDetails.EmpHasDiscount);
            Assert.Equal(260, employeeCalculationDetails.EmpWorkDaysInCurrentYear);
            Assert.Equal(500, employeeCalculationDetails.TotalExpectedDepDecWODiscount);
            Assert.Equal(450, employeeCalculationDetails.TotalExpectedDepDecWithDiscount);
            Assert.Equal(1000, employeeCalculationDetails.TotalExpectedEEDed);
        }

        [Fact]
        public void VerifyEmployeeCalculationDetails_ForFull26Weeks_WithtEmployeeDiscount()
        {
            var employee = new Employee()
            {
                Salary = 52000,
                EmployeeId = 1,
                FullName = "Ari",
                HireDate = Convert.ToDateTime("12/31/2018")
            };

            var payPeriodRangesForCurrentYear = Convert.ToDateTime("12/31/2018").GetPayPeriodRanges(26);

            EmployeeCalculationDetails employeeCalculationDetails = new EmployeeCalculationDetails(employee, payPeriodRangesForCurrentYear);
            Assert.Equal(450, employeeCalculationDetails.DepDedDiscountVal);
            Assert.Equal(1.93m, employeeCalculationDetails.DepRateOfDedPerWorkDay);
            Assert.Equal(1.74m, employeeCalculationDetails.DepRateOfDedWithDiscPerWorkDay);
            Assert.Equal(900, employeeCalculationDetails.EeDed);
            Assert.Equal(25, employeeCalculationDetails.EePayPerHr);
            Assert.Equal(3.47m, employeeCalculationDetails.EeRateOfDedPerWorkDay);
            Assert.True(employeeCalculationDetails.EmpHasDiscount);
            Assert.Equal(260, employeeCalculationDetails.EmpWorkDaysInCurrentYear);
            Assert.Equal(500, employeeCalculationDetails.TotalExpectedDepDecWODiscount);
            Assert.Equal(450, employeeCalculationDetails.TotalExpectedDepDecWithDiscount);
            Assert.Equal(900, employeeCalculationDetails.TotalExpectedEEDed);
        }

        [Fact]
        public void VerifyEmployeeCalculationDetails_ForLessThan26Weeks_WithoutDiscount()
        {
            var employee = new Employee()
            {
                Salary = 52000,
                EmployeeId = 1,
                FullName = "Radha Gumm",
                HireDate = Convert.ToDateTime("01/24/2019")
            };

            var payPeriodRangesForCurrentYear = Convert.ToDateTime("12/31/2018").GetPayPeriodRanges(26);

            EmployeeCalculationDetails employeeCalculationDetails = new EmployeeCalculationDetails(employee, payPeriodRangesForCurrentYear);
            Assert.Equal(450, employeeCalculationDetails.DepDedDiscountVal);
            Assert.Equal(1.93m, employeeCalculationDetails.DepRateOfDedPerWorkDay);
            Assert.Equal(1.74m, employeeCalculationDetails.DepRateOfDedWithDiscPerWorkDay);
            Assert.Equal(1000, employeeCalculationDetails.EeDed);
            Assert.Equal(25, employeeCalculationDetails.EePayPerHr);
            Assert.Equal(3.85m, employeeCalculationDetails.EeRateOfDedPerWorkDay);
            Assert.False(employeeCalculationDetails.EmpHasDiscount);
            Assert.Equal(242, employeeCalculationDetails.EmpWorkDaysInCurrentYear);
            Assert.Equal(465, employeeCalculationDetails.TotalExpectedDepDecWODiscount);
            Assert.Equal(418.85m, employeeCalculationDetails.TotalExpectedDepDecWithDiscount);
            Assert.Equal(930.77m, employeeCalculationDetails.TotalExpectedEEDed);
        }

        [Fact]
        public void VerifyEmployeeCalculationDetails_ForLessThan26Weeks_WithDiscount()
        {
            var employee = new Employee()
            {
                Salary = 52000,
                EmployeeId = 1,
                FullName = "Amy",
                HireDate = Convert.ToDateTime("01/24/2019")
            };

            var payPeriodRangesForCurrentYear = Convert.ToDateTime("12/31/2018").GetPayPeriodRanges(26);

            EmployeeCalculationDetails employeeCalculationDetails = new EmployeeCalculationDetails(employee, payPeriodRangesForCurrentYear);
            Assert.Equal(450, employeeCalculationDetails.DepDedDiscountVal);
            Assert.Equal(1.93m, employeeCalculationDetails.DepRateOfDedPerWorkDay);
            Assert.Equal(1.74m, employeeCalculationDetails.DepRateOfDedWithDiscPerWorkDay);
            Assert.Equal(900, employeeCalculationDetails.EeDed);
            Assert.Equal(25, employeeCalculationDetails.EePayPerHr);
            Assert.Equal(3.47m, employeeCalculationDetails.EeRateOfDedPerWorkDay);
            Assert.True(employeeCalculationDetails.EmpHasDiscount);
            Assert.Equal(242, employeeCalculationDetails.EmpWorkDaysInCurrentYear);
            Assert.Equal(465, employeeCalculationDetails.TotalExpectedDepDecWODiscount);
            Assert.Equal(418.85m, employeeCalculationDetails.TotalExpectedDepDecWithDiscount);
            Assert.Equal(837.70m, employeeCalculationDetails.TotalExpectedEEDed);
        }
    }
}
