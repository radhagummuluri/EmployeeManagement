using System;
using System.Collections.Generic;

namespace EmployeeManagement.Web.Models
{
    public class PayrollPreviewViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public ICollection<PayrollPreviewDetail> PayrollPreviewDetails { get; set; }
    }

    public class PayrollPreviewDetail
    {
        public String PayStart { get; set; }
        public String PayEnd { get; set; }
        public string GrossSalaryForPayPeriod { get; set; }
        public string TotalDeductionForPayPeriod { get; set; }
        public string NetSalaryForPayPeriod { get; set; }
        public string YearToDateGrossSalary { get; set; }
        public string YearToDateNetSalary { get; set; }
        public string EmployeePayPerHour { get; set; }
        public string NumberOfWorkHoursForPayPeriod { get; set; }
        public ICollection<DeductionDetail> DeductionDetails { get; set; }
        
    }

    public class DeductionDetail
    {
        public string Name { get; set; }
        public bool IsEmployee { get; set; }
        public string TotalDeductionForPayPeriod { get; set; }
        public string YearToDateDeduction { get; set; }
        public string Relationship { get; set; }
    }
}
