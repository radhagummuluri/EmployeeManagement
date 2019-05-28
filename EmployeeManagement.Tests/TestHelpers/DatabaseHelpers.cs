using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Tests.TestHelpers
{
    public static class DatabaseHelpers
    {
        public static EmployeeManagementContext GetTesEmployeeManagementContext()
        {
            var connection = Guid.NewGuid().ToString() + DateTime.Now.Ticks;
            var options = new DbContextOptionsBuilder<EmployeeManagementContext>()
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInMemoryDatabase(connection);

            var db = new EmployeeManagementContext(options.Options);
            return db;
        }

        public static Employee GivenAnEmployeeExists(this EmployeeManagementContext db, String empName, DateTime hireDate, string dependentName = null)
        {
            var employee = new Employee()
            {
                Salary = 52000,
                FullName = empName,
                HireDate = hireDate,
                OfficeLocation = "testOffice",
                Position = "test",
                Dependents = dependentName == null ? new List<Dependent>() : new List<Dependent>() { new Dependent() { FullName = dependentName, Relationship ="Son" } }
            };

            db.Employees.Add(employee);
            db.SaveChanges();

            return employee;
        }
    }
}
