﻿using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Sql.Entities;
using EmployeeManagement.Services.Util;
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
        Task<ICollection<PayrollPreview>> GetPayrollPreviewForEmployee(int employeeId, int year);
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

            //get 26 payperiodranges since first work day for the current year
            var  payPeriodRangesForCurrentYear = firstPayPeriodStartWorkDayCurrentYr.GetPayPeriodRanges(26);

            if (employee.HireDate > payPeriodRangesForCurrentYear.Last().EndDate)
            {
                //no pay previews for the current year.
                return;
            }

            EmployeeCalculationDetails employeeCalculationDetails = new EmployeeCalculationDetails(employee, payPeriodRangesForCurrentYear);

            //Create an entry in the employee Annual deduction for the year
            var employeeAnnualDed = new EmployeeAnnualDeduction()
            {
                EmployeeId = employee.EmployeeId,
                PayYear = currentYear,
                TotalExpectedEEDeduction = employeeCalculationDetails.TotalExpectedEEDed,
                TotalExpectedDepDeductionWithDiscount = employeeCalculationDetails.TotalExpectedDepDecWithDiscount,
                TotalExpectedDepDeductionWODiscount = employeeCalculationDetails.TotalExpectedDepDecWODiscount
            };

            _context.EmployeeAnnualDeductions.Add(employeeAnnualDed);
            await _context.SaveChangesAsync();
            employeeAnnualDed = _context.EmployeeAnnualDeductions.Where(d => d.EmployeeId == employee.EmployeeId && d.PayYear == currentYear).FirstOrDefault();

            if(employeeAnnualDed !=null)
            {
                //get a list of pay ranges based on date of hire
                var employeePayPeriodRangesForCurrentYear = payPeriodRangesForCurrentYear.Where(pp => employee.HireDate.Date <= pp.EndDate.Date).ToList();
                
                employeePayPeriodRangesForCurrentYear.ForEach((payPeriod) => 
                     CalculatePayrollPreviewForPayPeriod(payPeriod, employee, payrollPreviews, employeeAnnualDed, 
                     employeeCalculationDetails, employeePayPeriodRangesForCurrentYear)
                );
            }

            _context.PayrollPreviews.AddRange(payrollPreviews);
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeductionsAndPreview(int employeeId)
        {
            var empDed = await _context.EmployeeAnnualDeductions.
                Where(d => d.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (empDed!=null)
            {
                _context.EmployeeAnnualDeductions.Remove(empDed);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<PayrollPreview>> GetPayrollPreviewForEmployee(int employeeId, int year)
        {
            var eeAnnualDed = await _context.EmployeeAnnualDeductions
                .Where(d => d.EmployeeId == employeeId && d.PayYear == year)
                .FirstOrDefaultAsync();

            if (eeAnnualDed != null)
            {
                return await _context.PayrollPreviews.Where(pp => pp.EmployeeAnnualDeductionId == eeAnnualDed.EmployeeAnnualDeductionId).ToListAsync();
            }

            return new List<PayrollPreview>();
        }

        private void CalculatePayrollPreviewForPayPeriod(PayPeriod payPeriod, Employee employee, ICollection<PayrollPreview> payrollPreviews,
            EmployeeAnnualDeduction employeeAnnualDed, EmployeeCalculationDetails employeeCalculationDetails, List<PayPeriod> employeePayPeriodRangesForCurrentYear)
        {
            
            if (payPeriod.EndDate == employeePayPeriodRangesForCurrentYear.First().EndDate)
            {
                //first pay check calculate the gross pay = (number of days first week * 8 * $25) 
                //first pay check calculate the EE deduction based on number of days  = (number of days first week * EEDed)
                //first pay check calculate the DEP deduction(s) based on number of days = (number of days first week * DEPDed)
                var startDate = employee.HireDate.Date <= employeePayPeriodRangesForCurrentYear.First().StartDate
                    ? employeePayPeriodRangesForCurrentYear.First().StartDate
                    : employee.HireDate;

                var numWorkingDaysFirstPeriod = startDate.GetNumberOfWorkingDaysUntilDate(employeePayPeriodRangesForCurrentYear.First().EndDate);

                CalculatePayrollPreview(employee, payrollPreviews,
                    employeeAnnualDed, employeeCalculationDetails,
                    numWorkingDaysFirstPeriod, startDate, employeePayPeriodRangesForCurrentYear.First().EndDate, false);
            }
            else if (payPeriod.EndDate == employeePayPeriodRangesForCurrentYear.Last().EndDate)
            {
                //last pay period paycheck gross pay = 2000
                //last pay check calculate the EE deduction = total expected EE deduction - total actual EE deduction so far
                //last pay check calculate the DEP deduction = total expected DEP deduction - total actual DEP deduction so far
                CalculatePayrollPreview(employee, payrollPreviews,
                        employeeAnnualDed, employeeCalculationDetails,
                        10, employeePayPeriodRangesForCurrentYear.Last().StartDate, employeePayPeriodRangesForCurrentYear.Last().EndDate,
                        true);
            }
            else
            {
                //remaining paycheck gross pay = 2000
                //pay check calculate the EE deduction for 10 days = 10 * EEDed
                //pay check calculate the Dep deduction for 10 days = 10 * DEPDed

                CalculatePayrollPreview(employee, payrollPreviews,
                  employeeAnnualDed, employeeCalculationDetails,
                  10, payPeriod.StartDate, payPeriod.EndDate,
                  false);
            }
        }

        private void CalculatePayrollPreview(Employee employee, ICollection<PayrollPreview> payrollPreviews, 
            EmployeeAnnualDeduction employeeAnnualDed, EmployeeCalculationDetails employeeCalculationDetails,
            int numWorkingDaysForPayPeriod, DateTime payrollStartDt, DateTime payrollEndDate, bool isLastPayPeriod)
        {
            PayrollPreview eePayPreview = new PayrollPreview();
            eePayPreview.EmployeeId = employee.EmployeeId;
            eePayPreview.EmployeeAnnualDeductionId = employeeAnnualDed.EmployeeAnnualDeductionId;            
            eePayPreview.GrossSalaryForPayPeriod = numWorkingDaysForPayPeriod * 8 * employeeCalculationDetails.EePayPerHr;
            eePayPreview.NameBasedDiscount = employeeCalculationDetails.EmpHasDiscount;
            eePayPreview.PayrollStartDate = payrollStartDt;
            eePayPreview.PayRollEndDate = payrollEndDate;
            eePayPreview.EmployeePayPerHour = employeeCalculationDetails.EePayPerHr;
            eePayPreview.NumberOfWorkHoursForPayPeriod = numWorkingDaysForPayPeriod * 8;

            eePayPreview.TotalDeductionForPayPeriod = !isLastPayPeriod ? numWorkingDaysForPayPeriod * employeeCalculationDetails.EeRateOfDedPerWorkDay
                : (employeeCalculationDetails.TotalExpectedEEDed - payrollPreviews.Where(x => x.DependentId == 0).Sum(x => x.TotalDeductionForPayPeriod));

            eePayPreview.RateOfDeductionPerWorkDay = !isLastPayPeriod? employeeCalculationDetails.EeRateOfDedPerWorkDay
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
                    var totalDepDed = depPayPreview.NameBasedDiscount ? employeeCalculationDetails.TotalExpectedDepDecWithDiscount : employeeCalculationDetails.TotalExpectedDepDecWODiscount;
                    depPayPreview.TotalDeductionForPayPeriod = totalDepDed - payrollPreviews.Where(x => x.DependentId == dependent.DependentId).Sum(x => x.TotalDeductionForPayPeriod);

                    depPayPreview.RateOfDeductionPerWorkDay = depPayPreview.TotalDeductionForPayPeriod / numWorkingDaysForPayPeriod;
                }
                else
                {
                    depPayPreview.RateOfDeductionPerWorkDay = depPayPreview.NameBasedDiscount ? employeeCalculationDetails.DepRateOfDedWithDiscPerWorkDay : employeeCalculationDetails.DepRateOfDedPerWorkDay;
                    depPayPreview.TotalDeductionForPayPeriod = numWorkingDaysForPayPeriod * depPayPreview.RateOfDeductionPerWorkDay;
                }

                payrollPreviews.Add(depPayPreview);
            }
        }
    }
}
