using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services;
using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Controllers
{
    public class PayrollPreviewControllerTests
    {
        private Mock<IEmployeeService> _employeeServiceMock;
        private Mock<IPayrollPreviewService> _payrollPreviewServiceMock;
        private Mock<IPayrollPreviewHelper> _payrollPreviewHelperMock;
        private PayrollPreviewController _sut;

        public PayrollPreviewControllerTests()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _payrollPreviewServiceMock = new Mock<IPayrollPreviewService>();
            _payrollPreviewHelperMock = new Mock<IPayrollPreviewHelper>();

            _sut = new PayrollPreviewController(_employeeServiceMock.Object, _payrollPreviewServiceMock.Object, _payrollPreviewHelperMock.Object);
        }

        [Fact]
        public async Task Index_ShouldCallCalculatePayrollPreview_WhenSetToTrue()
        {
            var emp = new Employee()
            {
                EmployeeId = 1,
                FullName = "test",
                HireDate = DateTime.Now,
                Position = "pos",
                OfficeLocation = "ofl",
                CalculatePayrollPreview = true
            };

            _employeeServiceMock.Setup(x => x.GetEmployee(1))
                .ReturnsAsync(emp);

            await _sut.Index(1);
            _payrollPreviewServiceMock.Verify(service => service.CalculatePayrollPreview(It.Is<Employee>(g => g.EmployeeId == 1), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
            _employeeServiceMock.Verify(service => service.SetCalculatePayrollPreview(It.Is<int>(g => g == 1), It.Is<bool>(g => g == false)), Times.Once);

            _payrollPreviewServiceMock.Verify(service => service.GetPayrollPreviewForEmployee(It.Is<int>(g => g == 1), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Index_ShouldNeverCallCalculatePayrollPreview_WhenSetToFalse()
        {
            var emp = new Employee()
            {
                EmployeeId = 1,
                FullName = "test",
                HireDate = DateTime.Now,
                Position = "pos",
                OfficeLocation = "ofl",
                CalculatePayrollPreview = false
            };

            _employeeServiceMock.Setup(x => x.GetEmployee(1))
                .ReturnsAsync(emp);

            await _sut.Index(1);
            _payrollPreviewServiceMock.Verify(service => service.CalculatePayrollPreview(It.Is<Employee>(g => g.EmployeeId == 1), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never);
            _employeeServiceMock.Verify(service => service.SetCalculatePayrollPreview(It.Is<int>(g => g == 1), It.Is<bool>(g => g == false)), Times.Never);

            _payrollPreviewServiceMock.Verify(service => service.GetPayrollPreviewForEmployee(It.Is<int>(g => g == 1), It.IsAny<int>()), Times.Once);
        }
    }
}
