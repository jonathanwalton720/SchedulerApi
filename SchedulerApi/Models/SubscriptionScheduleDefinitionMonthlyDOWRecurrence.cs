using System.Collections.Generic;

namespace SchedulerApi.Models
{
    public class SubscriptionScheduleDefinitionMonthlyDOWRecurrence
    {
        public string WhichWeek { get; set; }
        public bool WhichWeekSpecified { get; set; }
        public Dictionary<string, bool> DaysOfWeek { get; set; }
        public Dictionary<string, bool> MonthsOfYear { get; set; }
    }
}
