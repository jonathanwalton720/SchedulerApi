using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SchedulerApi.Models
{
    public class Subscription
    {
        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; } = "http://report-server:8080/Reports/api/v2.0/$metadata#Subscriptions/$entity";
        public Guid Id { get; set; }
        public string Owner { get; set; }
        public bool IsDataDriven { get; set; }
        public string Description { get; set; }
        public string Report { get; set; }
        public bool IsActive { get; set; }
        public string EventType { get; set; }
        public SubscriptionSchedule Schedule { get; set; } = new SubscriptionSchedule();
        public string ScheduleDescription { get; set; }
        public string LastRunTime { get; set; }
        public string LastStatus { get; set; }
        public string DataQuery { get; set; }
        public object ExtensionSettings { get; set; }
        public string DeliveryExtension { get; set; }
        public string LocalizedDeliveryExtensionName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public List<SubscriptionParameter> ParameterValues { get; set; } = [];
    }
}
