using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services;
using EmployeeManagement.Web.Models;
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

            if (employee.CalculatePayrollPreview)
            {
                var firstPayPeriodStartWorkDayCurrentYr = Convert.ToDateTime("12/31/2018");
                await _payrollPreviewService.CalculatePayrollPreview(employee, firstPayPeriodStartWorkDayCurrentYr, DateTime.Now.Year);
                await _employeeService.SetCalculatePayrollPreview(id, false);
            }

            //Get the payroll previews for the employee order by pay start date
            var payPreviews = await _payrollPreviewService.GetPayrollPreviewForEmployee(employee.EmployeeId, DateTime.Now.Year);
            List<PayrollPreviewDetail> payrollPreviewDetails = CalculatePayrollPreviewDetails(employee, payPreviews);

            var model = new PayrollPreviewViewModel()
            {
                Year = DateTime.Now.Year,
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                PayrollPreviewDetails = payrollPreviewDetails
            };
            return View(model);
        }

        private List<PayrollPreviewDetail> CalculatePayrollPreviewDetails(Employee employee, ICollection<PayrollPreview> payPreviews)
        {
            return payPreviews.OrderBy(pp => pp.PayrollStartDate).GroupBy(
                            p => p.PayrollStartDate,
                            (key, g) =>
                                new PayrollPreviewDetail()
                                {
                                    PayStart = g.First().PayrollStartDate.ToShortDateString(),
                                    PayEnd = g.First().PayRollEndDate.ToShortDateString(),
                                    EmployeePayPerHour = $"{g.First(e => e.DependentId == 0).EmployeePayPerHour:C2}",
                                    NumberOfWorkHoursForPayPeriod = $"{g.First(e => e.DependentId == 0).NumberOfWorkHoursForPayPeriod}",
                                    GrossSalaryForPayPeriod = $"{g.Sum(x => x.GrossSalaryForPayPeriod):C2}",
                                    TotalDeductionForPayPeriod = $"{g.Sum(x => x.TotalDeductionForPayPeriod):C2}",
                                    NetSalaryForPayPeriod = $"{(g.Sum(x => x.GrossSalaryForPayPeriod) - g.Sum(x => x.TotalDeductionForPayPeriod)):C2}",
                                    YearToDateGrossSalary = $"{payPreviews.Where(pay => pay.PayRollEndDate <= g.First().PayRollEndDate).Sum(pay => pay.GrossSalaryForPayPeriod):C2}",
                                    YearToDateNetSalary = $"{payPreviews.Where(pay => pay.PayRollEndDate <= g.First().PayRollEndDate).Sum(pay => pay.GrossSalaryForPayPeriod) - payPreviews.Where(pay => pay.PayRollEndDate <= g.First().PayRollEndDate).Sum(pay => pay.TotalDeductionForPayPeriod):C2}",

                                    DeductionDetails = g.OrderBy(k => k.DependentId).Select(pp => new DeductionDetail()
                                    {
                                        IsEmployee = (pp.DependentId == 0),
                                        Name = (pp.DependentId == 0) ? employee.FullName : employee.Dependents.First(dep => dep.DependentId == pp.DependentId).FullName,
                                        TotalDeductionForPayPeriod = $"{pp.TotalDeductionForPayPeriod:C2}",
                                        Relationship = (pp.DependentId == 0) ? "Employee" : employee.Dependents.First(dep => dep.DependentId == pp.DependentId).Relationship,
                                        YearToDateDeduction = $"{payPreviews.Where(p => p.DependentId == pp.DependentId && p.PayRollEndDate <= pp.PayRollEndDate).Sum(p => p.TotalDeductionForPayPeriod):C2}"
                                    }).ToList()
                                }
                        ).ToList();
        }
    }
}