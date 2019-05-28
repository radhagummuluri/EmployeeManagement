using System;
using System.Collections.Generic;

namespace EmployeeManagement.Services.Util
{
    public static class PayPeriodExtensions
    {
        public static ICollection<PayPeriod> GetPayPeriodRanges(this DateTime startDate, int howMany)
        {
            List<PayPeriod> payDateRanges = new List<PayPeriod>();
            var payDates = GetFridayPaydays(startDate, howMany);

            foreach (DateTime payDate in payDates)
            {
                var payStartDate = payDate.AddDays(-11);
                payDateRanges.Add(new PayPeriod(payStartDate, payDate));
            }
            return payDateRanges;
        }
        
        public static int GetNumberOfWorkingDaysUntilDate(this DateTime startDate, DateTime stopDate)
        {
            int days = 0;
            while (startDate <= stopDate)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    ++days;
                }
                startDate = startDate.AddDays(1);
            }
            return days;
        }

        public static IEnumerable<DateTime> GetFridayPaydays(this DateTime startDate, int howMany)
        {
            int count = 0, fridays = 0;

            while (count < howMany)
            {
                startDate = startDate.AddDays(1);

                if (startDate.DayOfWeek == DayOfWeek.Friday && fridays == 0)
                {
                    fridays++;
                }
                else if (startDate.DayOfWeek == DayOfWeek.Friday)
                {
                    fridays = 0;
                    count++;
                    yield return startDate;
                }
            }
        }

        public static decimal CeilingWithPrecision(this decimal value, int decimalPlaces)
        {
            decimal adjustment = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Math.Ceiling(value * adjustment) / adjustment;
        }
    }
}
