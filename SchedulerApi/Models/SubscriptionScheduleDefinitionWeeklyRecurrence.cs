using System.Collections.Generic;

namespace JonathanWalton720.SchedulerApi.Models
{
    public class SubscriptionScheduleDefinitionWeeklyRecurrence
    {
        public bool WeeksIntervalSpecified { get; set; }
        public int WeeksInterval { get; set; }
        public Dictionary<string, bool> DaysOfWeek { get; set; }
    }
}
