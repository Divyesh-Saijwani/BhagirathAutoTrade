using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoBhagirath.Common.Utils
{
    public static class DateTimeUtil
    {
        // Function to check if a given date is a weekend (Saturday or Sunday)
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        // Function to calculate date that is `days` working days before `endDate`
        public static DateTime CalculateWorkingDaysBefore(this DateTime endDate, int days)
        {
            DateTime resultDate = endDate;

            // Loop through days to subtract and skip weekends
            for (int i = 0; i < days; i++)
            {
                resultDate = resultDate.AddDays(-1); // Subtract one day

                // If the resulting date falls on a weekend, subtract additional days until it's a weekday
                while (IsWeekend(resultDate))
                {
                    resultDate = resultDate.AddDays(-1);
                }
            }

            return resultDate;
        }

        // Function to check the last Thursday of the month
        public static bool IsLastThursdayOfMonth(DateTime date)
        {
            // Check if the date is a Thursday
            if (date.DayOfWeek != DayOfWeek.Thursday)
            {
                return false;
            }

            // Check if the date is within the last 7 days of the month
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            int remainingDays = daysInMonth - date.Day;

            if (remainingDays < 7)
            {
                return true;
            }

            return false;
        }

        // Function to get the last Thursday of the month
        public static DateTime GetLastThursdayOfMonth(DateTime currentDate)
        {
            int year = currentDate.Year;
            int month = currentDate.Month;

            // Calculate the last day of the current month
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            // Find the last Thursday of the month
            DateTime lastThursday = lastDayOfMonth;
            while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
            {
                lastThursday = lastThursday.AddDays(-1);
            }

            // Check if the last Thursday is in the current month or the previous month
            if (lastThursday.Month == month)
            {
                return lastThursday; // Last Thursday is in the current month
            }
            else
            {
                // Last Thursday is in the previous month
                // Calculate the last Thursday of the previous month
                DateTime lastThursdayOfPreviousMonth = GetLastThursdayOfMonth(currentDate.AddMonths(-1));
                return lastThursdayOfPreviousMonth;
            }
        }

        public static List<DateTime> GetLastDayOfMonth(this DateTime currentDate, DayOfWeek day, int count = 3)
        {
            var result = new List<DateTime>();
            var lastDate = currentDate;

            for (int i = 0; i < count; i++)
            {
                // Get the last day of the current month
                var lastDayOfMonth = new DateTime(lastDate.Year, lastDate.Month, DateTime.DaysInMonth(lastDate.Year, lastDate.Month));
                lastDate = lastDayOfMonth;
                // Find the last occurrence of the specified day of the week
                while (lastDayOfMonth.DayOfWeek != day)
                {
                    lastDayOfMonth = lastDayOfMonth.AddDays(-1);
                }

                result.Add(lastDayOfMonth);
                lastDate = lastDate.AddDays(1); // Move to the next month
            }

            return result;
        }

        public static List<DateTime> GetNextDayOfWeek(this DateTime currentDate, DayOfWeek day, int count = 6)
        {
            var result = new List<DateTime>();

            // Move to the start of the next week
            var nextWeekStart = currentDate.AddDays(0 - (int)currentDate.DayOfWeek);

            for (int i = 0; i < count; i++)
            {
                // Calculate the number of days until the target day of the week
                int daysUntilTargetDay = ((int)day - (int)nextWeekStart.DayOfWeek + 7) % 7;
                if (daysUntilTargetDay == 0)
                {
                    daysUntilTargetDay = 7; // Move to the next week if it's the same day
                }

                // Find the target day in the upcoming week
                var targetDate = nextWeekStart.AddDays(daysUntilTargetDay);
                result.Add(targetDate);

                // Move to the next week for the next occurrence
                nextWeekStart = nextWeekStart.AddDays(7);
            }

            return result;
        }
    }
}
