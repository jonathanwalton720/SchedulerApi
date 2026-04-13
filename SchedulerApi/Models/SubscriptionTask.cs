using JonathanWalton720.SchedulerApi.Models;

namespace JonathanWalton720.SchedulerApi.Models
{
    public class SubscriptionTask
    {
        public DateTime? NextRunDate { get; set; }
        public DateTime StartDate { get; set; }
        public int RecurrenceTypeID { get; set; }
        public int? MinutesInterval { get; set; }
        public int? DaysInterval { get; set; }
        public DaysOfWeekEnum DaysOfWeek { get; set; }
        public int? WeeksInterval { get; set; }
        public int? DaysOfMonth { get; set; }
        public int? Month { get; set; }
        public int? MonthlyWeek { get; set; }
        public string SubscriptionId { get; internal set; }
        public object ReportPath { get; internal set; }
    }
}