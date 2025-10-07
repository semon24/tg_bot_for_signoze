using System.Net;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using static VneOcherediGuard.Extensions.MarkdownExtensions;
using VneOcherediGuard.Options;

namespace VneOcherediGuard;

public class SimpleWebhook : ITelegramBotTask
{
    private HttpListener? _listener;
    private bool _isRunning;
    
    private readonly TelegramBot _telegramBot;
    private readonly IOptionsMonitor<WebhookOptions> _webhookOptions;
    public MessagesBuilder _messageBuilder;

    public SimpleWebhook(TelegramBot telegramBot, IOptionsMonitor<WebhookOptions> webhookOptions)
    {
        _telegramBot = telegramBot;
        _webhookOptions = webhookOptions;

    }
    
    public async Task StartAsync()
    {
        _isRunning = true;
        Console.WriteLine("Запускаю вебхук сервер...");

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_webhookOptions.CurrentValue.Url);
            _listener.Start();
            
            await Task.Run(HandleRequests);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска вебхук сервера: {ex.Message}");
        }
    }

    private async Task HandleRequests()
    {
        while (_isRunning && _listener != null)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                await Task.Run(() => ProcessRequest(context));
            }
            catch (Exception ex)
            {
                if (_isRunning)
                    Console.WriteLine($"️Ошибка: {ex.Message}");
            }
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var reader = new StreamReader(request.InputStream);
            var body = await reader.ReadToEndAsync();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Получен вебхук");
            
            await ParseSignozWebhook(body);
            
            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes("OK");
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки запроса: {ex.Message}");
        }
    }

    
    private async Task ParseSignozWebhook(string jsonBody)
    {
        try
        {
            var webhookData = JsonSerializer.Deserialize<SignozWebhookBody>(jsonBody);
            if (webhookData?.Alerts == null || webhookData.Alerts.Count == 0)
            {
                return;
            }
            foreach (var alert in webhookData.Alerts)
            {
                _messageBuilder = new MessagesBuilder(alert);
                await _telegramBot.SendMessagesAsync(_messageBuilder, CancellationToken.None);
                Console.WriteLine("────────────────────────────────────────");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка парсинга JSON: {ex.Message}");
            Console.WriteLine($"Сырые данные: {jsonBody}");
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener?.Stop();
        _listener?.Close();
        Console.WriteLine("Вебхук сервер остановлен");
    }
}