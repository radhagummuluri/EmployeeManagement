using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services;

namespace EmployeeManagement.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IPayrollPreviewService _payrollPreviewService;

        public EmployeeController(IEmployeeService employeeService, IPayrollPreviewService payrollPreviewService)
        {
            _employeeService = employeeService;
            _payrollPreviewService = payrollPreviewService;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            return View(await _employeeService.GetEmployees());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View(new Employee() {
                HireDate = Convert.ToDateTime("12/31/2018"),
                Salary = 52000
            });
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FullName,Position,HireDate,OfficeLocation,Salary")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.CalculatePayrollPreview = true;
                await _employeeService.CreateEmployee(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employee/Edit
        public async Task<IActionResult> Edit(int id)
        {
            Employee employee = await _employeeService.GetEmployee(id);

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("EmployeeId,FullName,Position,HireDate,OfficeLocation,Salary")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.CalculatePayrollPreview = true;
                await _employeeService.UpdateEmployee(employee);
                return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
            }
            return View(employee);
        }
        
        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _employeeService.RemoveEmployee(id))
            {
                return NotFound();
            }

            await _payrollPreviewService.DeleteDeductionsAndPreview(id);

            return RedirectToAction(nameof(Index));
        }       
    }
}