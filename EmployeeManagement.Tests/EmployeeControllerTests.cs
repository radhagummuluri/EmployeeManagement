using System;
using Xunit;
using Moq;
using EmployeeManagement.Services;
using EmployeeManagement.Web.Controllers;
using System.Threading.Tasks;
using EmployeeManagement.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Tests
{
    public class EmployeeControllerTests
    {
        private Mock<IEmployeeService> _employeeService;
        private Mock<IPayrollPreviewService> _payrollPreviewService;
        private EmployeeController _sut;

        public EmployeeControllerTests()
        {
            _employeeService = new Mock<IEmployeeService>();
            _payrollPreviewService = new Mock<IPayrollPreviewService>();

            _sut = new EmployeeController(_employeeService.Object, _payrollPreviewService.Object);
        }

        [Fact]
        public async Task Index_ShouldCallGetEmployees_Once()
        {
            await _sut.Index();
            _employeeService.Verify(service => service.GetEmployees(),Times.Once);
        }

        [Fact]
        public async Task Details_ShouldCallGetEmployee_Once()
        {
            await _sut.Details(1);
            _employeeService.Verify(service => service.GetEmployee(It.Is<int>(g => g == 1)), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCallCreateEmployee_Once()
        {
            Employee emp = new Employee() {
                EmployeeId = 0,
                FullName = "ziggy stardust",
                HireDate = DateTime.Now,
                Position = "aerospace engg",
                OfficeLocation = "Mars"
            };
            await _sut.Create(emp);

            _employeeService.Verify(service => service.CreateEmployee(It.Is<Employee>(g => g.FullName == "ziggy stardust")), Times.Once);
        }

        [Fact]
        public async Task Edit_ShouldCallUpdateEmployee_Once()
        {
            Employee emp = new Employee()
            {
                EmployeeId = 1,
                FullName = "abc",
                HireDate = DateTime.Now,
                Position = "test",
                OfficeLocation = "chicago"
            };
            await _sut.Edit(emp);

            _employeeService.Verify(service => service.UpdateEmployee(It.Is<Employee>(g => g.EmployeeId == 1)), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldCallRemoveEmployee_Once()
        {
            await _sut.Delete(1);

            _employeeService.Verify(service => service.RemoveEmployee(It.Is<int>(g => g == 1)), Times.Once);
        }

        [Fact]
        public async Task Delete_ForAnUnknownEmployee_ShouldReturnNotFound()
        {
            var result = await _sut.Delete(1);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}