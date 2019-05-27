using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Data.Entities
{
    public class EmployeeAnnualDeduction
    {
        [Key]
        public int EmployeeAnnualDeductionId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int PayYear { get; set; }

        [Required]
        public decimal TotalExpectedEEDeduction { get; set; }

        [Required]
        public decimal TotalExpectedDepDeductionWODiscount { get; set; }

        [Required]
        public decimal TotalExpectedDepDeductionWithDiscount { get; set; }

        public ICollection<PayrollPreview> PayrollPreviews { get; set; }
    }
}
