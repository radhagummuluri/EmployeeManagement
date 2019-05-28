using EmployeeManagement.Data.Sql.Entities;
using EmployeeManagement.Services;
using EmployeeManagement.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly EmployeeManagementContext _context;
        private readonly EmployeeService _sut;

        public EmployeeServiceTests()
        {
            _context = DatabaseHelpers.GetTesEmployeeManagementContext();
            _sut = new EmployeeService(_context);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SetCalculatePayrollPreview_GivenCalculateFlag_ShouldUpdateAccordingly(bool calculate)
        {
            var emp = _context.GivenAnEmployeeExists("Radha Gumm", Convert.ToDateTime("12/31/2018"));
            await _sut.SetCalculatePayrollPreview(emp.EmployeeId, calculate);

            var upEmp = _context.Employees.FirstOrDefault(e => e.EmployeeId == emp.EmployeeId);
            Assert.Equal(calculate, upEmp.CalculatePayrollPreview);
        }
    }
}
