using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.Digests;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using VneOcherediGuard.DataBase;
using VneOcherediGuard.Infrastructure;

namespace VneOcherediGuard.DataBaseServices
{
    public sealed class ChatServices
    {
        private IServiceProvider serviceProvider;
        private IServiceScopeFactory _serviceScopeFactory;
        public ChatServices(IServiceProvider provider, IServiceScopeFactory serviceScopeFactory)
        {
            serviceProvider = provider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task AddChatIdAsync(long chatId, string name, string role, string typeChat)
        {
            using var context = GetDbContext();

                var newChat = new ChatIds
                {
                    ChatId = chatId,
                    Name = $"@{name}",
                    Role = role,
                    TypeChat = typeChat
                };
            context.DbContext.ChatIds.Add(newChat);
            await context.DbContext.SaveChangesAsync();
        }

        public async Task<string> TakeTypeChat(long chatId)
        {
            using var context = GetDbContext();
            Console.WriteLine(chatId);
            var entity = await context.DbContext.ChatIds
                .SingleOrDefaultAsync(i => i.ChatId == chatId);
            if (entity is null)
            {
                throw new InvalidOperationException($"Нет такого пользователя");
            }
            else
            {
                return entity.TypeChat;
            }
        }

        public async Task<List<long>> GetAllChatIdsAsync()
        {
            using var context = GetDbContext();
            var chatIds = await context.DbContext.ChatIds
                .Select(chat => chat.ChatId) 
                .ToListAsync();
            return chatIds;
        }

        public async Task<Boolean> TryToFindChatId(long chatId)
        {
            using var context = GetDbContext();
            var chat = await context.DbContext.ChatIds
                .FirstOrDefaultAsync(c => c.ChatId == chatId);
            if (chat is null)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> TryToFindChatIdByName(string name)
        {
            using var context = GetDbContext();
            var chat = await context.DbContext.ChatIds
                .FirstOrDefaultAsync(c => c.Name == name);
            if (chat is null)
            {
                return false;
            }
            return true;
        }

        public async Task<string> TakeRole(long chatId)
        {
            using var context = GetDbContext();
            Console.WriteLine(chatId);
            var entity = await context.DbContext.ChatIds
                .SingleOrDefaultAsync(i => i.ChatId == chatId);
            if (entity is null)
            {
                throw new InvalidOperationException($"Нет такого пользователя");
            }
            else
            {
                return entity.Role;
            }
        }

        public async Task<long> FindChatId(string name, Task sendErorMessage)
        {
            using var context = GetDbContext();
            var entity = await context.DbContext.ChatIds
                .SingleOrDefaultAsync(c => c.Name == name);
            if (entity is null)
            {
                await sendErorMessage;
                throw new InvalidOperationException($"Запись с Name = '{name}' не найдена.");
            }
            else
            {
                return entity.ChatId;
            }
        }

        public async Task UpdateRole(string role, string name)
        {
            using var context = GetDbContext();
            var entity = await context.DbContext.ChatIds
                .SingleOrDefaultAsync(c => c.Name == name);
            if (entity is null)
            {
                throw new InvalidOperationException($"Запись с Name = '{name}' не найдена.");
            }
            entity.Role = role;
            await context.DbContext.SaveChangesAsync();
        }

        private ScopedDbContext GetDbContext()
        {
            var scope = _serviceScopeFactory.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return new ScopedDbContext { Scope = scope, DbContext = dbcontext };
        }
    }
}
