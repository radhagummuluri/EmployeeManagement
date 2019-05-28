using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services;
using EmployeeManagement.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Controllers
{
    public class DependentControllerTests
    {
        private Mock<IEmployeeService> _employeeServiceMock;
        private Mock<IDependentService> _dependentServiceMock;
        private DependentController _sut;

        public DependentControllerTests()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _dependentServiceMock = new Mock<IDependentService>();

            _sut = new DependentController(_employeeServiceMock.Object, _dependentServiceMock.Object);
        }

        [Fact]
        public async Task Index_ShouldCallGetEmployee_Once()
        {
            await _sut.Index(1);
            _employeeServiceMock.Verify(service => service.GetEmployee(It.Is<int>(g => g == 1)), Times.Once);
        }

        [Fact]
        public async Task AddOrEdit_GivenDependentId_ShouldCallGetDependent_Once()
        {
            await _sut.AddOrEdit(1, 2);
            _dependentServiceMock.Verify(service => service.GetDependent(It.Is<int>(g => g == 2)), Times.Once);
        }

        [Fact]
        public async Task AddOrEdit_GivenNoDependentId_PostShouldCreateDependent()
        {
            var dep = new Dependent() { FullName = "dep", EmployeeId = 1 };
            await _sut.AddOrEdit(dep);

            _dependentServiceMock.Verify(service => service.CreateDependent(It.Is<Dependent>(g => g.FullName == "dep")), Times.Once);
            _employeeServiceMock.Verify(service => service.SetCalculatePayrollPreview(It.Is<int>(g => g == 1), It.Is<bool>(g => g == true)), Times.Once);
        }

        [Fact]
        public async Task AddOrEdit_GivenNoDependentId_PostShouldUpdateDependent()
        {
            var dep = new Dependent() { FullName = "dep", EmployeeId = 1 , DependentId = 2};
            await _sut.AddOrEdit(dep);

            _dependentServiceMock.Verify(service => service.UpdateDependent(It.Is<Dependent>(g => g.FullName == "dep")), Times.Once);
            _employeeServiceMock.Verify(service => service.SetCalculatePayrollPreview(It.Is<int>(g => g == 1), It.Is<bool>(g => g == true)), Times.Once);
        }

        [Fact]
        public async Task DeleteknownDependent_ShouldCallSetCalculatePayrollPreview_Once()
        {
            _dependentServiceMock.Setup(x => x.RemoveDependent(2))
                .ReturnsAsync(true);

            await _sut.Delete(1,2);
            _employeeServiceMock.Verify(service => service.SetCalculatePayrollPreview(It.Is<int>(g => g == 1), It.Is<bool>(g => g == true)), Times.Once);
        }

        [Fact]
        public async Task DeleteUnknownDependent_ShouldReturnNotFound()
        {
            var result = await _sut.Delete(1, 2);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
