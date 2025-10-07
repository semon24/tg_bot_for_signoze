using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using static VneOcherediGuard.Extensions.MarkdownExtensions;


namespace VneOcherediGuard
{
    public class MessagesBuilder
    {
        public readonly string deploymentEnvironment = string.Empty;
        public readonly bool sendInGroup = false;
        public readonly double thresholdAlelrtValue;
        public readonly bool filterByTime;

        public StringBuilder message = new StringBuilder();

        public string status;
        public string emojiStatus;

        public string severity;
        public string emojiSeverity;

        public string description;
        public string timeFirstIncidient;
        public string link;
        public string name;

        public string head;


        public MessagesBuilder(Alert alert)
        {
               
            CreateStatusAndHeadString(alert.Status);
            CreateSeverityString(alert.Labels?.Severity);
            deploymentEnvironment = alert.Labels?.DeploymentEnvironment ?? "";
            sendInGroup = bool.Parse(alert.Labels?.SendInGroup ?? "false");
            thresholdAlelrtValue = double.Parse((alert.Labels?.ThresholdAlelrtValue?.Replace('.', ',') ?? "0,0").Trim('"'));
            description = alert.Annotations?.Description ?? "нет сообщения";
            timeFirstIncidient = ParseTime(alert.StartsAt);
            link = alert.GeneratorURL ?? "";
            name = alert.Labels?.Alertname ?? "";
            filterByTime = bool.Parse(alert.Labels?.FilteredByTime ?? "false");
            BuildMessage();
            
        }

        public void BuildMessage()
        {
            message.Append($"{head}\n");
            message.Append($"Название: {TextSanitize(name)}\n");
            message.Append($"Статус: {TextSanitize(status)} {emojiStatus}\n");
            message.Append($"Уровень: {TextSanitize(severity)} {emojiSeverity}\n");
            message.Append($"Описание: {TextSanitize(description)}\n");
            message.Append($"Окружение: {TextSanitize(deploymentEnvironment)}\n");
            message.Append($"Время: {TextSanitize(timeFirstIncidient)}\n");
            message.Append($"Ссылка: {TextSanitize(link)}\n");
        }

        public string ParseTime(string? timeStartAt)
        {
            if (DateTime.TryParse(timeStartAt, out DateTime startsAt))
            {
               return startsAt.ToString("dd.MM.yyyy HH:mm:ss");
            }
            return "";
        }

        private void CreateStatusAndHeadString(string? status)
        {
            switch (status)
            {
                case "firing":
                    this.status = "Firing";
                    emojiStatus = "❌";
                    head = "*Случилась беда\\!*";
                    break;
                case "resolved":
                    this.status = "Resolved";
                    emojiStatus = "✔️";
                    head = "*Проблема решена\\!*";
                    break;
                default:
                    this.status = status ?? "";
                    emojiStatus = "";
                    head = "";
                    break;
            }
        }

        private void CreateSeverityString(string? severity)
        {
            switch (severity)
            {
                case "warning":
                    this.severity = "Warning";
                    emojiSeverity = "⚠️";
                    break;
                case "eror":
                    this.severity = "Eror";
                    emojiSeverity = "‼️";
                    break;
                case "info":
                    this.severity = "Info";
                    emojiSeverity = "🚬";
                    break;
                case "critical":
                    this.severity = "Critical";
                    emojiSeverity = "⚰️";
                    break;
                default:
                    this.severity = severity ?? "";
                    emojiSeverity = "";
                    break;
            }
        }
    }
}
