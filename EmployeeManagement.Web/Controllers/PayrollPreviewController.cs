using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services;
using EmployeeManagement.Web.Helpers;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers
{
    public class PayrollPreviewController : Controller
    {
        private IEmployeeService _employeeService;
        private IPayrollPreviewService _payrollPreviewService;
        private IPayrollPreviewHelper _payrollPreviewHelper;

        public PayrollPreviewController(IEmployeeService employeeService, IPayrollPreviewService payrollPreviewService, IPayrollPreviewHelper payrollPreviewHelper)
        {
            _employeeService = employeeService;
            _payrollPreviewService = payrollPreviewService;
            _payrollPreviewHelper = payrollPreviewHelper;
        }

        public async Task<IActionResult> Index(int id)
        {
            var employee = await _employeeService.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            if (employee.CalculatePayrollPreview)
            {
                var firstPayPeriodStartWorkDayCurrentYr = Convert.ToDateTime("12/31/2018");
                await _payrollPreviewService.CalculatePayrollPreview(employee, firstPayPeriodStartWorkDayCurrentYr, DateTime.Now.Year);
                await _employeeService.SetCalculatePayrollPreview(id, false);
            }

            //Get the payroll previews for the employee order by pay start date
            var payPreviews = await _payrollPreviewService.GetPayrollPreviewForEmployee(employee.EmployeeId, DateTime.Now.Year);
            List<PayrollPreviewDetail> payrollPreviewDetails = _payrollPreviewHelper.CalculatePayrollPreviewDetails(employee, payPreviews);

            var model = new PayrollPreviewViewModel()
            {
                Year = DateTime.Now.Year,
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                PayrollPreviewDetails = payrollPreviewDetails
            };
            return View(model);
        }   
    }
}