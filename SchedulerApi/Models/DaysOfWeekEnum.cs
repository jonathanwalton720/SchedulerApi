namespace JonathanWalton720.SchedulerApi.Models
{
    [System.Flags]
    public enum DaysOfWeekEnum
    {
        None = 0,
        Sunday = 1,      // or 1 << 0
        Monday = 2,      // or 1 << 1
        Tuesday = 4,     // or 1 << 2
        Wednesday = 8,   // or 1 << 3
        Thursday = 16,   // or 1 << 4
        Friday = 32,     // or 1 << 5
        Saturday = 64    // or 1 << 6
    }
}
