
using Microsoft.EntityFrameworkCore;
using SchedulerDb.Models;

namespace SchedulerDb
{
    public class SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : DbContext(options)
    {
        // Define DbSet properties for your entities here   
        public DbSet<SubscriptionTask> SubscriptionTasks { get; set; }
    }
}
