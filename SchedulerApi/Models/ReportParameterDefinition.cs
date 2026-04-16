using System.Collections.Generic;

namespace SchedulerApi.Models
{
    public class ReportParameterDefinition
    {
        public string Name { get; set; }
        public string ParameterType { get; set; }
        public string ParameterState { get; set; }
        public List<object> ValidValues { get; set; }
        public bool ValidValuesIsNull { get; set; }
        public bool Nullable { get; set; }
        public bool AllowBlank { get; set; }
        public bool MultiValue { get; set; }
    }
}
