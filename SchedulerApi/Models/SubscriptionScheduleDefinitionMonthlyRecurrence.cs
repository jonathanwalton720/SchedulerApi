using System.Collections.Generic;

namespace SchedulerApi.Models
{
    public class SubscriptionScheduleDefinitionMonthlyRecurrence
    {
        public string Days { get; set; }
        public Dictionary<string, bool> MonthsOfYear { get; set; }
    }
}
