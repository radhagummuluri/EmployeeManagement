using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services;

namespace EmployeeManagement.Web.Controllers
{
    public class DependentController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDependentService _dependentService;

        public DependentController(IEmployeeService employeeService, IDependentService dependentService)
        {
            _employeeService = employeeService;
            _dependentService = dependentService;
        }

        // GET: Dependent
        public async Task<IActionResult> Index(int id)
        {
            var employee = await _employeeService.GetEmployee(id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        
        // GET: Dependent/Create
        public async Task<IActionResult> AddOrEdit(int id, int? dependentId)
        {
            if(dependentId == null)
            {
                return View(new Dependent() {
                    EmployeeId = id
                });
            }
            else
            {
                Dependent dependent = await _dependentService.GetDependent(dependentId.Value);

                if (dependent == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(dependent);
                }
            }
        }

        // POST: Dependent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("DependentId,FullName,Relationship,EmployeeId")] Dependent dependent)
        {
            if (ModelState.IsValid)
            {
                if (dependent.DependentId == 0)
                    await _dependentService.CreateDependent(dependent);
                else
                    await _dependentService.UpdateDependent(dependent);

                await _employeeService.SetCalculatePayrollPreview(dependent.EmployeeId, true);

                return RedirectToAction(nameof(Index), new { id = dependent.EmployeeId});
            }
            return View(dependent);
        }

        // GET: Dependent/Delete/5
        public async Task<IActionResult> Delete(int id, int dependentId)
        {
            if (!await _dependentService.RemoveDependent(dependentId))
            {
                return NotFound();
            }

            await _employeeService.SetCalculatePayrollPreview(id, true);

            return RedirectToAction(nameof(Index), new { id });
        }
    }
}
