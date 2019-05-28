# EmployeeManagement
This solution is for managing employees and dependents and get a preview of the payroll for every period.
Along with providing facility to create/read/update/delete employee and dependents this application calculates the pay preview per period.
The pay preview screen presents the following information per pay period:
- pay period beginning date
- pay period end date
- number of work hours in the pay period
- employee pay rate/hr
- gross salary per pay period
- total deductions for the pay period
- net salary per pay period
- Year to date gross salary
- Year to date net salary
- deduction amount per census type (employee/dependents)
- year to date deduction amount per census type

Basic Design Goals: 
- Solution has a layered architecture where every layer has specific responsibility/concerns.
- UI is intuitive to use
- Decouple the code between layers using dependency injection framework
- Add test coverage to improve code reliability

This solution is split into multi layers with specific responsibilities and has UI, Business Logic (BLL) and Data Access (DAL) layers.
The solution consists of the following projects:

1. EmployeeManagement.Web 
	This is a UI layer that is an ASP.NET core 2.2 MVC web application. 
	This project only interacts with the Services layer and presents information related to Employee/Dependents and Payroll preview 
	in Razor views.
 
2. EmployeeManagement.Services
	This is the Business Logic layer and an ASP.NET core library project. 
	This has the service classes that are called by the web layer. 
	This service layer communicates with the Data Access layer.
	The service classes have the DBContext class from the Data access layer injected in them so that thay can perform CRUD 
	operations in the database.

3. EmployeeManagement.Data 
	This is the Data access layer. It has the Entity classes that represent the database model and the DBContext class that is used to interact with the database.
	
4. EmployeeManagement.Test
	This project has unit tests that test controllers, services and extension methods.

5. Database is set to mssql sqlexpress server instance.
	
Assumptions:
- Employee makes $2000 per pay period
- Employee deduction is $1000/ year and $900 if name starts with "A" (case insensitive)
- Dependent deduction is $500/ year and $450 if name starts with "A" (case insensitive)
- There are 26 pay periods in a year. 
- Employee Pay schedule is calculated for the current year only (2019). Anyone with date of hire on or before 12/31/2018 has 26 pay periods in 2019.
- Prorated the employee deductions based on hire date if they do not have all 26 pay previews for the year.
- Qualifying events such as addition of a dependent/ marriage etc. at a later date in the year are not considered. 
  The application recalculates the payroll preview with deductions for the entire year if employee information changes or when dependents are added or removed.

Ways to improve the basic version of this application:
- Add authentication/authorization feature.
- Have seperate secure micro service for employee management (CRUD operations) and another microservice for payroll preview calculation,
  so that each service can be containerized and the application can scale better.
- Add logging to splunk/stackify for non development environments.
- Add integration tests to test the microservice webapis.