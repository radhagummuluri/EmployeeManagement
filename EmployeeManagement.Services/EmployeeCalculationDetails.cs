using EmployeeManagement.Data.Entities;
using EmployeeManagement.Services.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Services
{
    public class EmployeeCalculationDetails
    {
        public Employee Employee { get; set; }
        public ICollection<PayPeriod> PayPeriodRangesForCurrentYear { get; set; }

        //employee pay/hr = employee.salary/2080 (presently $25/hr as gross salary set to 52000/yr or $2000/pay period)
        //annual EE deduction = 1000, EE Name starts with A 10% disc = 900, 
        //annual DEP deduction = 500, Dep Name starts with A 10% disc = 450
        public decimal EePayPerHr => Convert.ToDecimal(Employee.Salary / Constants.TOTAL_ANNUAL_WORK_HOURS).CeilingWithPrecision(2);

        public bool EmpHasDiscount => Employee.FullName.Trim().StartsWith("a", StringComparison.OrdinalIgnoreCase);

        public decimal EeDed => EmpHasDiscount
                ? (Constants.EE_ANNUAL_DEDUCTION - Constants.EE_ANNUAL_DEDUCTION * Constants.NAME_BASED_DISCOUNT_PERCENT / 100)
                : Constants.EE_ANNUAL_DEDUCTION;

        public decimal DepDedDiscountVal => Convert.ToDecimal(Constants.DEP_ANNUAL_DEDUCTION - Constants.DEP_ANNUAL_DEDUCTION * Constants.NAME_BASED_DISCOUNT_PERCENT / 100).CeilingWithPrecision(2);

        //based on the hire date, calculate the employee's number of working days until last pay end date. max will be 260 days for current year.
        //Assuming that employees are hired for the whole year
        public int EmpWorkDaysInCurrentYear => Employee.HireDate.GetNumberOfWorkingDaysUntilDate(PayPeriodRangesForCurrentYear.Last().EndDate) > Constants.TOTAL_ANNUAL_WORK_DAYS
            ? Constants.TOTAL_ANNUAL_WORK_DAYS
            : Employee.HireDate.GetNumberOfWorkingDaysUntilDate(PayPeriodRangesForCurrentYear.Last().EndDate);

        //calculate the total expected EE deduction with or without discount x = (work days * eeDed)/260            
        public decimal TotalExpectedEEDed => Convert.ToDecimal((EmpWorkDaysInCurrentYear * EeDed) / Constants.TOTAL_ANNUAL_WORK_DAYS).CeilingWithPrecision(2);

        //calculate the rate of deduction for EE per day = totalExpectedEEDed / work days
        public decimal EeRateOfDedPerWorkDay => Convert.ToDecimal(TotalExpectedEEDed / EmpWorkDaysInCurrentYear).CeilingWithPrecision(2);

        //calculate the total expected Dep deduction without discount y = (work days * 500)/260 
        public decimal TotalExpectedDepDecWODiscount => Convert.ToDecimal(((double)EmpWorkDaysInCurrentYear * Constants.DEP_ANNUAL_DEDUCTION) / Constants.TOTAL_ANNUAL_WORK_DAYS).CeilingWithPrecision(2);

        //calculate the total expected Dep deduction with discount y = (work days * 450)/260
        public decimal TotalExpectedDepDecWithDiscount => Convert.ToDecimal((EmpWorkDaysInCurrentYear * DepDedDiscountVal) / Constants.TOTAL_ANNUAL_WORK_DAYS).CeilingWithPrecision(2);

        //calculate the rate of deduction for Dep per day DEPDed = y / work day
        public decimal DepRateOfDedPerWorkDay => Convert.ToDecimal(TotalExpectedDepDecWODiscount / EmpWorkDaysInCurrentYear).CeilingWithPrecision(2);

        //calculate the rate of discounted deduction for Dep per day DEPDed = y / work day
        public decimal DepRateOfDedWithDiscPerWorkDay => Convert.ToDecimal(TotalExpectedDepDecWithDiscount / EmpWorkDaysInCurrentYear).CeilingWithPrecision(2);

        public EmployeeCalculationDetails(Employee employee, ICollection<PayPeriod> payPeriodRangesForCurrentYear)
        {
            Employee = employee;
            PayPeriodRangesForCurrentYear = payPeriodRangesForCurrentYear;
        }
    }
}
