using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Sql.Entities;
using EmployeeManagement.Data.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public interface IPayrollPreviewService
    {
        Task CalculatePayrollPreview(Employee employee, DateTime firstPayPeriodStartWorkDayCurrentYr, int currentYear);
        Task DeleteDeductionsAndPreview(int employeeId);
    }
    public class PayrollPreviewService : IPayrollPreviewService
    {
        
        private EmployeeManagementContext _context;

        public PayrollPreviewService(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task CalculatePayrollPreview(Employee employee, DateTime firstPayPeriodStartWorkDayCurrentYr, int currentYear)
        {
            await DeleteDeductionsAndPreview(employee.EmployeeId);

            var payrollPreviews = new List<PayrollPreview>();

            //employee pay/hr = employee.salary/2080 (presently $25/hr as gross salary set to 52000/yr or $2000/pay period)
            //annual EE deduction = 1000, EE Name starts with A 10% disc = 900, 
            //annual DEP deduction = 500, Dep Name starts with A 10% disc = 450
            var eePayPerHr = Convert.ToDecimal(employee.Salary / Constants.TOTAL_ANNUAL_WORK_HOURS).CeilingWithPrecision(2);

            var empHasDiscount = employee.FullName.Trim().StartsWith("a", StringComparison.OrdinalIgnoreCase);
            var eeDed = empHasDiscount
                ? (Constants.EE_ANNUAL_DEDUCTION - Constants.EE_ANNUAL_DEDUCTION * Constants.NAME_BASED_DISCOUNT_PERCENT/100)
                : Constants.EE_ANNUAL_DEDUCTION;

            var depDedDiscountVal = Convert.ToDecimal(Constants.DEP_ANNUAL_DEDUCTION - Constants.DEP_ANNUAL_DEDUCTION * Constants.NAME_BASED_DISCOUNT_PERCENT / 100).CeilingWithPrecision(2);

            //get 26 payperiodranges since first work day for the current year
            var payPeriodRangesForCurrentYear = firstPayPeriodStartWorkDayCurrentYr.GetPayPeriodRanges(26);

            //based on the hire date, calculate the employee's number of working days until last pay end date. max will be 260 days for current year.
            //Assuming that employees are hired for the whole year
            var empWorkDaysInCurrentYear = employee.HireDate.GetNumberOfWorkingDaysUntilDate(payPeriodRangesForCurrentYear.Last().EndDate);
            empWorkDaysInCurrentYear = empWorkDaysInCurrentYear > Constants.TOTAL_ANNUAL_WORK_DAYS ? Constants.TOTAL_ANNUAL_WORK_DAYS : empWorkDaysInCurrentYear;

            //calculate the total expected EE deduction with or without discount x = (work days * eeDed)/260            
            var totalExpectedEEDed = Convert.ToDecimal(empWorkDaysInCurrentYear * eeDed / Constants.TOTAL_ANNUAL_WORK_DAYS).CeilingWithPrecision(2);

            //calculate the rate of deduction for EE per day = totalExpectedEEDed / work days
            var eeRateOfDedPerWorkDay = Convert.ToDecimal(totalExpectedEEDed / empWorkDaysInCurrentYear).CeilingWithPrecision(2);

            //calculate the total expected Dep deduction without discount y = (work days * 500)/260 
            //calculate the total expected Dep deduction with discount y = (work days * 450)/260
            var totalExpectedDepDecWODiscount = Convert.ToDecimal((empWorkDaysInCurrentYear * Constants.DEP_ANNUAL_DEDUCTION) / Constants.TOTAL_ANNUAL_WORK_DAYS).CeilingWithPrecision(2);
            var totalExpectedDepDecWithDiscount = Convert.ToDecimal((empWorkDaysInCurrentYear * depDedDiscountVal) / Constants.TOTAL_ANNUAL_WORK_DAYS).CeilingWithPrecision(2);

            //calculate the rate of deduction for Dep per day DEPDed = y / work day
            var depRateOfDedPerWorkDay = Convert.ToDecimal(totalExpectedDepDecWODiscount / empWorkDaysInCurrentYear).CeilingWithPrecision(2);

            //calculate the rate of discounted deduction for Dep per day DEPDed = y / work day
            var depRateOfDedWithDiscPerWorkDay = Convert.ToDecimal(totalExpectedDepDecWithDiscount / empWorkDaysInCurrentYear).CeilingWithPrecision(2);

            //Create an entry in the employee Annual deduction for the year
            var employeeAnnualDed = new EmployeeAnnualDeduction()
            {
                EmployeeId = employee.EmployeeId,
                PayYear = currentYear,
                TotalExpectedEEDeduction = (decimal)totalExpectedEEDed,
                TotalExpectedDepDeductionWithDiscount = (decimal)totalExpectedDepDecWithDiscount,
                TotalExpectedDepDeductionWODiscount = (decimal)totalExpectedDepDecWODiscount
            };
            _context.EmployeeAnnualDeductions.Add(employeeAnnualDed);
            await _context.SaveChangesAsync();
            employeeAnnualDed = _context.EmployeeAnnualDeductions.Where(d => d.EmployeeId == employee.EmployeeId && d.PayYear == currentYear).FirstOrDefault();

            if(employeeAnnualDed !=null)
            {
                //get a list of pay ranges based on date of hire
                var employeePayPeriodRangessForCurrentYear = payPeriodRangesForCurrentYear.Where(pp => employee.HireDate.Date <= pp.EndDate.Date).ToList();

                //first pay check calculate the gross pay = (number of days first week * 8 * $25) 
                //first pay check calculate the EE deduction based on number of days  = (number of days first week * EEDed)
                //first pay check calculate the DEP deduction(s) based on number of days = (number of days first week * DEPDed)
                var numWorkingDaysFirstPeriod = employee.HireDate.GetNumberOfWorkingDaysUntilDate(employeePayPeriodRangessForCurrentYear.First().EndDate);

                CalculatePayrollPreview(employee, payrollPreviews,
                    employeeAnnualDed, empHasDiscount, eePayPerHr,
                    eeRateOfDedPerWorkDay, depRateOfDedPerWorkDay, depRateOfDedWithDiscPerWorkDay, 
                    numWorkingDaysFirstPeriod, employee.HireDate, employeePayPeriodRangessForCurrentYear.First().EndDate, 
                    false, totalExpectedEEDed, totalExpectedDepDecWithDiscount, totalExpectedDepDecWODiscount);

                //remaining paycheck gross pay = 2000
                //pay check calculate the EE deduction for 10 days = 10 * EEDed
                //pay check calculate the Dep deduction for 10 days = 10 * DEPDed
                foreach (var payPeriod in employeePayPeriodRangessForCurrentYear.Skip(1).SkipLast(1))
                {
                    
                    CalculatePayrollPreview(employee, payrollPreviews,
                        employeeAnnualDed, empHasDiscount, eePayPerHr,
                        eeRateOfDedPerWorkDay, depRateOfDedPerWorkDay, depRateOfDedWithDiscPerWorkDay, 
                        10, payPeriod.StartDate, payPeriod.EndDate, 
                        false, totalExpectedEEDed, totalExpectedDepDecWithDiscount, totalExpectedDepDecWODiscount);

                }

                //last pay period paycheck gross pay = 2000
                //last pay check calculate the EE deduction = total expected EE deduction - total actual EE deduction so far
                //last pay check calculate the DEP deduction = total expected DEP deduction - total actual DEP deduction so far

                CalculatePayrollPreview(employee, payrollPreviews,
                        employeeAnnualDed, empHasDiscount, eePayPerHr,
                        eeRateOfDedPerWorkDay, depRateOfDedPerWorkDay, depRateOfDedWithDiscPerWorkDay,
                        10, employeePayPeriodRangessForCurrentYear.Last().StartDate, employeePayPeriodRangessForCurrentYear.Last().EndDate, 
                        true, totalExpectedEEDed, totalExpectedDepDecWithDiscount, totalExpectedDepDecWODiscount);
            }

            _context.PayrollPreviews.AddRange(payrollPreviews);
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeductionsAndPreview(int employeeId)
        {
            var empDed = await _context.EmployeeAnnualDeductions.FirstOrDefaultAsync(d => d.EmployeeId == employeeId);
            if(empDed!=null)
            {
                _context.EmployeeAnnualDeductions.Remove(empDed);
                await _context.SaveChangesAsync();
            }
        }

        private void CalculatePayrollPreview(Employee employee, ICollection<PayrollPreview> payrollPreviews, 
            EmployeeAnnualDeduction employeeAnnualDed,
            bool empHasDiscount, decimal eePayPerHr, decimal eeRateOfDedPerWorkDayDay,
            decimal depRateOfDedPerWorkDay, decimal depRateOfDedWithDiscPerWorkDay,
            int numWorkingDaysForPayPeriod, DateTime payrollStartDt, DateTime payrollEndDate, 
            bool isLastPayPeriod, decimal totalExpectedEEDed, decimal totalExpectedDepDecWithDiscount, decimal totalExpectedDepDecWODiscount)
        {
            PayrollPreview eePayPreview = new PayrollPreview();
            eePayPreview.EmployeeId = employee.EmployeeId;
            eePayPreview.EmployeeAnnualDeductionId = employeeAnnualDed.EmployeeAnnualDeductionId;            
            eePayPreview.GrossSalaryForPayPeriod = numWorkingDaysForPayPeriod * 8 * eePayPerHr;
            eePayPreview.NameBasedDiscount = empHasDiscount;
            eePayPreview.PayrollStartDate = payrollStartDt;
            eePayPreview.PayRollEndDate = payrollEndDate;

            eePayPreview.TotalDeductionForPayPeriod = !isLastPayPeriod ? numWorkingDaysForPayPeriod * eeRateOfDedPerWorkDayDay
                : (totalExpectedEEDed - payrollPreviews.Where(x => x.DependentId == 0).Sum(x => x.TotalDeductionForPayPeriod));

            eePayPreview.RateOfDeductionPerWorkDay = !isLastPayPeriod? eeRateOfDedPerWorkDayDay
                : eePayPreview.TotalDeductionForPayPeriod / numWorkingDaysForPayPeriod;

            payrollPreviews.Add(eePayPreview);

            foreach (Dependent dependent in employee.Dependents)
            {
                PayrollPreview depPayPreview = new PayrollPreview();
                depPayPreview.EmployeeId = employee.EmployeeId;
                depPayPreview.DependentId = dependent.DependentId;
                depPayPreview.EmployeeAnnualDeductionId = employeeAnnualDed.EmployeeAnnualDeductionId;
                depPayPreview.NameBasedDiscount = dependent.FullName.Trim().StartsWith("a", StringComparison.OrdinalIgnoreCase);
                depPayPreview.PayrollStartDate = payrollStartDt;
                depPayPreview.PayRollEndDate = payrollEndDate;

                if (isLastPayPeriod)
                {
                    var totalDepDed = depPayPreview.NameBasedDiscount ? totalExpectedDepDecWithDiscount : totalExpectedDepDecWODiscount;
                    depPayPreview.TotalDeductionForPayPeriod = totalDepDed - payrollPreviews.Where(x => x.DependentId == dependent.DependentId).Sum(x => x.TotalDeductionForPayPeriod);

                    depPayPreview.RateOfDeductionPerWorkDay = depPayPreview.TotalDeductionForPayPeriod / numWorkingDaysForPayPeriod;
                }
                else
                {
                    depPayPreview.RateOfDeductionPerWorkDay = depPayPreview.NameBasedDiscount ? depRateOfDedWithDiscPerWorkDay : depRateOfDedPerWorkDay;
                    depPayPreview.TotalDeductionForPayPeriod = numWorkingDaysForPayPeriod * depPayPreview.RateOfDeductionPerWorkDay;
                }

                payrollPreviews.Add(depPayPreview);
            }
        }
    }
}
