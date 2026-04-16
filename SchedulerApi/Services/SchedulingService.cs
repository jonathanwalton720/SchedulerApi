using SchedulerApi.Models;
using SchedulerApi.Utilities;
using SchedulerDb.Models;
using System.Text.Json;

namespace SchedulerApi.Services
{
    public class SchedulingService(IRepository repository, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IConfiguration configuration) : BackgroundService
    {
        private readonly int _schedulingServiceInterval = configuration.GetValue<int>("ReportSchedulingIntervalSeconds", 20);
        private readonly ILogger _logger = loggerFactory.CreateLogger<SchedulingService>();
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly IRepository _repository = repository;

        public string UserName => Environment.UserName;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

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
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Subscription {subscriptionId} success", task.SubscriptionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing Subscription {subscriptionId}", task.SubscriptionId);
                return;
            }

            // update the next run time
            task.NextRunDate = SchedulingUtility.GetNextRunDate(task, DateTime.Now);
            _repository.ProcessSubscriptionSchedule(task);

        }

        private static DateTime GetDefaultLastRunDate(SubscriptionTask task)
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
                _logger.LogWarning("NextRunDate is null for subscription {subscriptionId}, skipping parameter update.", task.SubscriptionId);
                return false;
            }
            var parameterValues = new List<SubscriptionParameter>
            {
                new() { Name = "StartDate", Value = lastRunDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                new() { Name = "EndDate", Value = task.NextRunDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") }
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
                _logger.LogError(ex, "Error patching ParameterDefinitions {reportPath}", task.ReportPath);
                return false;
            }

            return true;
        }

        private async Task<bool?> HasReportParameters(SubscriptionTask task)
        {
            var hasParameters = false;

            ReportParameterDefinitionResponse parameterDefinitionResponse;
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
                _logger.LogError(ex, "Error executing ParameterDefinitions {reportPath}", task.ReportPath);
                return null;
            }

            return hasParameters;
        }
    }
}
