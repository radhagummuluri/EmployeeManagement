using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Data.Entities
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public String Position { get; set; }

        [Column(TypeName = "date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Date Of Hire")]
        public DateTime HireDate { get; set; }

        [Column(TypeName = "varchar(100)")]
        [DisplayName("Office Location")]
        public string OfficeLocation { get; set; }

        [Required]
        public bool CalculatePayrollPreview { get; set; }

        [Required]
        public int Salary { get; set; }

        public ICollection<Dependent> Dependents { get; set; }
    }
}
