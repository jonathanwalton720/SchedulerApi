using System;

namespace JonathanWalton720.SchedulerApi.Models
{
    public class SubscriptionScheduleDefinition
    {
        public DateTime StartDateTime { get; set; }

        public bool EndDateSpecified { get; set; }

        public DateTime EndDate { get; set; }

        public SubscriptionScheduleDefinitionRecurrence Recurrence { get; set; }

    }
}
