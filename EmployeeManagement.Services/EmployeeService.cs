using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Task<IList<Employee>> GetEmployees();
        Task<Employee> GetEmployee(int employeeId);
        Task<int> CreateEmployee(Employee employee);
        Task<int> UpdateEmployee(Employee employee);
        Task<bool> RemoveEmployee(int employeeId);
        Task SetCalculatePayrollPreview(int employeeId, bool calculate);
    }
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeManagementContext _context;

        public EmployeeService(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<int> CreateEmployee(Employee employee)
        {
            _context.Add(employee);
            return await _context.SaveChangesAsync();
        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            return await _context.Employees
                .Include(e => e.Dependents)
                .FirstOrDefaultAsync(m => m.EmployeeId == employeeId);
        }

        public async Task<IList<Employee>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<bool> RemoveEmployee(int employeeId)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == employeeId);
            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> UpdateEmployee(Employee employee)
        {
            _context.Update(employee);
            return await _context.SaveChangesAsync();
        }

        public async Task SetCalculatePayrollPreview(int employeeId, bool calculate)
        {
            var employee = await GetEmployee(employeeId);
            if(employee !=null)
            {
                employee.CalculatePayrollPreview = calculate;
                await UpdateEmployee(employee);
            }
        }
    }
}
