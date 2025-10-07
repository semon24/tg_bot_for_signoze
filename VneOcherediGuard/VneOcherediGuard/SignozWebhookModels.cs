using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VneOcherediGuard
{
        public class Alert
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("labels")]
        public Labels? Labels { get; set; }

        [JsonPropertyName("annotations")]
        public Annotations? Annotations { get; set; }

        [JsonPropertyName("startsAt")]
        public string? StartsAt { get; set; }

        [JsonPropertyName("endsAt")]
        public string? EndsAt { get; set; }

        [JsonPropertyName("generatorURL")]
        public string? GeneratorURL { get; set; }

        [JsonPropertyName("fingerprint")]
        public string? Fingerprint { get; set; }
    }

    public class Annotations
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("related_logs")]
        public string? RelatedLogs { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }
    }

    public class CommonAnnotations
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("related_logs")]
        public string? RelatedLogs { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }
    }

    public class CommonLabels
    {
        [JsonPropertyName("alertname")]
        public string? Alertname { get; set; }

        [JsonPropertyName("deployment.environment")]
        public string? DeploymentEnvironment { get; set; }

        [JsonPropertyName("ruleId")]
        public string? RuleId { get; set; }

        [JsonPropertyName("ruleSource")]
        public string? RuleSource { get; set; }

        [JsonPropertyName("severity")]
        public string? Severity { get; set; }

        [JsonPropertyName("threshold.name")]
        public string? ThresholdName { get; set; }

        [JsonPropertyName("sendInGroup")]
        public string? SendInGroup { get; set; }

        [JsonPropertyName("thresholdAlelrtValue")]
        public string? ThresholdAlelrtValue { get; set; }

        [JsonPropertyName("filteredByTime")]
        public string? FilteredByTime { get; set; }
    }

    public class GroupLabels
    {
        [JsonPropertyName("alertname")]
        public string? Alertname { get; set; }
    }

    public class Labels
    {
        [JsonPropertyName("alertname")]
        public string? Alertname { get; set; }

        [JsonPropertyName("deployment.environment")]
        public string? DeploymentEnvironment { get; set; }

        [JsonPropertyName("ruleId")]
        public string? RuleId { get; set; }

        [JsonPropertyName("ruleSource")]
        public string? RuleSource { get; set; }

        [JsonPropertyName("severity")]
        public string? Severity { get; set; }

        [JsonPropertyName("threshold.name")]
        public string? ThresholdName { get; set; }

        [JsonPropertyName("sendInGroup")] 
        public string? SendInGroup { get; set; }

        [JsonPropertyName("thresholdAlelrtValue")] 
        public string? ThresholdAlelrtValue { get; set; }

        [JsonPropertyName("filteredByTime")]
        public string? FilteredByTime { get; set; }
    }

    public class SignozWebhookBody
    {
        [JsonPropertyName("receiver")]
        public string? Receiver { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("alerts")]
        public List<Alert>? Alerts { get; set; }

        [JsonPropertyName("groupLabels")]
        public GroupLabels? GroupLabels { get; set; }

        [JsonPropertyName("commonLabels")]
        public CommonLabels? CommonLabels { get; set; }

        [JsonPropertyName("commonAnnotations")]
        public CommonAnnotations? CommonAnnotations { get; set; }

        [JsonPropertyName("externalURL")]
        public string? ExternalURL { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("groupKey")]
        public string? GroupKey { get; set; }

        [JsonPropertyName("truncatedAlerts")]
        public int? TruncatedAlerts { get; set; }
    }
}