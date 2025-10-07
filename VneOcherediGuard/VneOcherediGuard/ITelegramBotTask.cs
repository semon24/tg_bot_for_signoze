namespace VneOcherediGuard;

public interface ITelegramBotTask
{
    Task StartAsync();
    void Stop();
}