using System;

namespace SchedulerApi.Models
{
    public class SubscriptionParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsValueFieldReference { get; set; } = false;
    }
}
