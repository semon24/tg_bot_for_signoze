using Microsoft.Extensions.Options;

namespace VneOcherediGuard.Options;

public sealed class WebhookOptions 
{
    public string Url { get; set; }
}