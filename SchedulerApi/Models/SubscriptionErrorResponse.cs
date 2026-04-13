namespace JonathanWalton720.SchedulerApi.Models
{
    public class SubscriptionErrorResponse
    {
        public SubscriptionError Error { get; set; } = new SubscriptionError();
    }

    public class SubscriptionError
    {
        public string? Code { get; set; }

        public string? Message { get; set; }
    }
}
