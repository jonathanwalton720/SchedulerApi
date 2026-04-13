using JonathanWalton720.SchedulerApi.Models;
using JonathanWalton720.SchedulerApi.Utilities;
using System.Text.Json;

namespace JonathanWalton720.SchedulerApi.Services
{
    public class SchedulingService : BackgroundService
    {
        private readonly int _schedulingServiceInterval;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IRepository _repository;


        public string UserName => Environment.UserName;

        public SchedulingService(IRepository repository, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _schedulingServiceInterval = configuration.GetValue<int>("ReportSchedulingIntervalSeconds", 20);

            _logger = loggerFactory.CreateLogger<SchedulingService>();
            _httpClient = httpClientFactory.CreateClient();
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    var subscriptionSchedules = _repository.GetSubscriptionTasks();
                    foreach (var subscriptionSchedule in subscriptionSchedules)
                    {
                        await DoWorkAsync(subscriptionSchedule);
                    }

                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation gracefully
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the scheduled task.");
                }
                // Delay for the specified interval before the next execution
                await Task.Delay(_schedulingServiceInterval * 1000, stoppingToken); // 1000 milliseconds in a second
            }
        }

        private async Task DoWorkAsync(SubscriptionTask task)
        {
            var lastRunDate = task.NextRunDate ?? GetDefaultLastRunDate(task);

            // check to see if there are any report parameters to update
            var hasParameters = await HasReportParameters(task);
            if (!hasParameters.HasValue)
            {
                return;
            }

            // update the report parameters if needed
            if (hasParameters.Value && task.NextRunDate.HasValue)
            {
                bool flowControl = await UpdateParameters(task, lastRunDate);
                if (!flowControl)
                {
                    return;
                }
            }

            // execute report subscription
            var executeResponse = await _httpClient.PostAsync("Subscriptions(" + task.SubscriptionId + ")/Model.Execute", new StringContent("{}"));
            try
            {
                executeResponse.EnsureSuccessStatusCode();
                _logger.LogInformation($"Subscription {task.SubscriptionId} success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"error executing Subscription {task.SubscriptionId}");
                return;
            }

            // update the next run time
            task.NextRunDate = SchedulingUtility.GetNextRunDate(task, DateTime.Now);
            _repository.ProcessSubscriptionSchedule(task);

        }

        private DateTime GetDefaultLastRunDate(SubscriptionTask task)
        {
            switch (task.RecurrenceTypeID)
            {
                case 1: // minute
                    if (!task.MinutesInterval.HasValue)
                    {
                        return task.StartDate.AddMinutes(-60);
                    }
                    return task.StartDate.AddMinutes(-task.MinutesInterval.Value);
                case 2: // daily
                    if (!task.DaysInterval.HasValue)
                    {
                        return task.StartDate.AddDays(-1);
                    }
                    return task.StartDate.AddDays(-task.DaysInterval.Value);
                case 3: // weekly
                    return task.StartDate.AddDays(-7);
                case 4: // monthly
                    return task.StartDate.AddMonths(-1);
                case 5: // monthlyDOW
                    return task.StartDate.AddMonths(-1);
                default:
                    return task.StartDate.AddDays(-7);
            }
        }

        private async Task<bool> UpdateParameters(SubscriptionTask task, DateTime lastRunDate)
        {
            if (!task.NextRunDate.HasValue)
            {
                _logger.LogWarning($"NextRunDate is null for subscription {task.SubscriptionId}, skipping parameter update.");
                return false;
            }
            var parameterValues = new List<SubscriptionParameter>
                {
                    new SubscriptionParameter { Name = "StartDate", Value = lastRunDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                    new SubscriptionParameter { Name = "EndDate", Value = task.NextRunDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") }
                };

            var patch = new
            {
                ParameterValues = parameterValues
            };

            var content = JsonSerializer.Serialize(patch);
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            try
            {
                var response2 = await _httpClient.PatchAsync("Subscriptions(" + task.SubscriptionId + ")", byteContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"error patching ParameterDefinitions {task.ReportPath}");
                return false;
            }

            return true;
        }

        private async Task<bool?> HasReportParameters(SubscriptionTask task)
        {
            var hasParameters = false;

            ReportParameterDefinitionResponse? parameterDefinitionResponse;
            try
            {
                var response = await _httpClient.GetAsync($"Reports(path%3D'{task.ReportPath}')/ParameterDefinitions");
                var json = await response.Content.ReadAsStringAsync();
                parameterDefinitionResponse = JsonSerializer.Deserialize<ReportParameterDefinitionResponse>(json);
                if (parameterDefinitionResponse != null)
                {
                    hasParameters = parameterDefinitionResponse.value.Any(n => n.Name == "StartDate") && parameterDefinitionResponse.value.Any(n => n.Name == "EndDate");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"error executing ParameterDefinitions {task.ReportPath}");
                return null;
            }

            return hasParameters;
        }
    }
}
