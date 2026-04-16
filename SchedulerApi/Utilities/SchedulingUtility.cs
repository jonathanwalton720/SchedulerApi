using SchedulerApi.Models;
using SchedulerDb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SchedulerApi.Utilities
{
    public static class SchedulingUtility
    {
        public static int ConvertBoolListToInt(List<bool> boolValues)
        {
            int result = 0;
            for (int i = 0; i < boolValues.Count; i++)
            {
                if (boolValues[i])
                {
                    // If true, set the corresponding bit
                    result |= (1 << i);
                }
            }
            return result;
        }

        public static int GetWeekOfMonth(DateTime date)
        {
            // Get the first day of the week for the current culture
            DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            // Find the first day of the month
            DateTime firstOfMonth = new (date.Year, date.Month, 1);

            // Find the first occurrence of the culture's FirstDayOfWeek in the month or the days just before
            int daysToAdd = ((int)firstDayOfWeek - (int)firstOfMonth.DayOfWeek + 7) % 7;
            DateTime firstStartOfWeek = firstOfMonth.AddDays(daysToAdd);

            // If the date is before the first start of the week for the month (e.g., the 1st falls on a Wednesday,
            // but the culture starts the week on Sunday, so the "first week" starts the last Sunday of the previous month)
            // it is still considered the first calendar week of the month's visual calendar representation.

            // The number of full weeks passed since the first start of the week
            int weekOfMonth = (date.DayOfYear - firstStartOfWeek.DayOfYear) / 7 + 1;

            // Note: This calculation assumes a partial week at the start is Week 1.
            // It's a common interpretation for visual calendars but may not align with all specific business rules (e.g., ISO 8601).

            return weekOfMonth;
        }

        public static Dictionary<string, bool> ConvertBitmaskToMonthsOfYear(int mask)
        {
            var months = new Dictionary<string, bool>
            {
                { "January", false },
                { "February", false },
                { "March", false },
                { "April", false },
                { "May", false },
                { "June", false },
                { "July", false },
                { "August", false },
                { "September", false },
                { "October", false },
                { "November", false },
                { "December", false }
            };

            // mask for Month column will have 12 bits
            for (int i = 0; i < 12; i++)
            {
                // Create a mask for the current bit position (1, 2, 4, 8, etc.)
                int bitMask = 1 << i;

                // Use the bitwise AND operator (&) to check if the bit is set in the original mask.
                // If (mask & bitMask) is non-zero, the bit is set (true), otherwise it's zero (false).
                if ((mask & bitMask) != 0)
                {
                    string key = months.ElementAt(i).Key;
                    months[key] = true;
                }
            }

            return months;
        }

        public static Dictionary<string, bool> ConvertBitmaskToDaysOfWeek(int mask)
        {
            var days = new Dictionary<string, bool>
            {
                { "Sunday", false },
                { "Monday", false },
                { "Tuesday", false },
                { "Wednesday", false },
                { "Thursday", false },
                { "Friday", false },
                { "Saturday", false }
            };

            for (int i = 0; i < 7; i++)
            {
                // Create a mask for the current bit position (1, 2, 4, 8, etc.)
                int bitMask = 1 << i;

                // Use the bitwise AND operator (&) to check if the bit is set in the original mask.
                // If (mask & bitMask) is non-zero, the bit is set (true), otherwise it's zero (false).
                if ((mask & bitMask) != 0)
                {
                    string key = days.ElementAt(i).Key;
                    days[key] = true;
                }
            }

            return days;
        }

        public static int ConvertWhichWeekToInt(string whichWeek)
        {
            int monthlyWeek = 0;
            switch (whichWeek)
            {
                case "FirstWeek":
                    monthlyWeek = 1;
                    break;
                case "SecondWeek":
                    monthlyWeek = 2;
                    break;
                case "ThirdWeek":
                    monthlyWeek = 3;
                    break;
                case "FourthWeek":
                    monthlyWeek = 4;
                    break;
                case "FifthWeek":
                    monthlyWeek = 5;
                    break;
                default:
                    break;
            }

            return monthlyWeek;
        }

        public static string ConvertIntToWhichWeek(int value)
        {
            string whichWeek = "";
            switch (value)
            {
                case 1:
                    whichWeek = "FirstWeek";
                    break;
                case 2:
                    whichWeek = "SecondWeek";
                    break;
                case 3:
                    whichWeek = "ThirdWeek";
                    break;
                case 4:
                    whichWeek = "FourthWeek";
                    break;
                case 5:
                    whichWeek = "FifthWeek";
                    break;
                default:
                    break;
            }

            return whichWeek;
        }

        public static List<int> ConvertBitmaskToListOfInts(int mask, int length)
        {
            var days = new List<int>();

            // mask for Month column will have 12 bits
            for (int i = 0; i < length; i++)
            {
                // Create a mask for the current bit position (1, 2, 4, 8, etc.)
                int bitMask = 1 << i;

                // Use the bitwise AND operator (&) to check if the bit is set in the original mask.
                // If (mask & bitMask) is non-zero, the bit is set (true), otherwise it's zero (false).
                if ((mask & bitMask) != 0)
                {
                    days.Add(i + 1);
                }
            }

            return days;
        }

        public static List<int> GetDaysOfWeek(DaysOfWeekEnum daysOfWeek)
        {
            var days = new List<int>();
            foreach (var day in Enum.GetValues<DaysOfWeekEnum>())
            {
                if (daysOfWeek.HasFlag(day))
                {
                    switch (day)
                    {
                        case DaysOfWeekEnum.Sunday:
                            days.Add(0);
                            break;
                        case DaysOfWeekEnum.Monday:
                            days.Add(1);
                            break;
                        case DaysOfWeekEnum.Tuesday:
                            days.Add(2);
                            break;
                        case DaysOfWeekEnum.Wednesday:
                            days.Add(3);
                            break;
                        case DaysOfWeekEnum.Thursday:
                            days.Add(4);
                            break;
                        case DaysOfWeekEnum.Friday:
                            days.Add(5);
                            break;
                        case DaysOfWeekEnum.Saturday:
                            days.Add(6);
                            break;
                    }
                }
            }
            return days;
        }

        public static string ConvertDaysOfMonthToString(int value)
        {
            var list = ConvertBitmaskToListOfInts(value, 31);

            return string.Join(",", list);
        }

        public static int ParseDaysOfMonth(string daysValue)
        {
            int daysOfMonth = 0;
            var days = daysValue.Split(',').Select(day => int.Parse(day));
            for (var i = 0; i < 31; i++)
            {
                if (days.Contains(i + 1))
                {
                    daysOfMonth |= (1 << i);
                }
            }

            return daysOfMonth;
        }

        public static DateTime? GetNextRunDate(SubscriptionTask task, DateTime now)
        {
            DateTime nextRunDate = task.NextRunDate ?? task.StartDate;
            var lastRunDate = nextRunDate;
            switch (task.RecurrenceTypeID)
            {
                case 1: // once
                    return null;
                case 2: // hours
                    // keep finding next run date until it is in the future
                    while (nextRunDate < now)
                    {
                        nextRunDate = nextRunDate.AddMinutes(task.MinutesInterval.Value);
                    }
                    break;
                case 3: // days
                    // keep finding next run date until it is in the future
                    while (nextRunDate < now)
                    {
                        nextRunDate = nextRunDate.AddDays(task.DaysInterval.Value);
                    }
                    break;
                case 4: // weeks
                    var daysOfWeekEnum = (DaysOfWeekEnum)task.DaysOfWeek;
                    var daysOfWeek = SchedulingUtility.GetDaysOfWeek(daysOfWeekEnum);
                    var dayOfWeek = now.DayOfWeek;
                    while (nextRunDate < now)
                    {
                        DateTime nextDayOfWeek = nextRunDate;
                        int daysToAdd = 0;
                        if ((int)nextDayOfWeek.DayOfWeek >= daysOfWeek.Max())
                        {
                            daysToAdd = 8 - (int)nextDayOfWeek.DayOfWeek + (((task.WeeksInterval ?? 1) - 1) * 7);
                        }
                        else
                        {
                            do
                            {
                                nextDayOfWeek = nextDayOfWeek.AddDays(1);

                            } while (!daysOfWeek.Contains((int)nextDayOfWeek.DayOfWeek));
                            daysToAdd = ((int)nextDayOfWeek.DayOfWeek - (int)nextRunDate.DayOfWeek + 7) % 7;
                        }
                        nextRunDate = nextRunDate.AddDays(daysToAdd);
                    }
                    break;
                case 5: // days of month
                    var days = SchedulingUtility.ConvertBitmaskToListOfInts(task.DaysOfMonth.Value, 31);
                    // first get the next day of month

                    var lastRunDay = nextRunDate.Day;
                    var nextDay = days.FirstOrDefault(day => (day > lastRunDay));
                    if (nextDay != 0)
                    {
                        var addDays = nextDay - lastRunDay;
                        nextRunDate = nextRunDate.AddDays(addDays);
                    }
                    else
                    {
                        var months = SchedulingUtility.ConvertBitmaskToListOfInts(task.Month.Value, 12);
                        var thisMonth = now.Month;
                        var nextMonth = months.FirstOrDefault(month => month > thisMonth);
                        if (nextMonth != 0)
                        {
                            var addMonths = nextMonth - thisMonth;
                            nextRunDate = nextRunDate.AddMonths(addMonths);
                            int targetDay = days.FirstOrDefault() == 0 ? 1 : days.FirstOrDefault();
                            var addDays = targetDay - nextRunDate.Day;
                            nextRunDate = nextRunDate.AddDays(addDays);
                        }
                        else
                        {
                            var firstMonth = months.FirstOrDefault();
                            var addMonths = 12 + firstMonth - thisMonth;
                            nextRunDate = nextRunDate.AddMonths(addMonths);
                            int targetDay = days.FirstOrDefault() == 0 ? 1 : days.FirstOrDefault();
                            var addDays = targetDay - nextRunDate.Day;
                            nextRunDate = nextRunDate.AddDays(addDays);
                        }
                    }

                    break;
                case 6:
                    var daysOfWeekEnum2 = (DaysOfWeekEnum)task.DaysOfWeek;
                    var daysOfWeek2 = SchedulingUtility.GetDaysOfWeek(daysOfWeekEnum2);
                    var dayOfWeek2 = now.DayOfWeek;
                    var months2 = SchedulingUtility.ConvertBitmaskToListOfInts(task.Month.Value, 12);
                    var whichWeek = task.MonthlyWeek;
                    var currentWeek = GetCurrentWeek(nextRunDate);
                    do
                    {
                        DateTime nextDayOfWeek = nextRunDate;
                        int daysToAdd = 0;

                        do
                        {
                            nextDayOfWeek = nextDayOfWeek.AddDays(1);

                        } while (!daysOfWeek2.Contains((int)nextDayOfWeek.DayOfWeek));
                        daysToAdd = ((int)nextDayOfWeek.DayOfWeek - (int)nextRunDate.DayOfWeek + 7) % 7;
                        if (daysToAdd == 0)
                        {
                            daysToAdd = 7;
                        }

                        nextRunDate = nextRunDate.AddDays(daysToAdd);
                        currentWeek = GetCurrentWeek(nextRunDate);

                    } while (nextRunDate < now || !months2.Contains(nextRunDate.Month) || (whichWeek.HasValue && whichWeek.Value != currentWeek));
                    break;
                default:
                    break;
            }
            return nextRunDate;
        }

        private static int GetCurrentWeek(DateTime date)
        {
            var currentMonth = new DateTime(date.Year, date.Month, 1);
            var monthStartDay = currentMonth.DayOfWeek;
            var currentDate = date.Day;
            var currentWeek = 0;
            for (int i = 0; i < 5; i++)
            {
                var threshold = 8 - (int)monthStartDay + (i * 7);
                if (currentDate < threshold)
                {
                    currentWeek = i + 1;
                    break;
                }
            }
            return currentWeek;
        }
    }
}
