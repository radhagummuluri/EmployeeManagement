using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Data.Util
{
    public class PayPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public PayPeriod(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}