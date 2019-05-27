using System;
using System.Threading.Tasks;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers
{
    public class PayrollPreviewController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IPayrollPreviewService _payrollPreviewService;

        public PayrollPreviewController(IEmployeeService employeeService, IPayrollPreviewService payrollPreviewService)
        {
            _employeeService = employeeService;
            _payrollPreviewService = payrollPreviewService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var employee = await _employeeService.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            if(employee.CalculatePayrollPreview)
            {
                var firstPayPeriodStartWorkDayCurrentYr = Convert.ToDateTime("12/31/2018");
                await _payrollPreviewService.CalculatePayrollPreview(employee, firstPayPeriodStartWorkDayCurrentYr, DateTime.Now.Year);
                await _employeeService.SetCalculatePayrollPreview(id, false);
            }

            return View(employee);
        }
    }
}