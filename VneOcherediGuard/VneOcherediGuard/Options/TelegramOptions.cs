using Microsoft.Extensions.Options;

namespace VneOcherediGuard.Options;

public sealed class TelegramOptions 
{
    public string Token { get; set; }
    public List<long> ChatIds { get; set; }
}