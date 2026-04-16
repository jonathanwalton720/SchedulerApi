using SchedulerDb;
using SchedulerDb.Models;

namespace SchedulerApi.Services
{
    public interface IRepository
    {
        List<SubscriptionTask> GetSubscriptionTasks();
        void ProcessSubscriptionSchedule(SubscriptionTask task);
    }


    public class Repository(SchedulerDbContext context) : IRepository
    {
        private readonly SchedulerDbContext _context = context;

        public List<SubscriptionTask> GetSubscriptionTasks()
        {

            var susbscriptionTasks = from st in _context.SubscriptionTasks
                                     where st.NextRunDate < DateTime.Now && st.NextRunDate != null 
                                     select _context.SubscriptionTasks;
            return [.. _context.SubscriptionTasks];
        }

        public void ProcessSubscriptionSchedule(SubscriptionTask task)
        {
            throw new NotImplementedException();
        }
    }
}
