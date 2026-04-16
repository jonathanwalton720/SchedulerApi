namespace SchedulerApi.Models
{
    public class SubscriptionScheduleDefinitionRecurrence
    {
        public SubscriptionScheduleDefinitionMinuteRecurrence MinuteRecurrence { get; set; }
        public SubscriptionScheduleDefinitionDailyRecurrence DailyRecurrence { get; set; }
        public SubscriptionScheduleDefinitionWeeklyRecurrence WeeklyRecurrence { get; set; }
        public SubscriptionScheduleDefinitionMonthlyRecurrence MonthlyRecurrence { get; set; }
        public SubscriptionScheduleDefinitionMonthlyDOWRecurrence MonthlyDOWRecurrence { get; set; }
    }
}
