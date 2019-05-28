using EmployeeManagement.Services.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Services
{
    public class PayPeriodExtensionsTests
    {
        [Fact]
        public void GetNumberOfWorkingDaysUntilDate_FromLastDayOf2018_ToLastPayCheckEndDate_Returns260WorkingDays()
        {
            var workDays = Convert.ToDateTime("12/31/2018").GetNumberOfWorkingDaysUntilDate(Convert.ToDateTime("12/27/2019"));
            Assert.Equal(260, workDays);
        }

        [Fact]
        public void GetNumberOfWorkingDaysUntilDate_GivenMidYearStartDate_VerifyWorkingDays()
        {
            var workDays = Convert.ToDateTime("06/01/2019").GetNumberOfWorkingDaysUntilDate(Convert.ToDateTime("12/27/2019"));
            Assert.Equal(150, workDays);
        }

        [Fact]
        public void GetPayPeriodRanges_GivenStartDate_VerifyRanges()
        {
            var payRanges = Convert.ToDateTime("12/31/2018").GetPayPeriodRanges(26);
            Assert.Equal(Convert.ToDateTime("12/31/2018").Date, payRanges.First().StartDate.Date);
            Assert.Equal(Convert.ToDateTime("01/11/2019").Date, payRanges.First().EndDate.Date);

            Assert.Equal(Convert.ToDateTime("2019-12-16").Date, payRanges.Last().StartDate);
            Assert.Equal(Convert.ToDateTime("2019-12-27").Date, payRanges.Last().EndDate);
        }
    }
}
