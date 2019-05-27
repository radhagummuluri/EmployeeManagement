using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmployeeManagement.Data.Entities
{
    public class PayrollPreview
    {
        [Key]
        public int PayrollPreviewId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public int DependentId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime PayrollStartDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime PayRollEndDate { get; set; }

        [Required]
        public decimal GrossSalaryForPayPeriod { get; set; }

        [Required]
        public decimal TotalDeductionForPayPeriod { get; set; }

        [Required]
        public decimal RateOfDeductionPerWorkDay { get; set; }

        [Required]
        public bool NameBasedDiscount { get; set; }

        public int EmployeeAnnualDeductionId { get; set; }
        public EmployeeAnnualDeduction EmployeeAnnualDeduction { get; set; }
    }
}
