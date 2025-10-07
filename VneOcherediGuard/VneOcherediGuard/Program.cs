using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Reflection;
using VneOcherediGuard;
using VneOcherediGuard.DataBaseServices;
using VneOcherediGuard.Infrastructure;
using VneOcherediGuard.Options;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddOptions<TelegramOptions>().BindConfiguration("Telegram");
        serviceCollection.AddOptions<WebhookOptions>().BindConfiguration("Webhook");
        serviceCollection.AddOptions<DataBaseOptions>().BindConfiguration("DataBase");
        serviceCollection.AddSingleton(x => x.GetRequiredService<IOptions<DataBaseOptions>>().Value);
        serviceCollection.AddDbContext<ApplicationDbContext>();
        serviceCollection.AddSingleton<TelegramBot>();
        serviceCollection.AddScoped<SimpleWebhook>();
        serviceCollection.AddSingleton<ChatServices>();
        serviceCollection.AddSingleton<MessagesServices>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        var cts = new CancellationTokenSource();
        var telegramBot = serviceProvider.GetRequiredService<TelegramBot>();

        telegramBot.Initialize(cts.Token);
        Console.WriteLine("Бот запущен...");

        try
        {
            foreach (Type x in Assembly.GetAssembly(typeof(Program))!.GetTypes().Where(x => !x.IsInterface))
            {
                if (!x.IsAssignableTo(typeof(ITelegramBotTask))) { continue; }

                var telegramBotTask = serviceProvider.GetRequiredService(x) as ITelegramBotTask;
                telegramBotTask?.StartAsync().ConfigureAwait(false);
            }


            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        await cts.CancelAsync();
        Console.WriteLine("Бот остановлен... ");
    }
}