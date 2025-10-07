using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using VneOcherediGuard.DataBaseServices;
using VneOcherediGuard.Diologs;
using VneOcherediGuard.Options;
using static VneOcherediGuard.Extensions.MarkdownExtensions;


namespace VneOcherediGuard;

public sealed class TelegramBot
{
    private readonly IOptionsMonitor<TelegramOptions> _telegramOptions;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ChatServices _chatServices;
    private readonly MessagesServices _messagesServices;
    private bool _initialize;
    private readonly Dictionary<long, ConversationStateUpdateRole> _userStates = new();
    private TimeSpan filterForFiveProcent = TimeSpan.FromMinutes(10);
    private TimeSpan filterForTenProcent = TimeSpan.FromHours(1);
    private TimeSpan filterForTwentyProcent = TimeSpan.FromDays(1);


    public TelegramBot(IOptionsMonitor<TelegramOptions> telegramOptions, ChatServices chatServices, MessagesServices messagesServices)
    {
        _telegramOptions = telegramOptions;
        _telegramBotClient = new TelegramBotClient(_telegramOptions.CurrentValue.Token);
        _chatServices = chatServices;
        _messagesServices = messagesServices;
    }

    public void Initialize(CancellationToken cancellationToken)
    {
        _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: cancellationToken);
        _initialize = true;
    }
    
    public async Task TargetSendMessagesAsync(string message, long chatId, CancellationToken cancellationToken)
    {
        if (!_initialize)
        {
            Console.WriteLine("Для отправки сообщений необходимо вызвать метод Initialize");
            return;
        }

        await _telegramBotClient.SendTextMessageAsync(chatId, TextSanitize(message)
                    , parseMode: ParseMode.MarkdownV2
                    , disableWebPagePreview: true
                    , cancellationToken: cancellationToken);

    }

    public async Task SendMessagesAsync(MessagesBuilder messageBuilder, CancellationToken cancellationToken)
    {
        if (!_initialize)
        {
            Console.WriteLine("Для отправки сообщений необходимо вызвать метод Initialize");
            return ;
        }
        
        var chatIds = await _chatServices.GetAllChatIdsAsync();
        var message = messageBuilder.message.ToString();
        var nameMessage = messageBuilder.name;
        var filteredByTime = messageBuilder.filterByTime;
        var isSendGroup = messageBuilder.sendInGroup;
        var deploymentEnvironment = messageBuilder.deploymentEnvironment;
        var thresholdAlelrtValue = messageBuilder.thresholdAlelrtValue;

        foreach (var chatId in chatIds)
        {
            if (await _chatServices.TakeRole(chatId) != "None")
            {
                try
                {
                    await _telegramBotClient.SendTextMessageAsync(chatId
                        , message
                        , parseMode: ParseMode.MarkdownV2
                        , disableWebPagePreview: true
                        , cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке в чат (Id: {chatId}) сообщения: {ex.Message}");
                }
            }

            if (await _chatServices.TakeTypeChat(chatId) == "Group")
            {
                if (nameMessage is not null
                    && filteredByTime && isSendGroup)
                {
                    Console.WriteLine("Пытаемся отправить сообщение в группу с фильтром по времени");
                    await SendMessageWithFilterAsync(nameMessage, message, chatId, deploymentEnvironment, thresholdAlelrtValue, cancellationToken);
                }

                else if (nameMessage is not null
                    && isSendGroup)
                {
                    Console.WriteLine("Пытаемся отправить сообщение в группу без фильтра по времени");
                    await TrySendMessage(chatId, message, cancellationToken);
                }
            }
        }
    }
    

    private async Task TrySendMessage(long chatId, string message, CancellationToken cancellationToken)
    {
        try
        {
            await _telegramBotClient.SendTextMessageAsync(chatId, message
                , parseMode: ParseMode.MarkdownV2
                , disableWebPagePreview: true
                , cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке в чат (Id: {chatId}) сообщения: {ex.Message}");
        }
    }

    private async Task SendMessageWithFilterAsync(string nameMessage, 
        string message, long chatId, 
        string deploymentEnvironment, double thresholdAlelrtValue, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine(deploymentEnvironment);
        async Task UpdateDataMessage()
        {
            await _messagesServices.DeleteMessageAsync(chatId, "свободного места на диске", deploymentEnvironment);
            await _messagesServices.AddMessageAsync(chatId, nameMessage, deploymentEnvironment);
            await TrySendMessage(chatId, message, cancellationToken);
        }

        if (await _messagesServices.CheckIsNotMessageInTable(chatId, "свободного места на диске", deploymentEnvironment))
        {
            Console.WriteLine("Нет параметра в таблцие - автом. добавляю");
            await _messagesServices.AddMessageAsync(chatId, nameMessage, deploymentEnvironment);
            await TrySendMessage(chatId, message, cancellationToken);
        }

        else
        {
            Console.WriteLine("Такой alert уже есть в таблице, проверяю...");

            var lastMesssagewTime = await _messagesServices.TakeTimeFromMessage(chatId, "свободного места на диске", deploymentEnvironment);
            var differenceTime = DateTime.UtcNow - lastMesssagewTime;
            var procentLastAlertMemory = await _messagesServices.TakeProcentFromMessage(chatId, "свободного места на диске", deploymentEnvironment);

            Console.WriteLine("Пытаемся переписать значение");
            Console.WriteLine($"{differenceTime} - разница");
            Console.WriteLine($"{lastMesssagewTime} - последнее время");
            Console.WriteLine($"{DateTime.UtcNow} - текущее время");

            if (thresholdAlelrtValue == 0.8 && (differenceTime >= filterForTwentyProcent || procentLastAlertMemory != "20"))
            {
                await UpdateDataMessage();
            }

            else if (thresholdAlelrtValue == 0.9 && (differenceTime >= filterForTenProcent || procentLastAlertMemory != "10"))
            {
                await UpdateDataMessage();
            }

            else if (thresholdAlelrtValue == 0.95 && (differenceTime >= filterForFiveProcent || procentLastAlertMemory != "5"))
            {
                await UpdateDataMessage();
            }
        }
    }

    private async Task TryToAddNewChatId(long chatId,string username, string role, string typeChat)
    {
        var checkChatId = await _chatServices.TryToFindChatId(chatId);
        if (!checkChatId)
        {
            await _chatServices.AddChatIdAsync(chatId, username, role, typeChat);
            Console.WriteLine($"Добавлен новый ChatId: {chatId}");
        }
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message.Text != null)
        {
            var chatId = update.Message.Chat.Id;
            var user = update.Message.From;
            var chat = update.Message.Chat;
            var messageText = update.Message.Text;
            if (messageText.StartsWith("/update_role"))
            {
                if (await _chatServices.TakeRole(chatId) == "Admin")
                {
                    var state = new ConversationStateUpdateRole { CurrentHandler = "awaiting_username" };
                    _userStates[chatId] = state;
                    await TargetSendMessagesAsync("Введите имя пользователя:", chatId, cancellationToken: cancellationToken);
                }

                else
                {
                    await TargetSendMessagesAsync("Недостаточно прав!", chatId, cancellationToken: cancellationToken);
                }
            }

            else if (_userStates.ContainsKey(chatId) && _userStates[chatId].CurrentHandler != "")
            {
                switch (_userStates[chatId].CurrentHandler)
                {
                    case "awaiting_username":
                        if (await _chatServices.TryToFindChatIdByName(messageText))
                        {
                            _userStates[chatId].UserName = messageText;
                            _userStates[chatId].CurrentHandler = "awaiting_role";
                            await TargetSendMessagesAsync("Введите роль: Admin | User | None", chatId, cancellationToken: cancellationToken);
                        }
                        else
                        {
                            await TargetSendMessagesAsync("Неверное имя пользователя!", chatId, cancellationToken: cancellationToken);
                        }
                        break;

                    case "awaiting_role":
                        if (messageText != "Admin" && messageText != "User" && messageText != "None")
                        {
                            await TargetSendMessagesAsync("Неверная роль!", chatId, cancellationToken: cancellationToken);
                        }
                        else
                        {
                            _userStates[chatId].NewRole = messageText;
                            await _chatServices.UpdateRole(_userStates[chatId].NewRole, _userStates[chatId].UserName);
                            _userStates.Remove(chatId);
                            await TargetSendMessagesAsync("Роль успешно обновлена!", chatId, cancellationToken: cancellationToken);
                        }
                        break;
                }
            }

            if (chat.Title is null)
            {
                string username = "не указан";
                if (user is not null && !string.IsNullOrEmpty(user.Username))
                {
                    username = user.Username;
                }
                Console.WriteLine($"Group ChatId: {chatId} {username}");
                await TryToAddNewChatId(chatId, username, "None", "Private");
            }
            else
            {
                await TryToAddNewChatId(chatId, chat.Title, "None", "Group");
            }
        }
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}