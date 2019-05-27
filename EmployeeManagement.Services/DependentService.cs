using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public interface IDependentService
    {
        Task<Dependent> GetDependent(int dependentId);
        Task<int> CreateDependent(Dependent dependent);
        Task<int> UpdateDependent(Dependent dependent);
        Task<bool> RemoveDependent(int dependentId);
    }
    public class DependentService : IDependentService
    {
        private readonly EmployeeManagementContext _context;

        public DependentService(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<int> CreateDependent(Dependent dependent)
        {
            _context.Add(dependent);
            return await _context.SaveChangesAsync();
        }

        public async Task<Dependent> GetDependent(int dependentId)
        {
           return  await _context.Dependents
                    .Include(d => d.Employee)
                    .Where(d => d.DependentId == dependentId)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveDependent(int dependentId)
        {
            var dependent = await _context.Dependents
                .FirstOrDefaultAsync(m => m.DependentId == dependentId);
            if (dependent == null)
            {
                return false;
            }
            _context.Dependents.Remove(dependent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> UpdateDependent(Dependent dependent)
        {
            _context.Update(dependent);
            return await _context.SaveChangesAsync();
        }
    }
}
