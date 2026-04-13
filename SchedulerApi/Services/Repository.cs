using JonathanWalton720.SchedulerApi.Models;

namespace JonathanWalton720.SchedulerApi.Services
{
    public interface IRepository
    {
        List<SubscriptionTask> GetSubscriptionTasks();
        void ProcessSubscriptionSchedule(SubscriptionTask task);
    }


    public class Repository : IRepository
    {
        public List<SubscriptionTask> GetSubscriptionTasks()
        {
            throw new NotImplementedException();
        }

        public void ProcessSubscriptionSchedule(SubscriptionTask task)
        {
            throw new NotImplementedException();
        }
    }
}
