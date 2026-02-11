using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace to_integrations.HelperMethods
{
    public static class HelperMethods
    {
        public static (string startDate, string endDate) GetValidDateRange(
           int startInDays = 7,
           int stayLengthDays = 5)
        {
            var start = DateTime.UtcNow.Date.AddDays(startInDays);
            var end = start.AddDays(stayLengthDays);

            return (
                start.ToString("yyyy-MM-dd"),
                end.ToString("yyyy-MM-dd")
            );
        }
    }
}
