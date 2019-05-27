using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmployeeManagement.Data.Entities
{
    public class Dependent
    {
        [Key]
        public int DependentId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [Column(TypeName = "varchar(250)")]
        [Required]
        public string Relationship { get; set; }
        
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
