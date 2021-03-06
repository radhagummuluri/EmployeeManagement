/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *
  FROM [EmployeeDB].[dbo].[Employees]

SELECT *
  FROM [EmployeeDB].[dbo].[Dependents]

SELECT *
  FROM [EmployeeDB].[dbo].[EmployeeAnnualDeductions]

SELECT *
  FROM [EmployeeDB].[dbo].[PayrollPreviews] pp
  order by pp.PayrollStartDate asc


  select pp.EmployeeId, pp.DependentId, sum(pp.TotalDeductionForPayPeriod)
  FROM [EmployeeDB].[dbo].[PayrollPreviews] as pp
  group by pp.EmployeeId, pp.DependentId

  
  select pp.EmployeeId, pp.PayrollStartDate, pp.PayRollEndDate, sum(pp.GrossSalaryForPayPeriod) as GrossPay, 
  sum(pp.TotalDeductionForPayPeriod) as TotalDeduction, 
  (sum(pp.GrossSalaryForPayPeriod) - sum(pp.TotalDeductionForPayPeriod)) as NetPay
  FROM [EmployeeDB].[dbo].[PayrollPreviews] as pp
  group by pp.EmployeeId, pp.PayrollStartDate, pp.PayRollEndDate
  









