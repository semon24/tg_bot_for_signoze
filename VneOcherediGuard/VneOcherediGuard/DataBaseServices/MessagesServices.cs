using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql.Replication.PgOutput.Messages;
using Org.BouncyCastle.Crypto.Digests;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bots.Types;
using VneOcherediGuard.Infrastructure;
using VneOcherediGuard.Infrastructure.DataBase;

namespace VneOcherediGuard.DataBaseServices
{
    public sealed class MessagesServices
    {
        private IServiceProvider serviceProvider;
        private IServiceScopeFactory _serviceScopeFactory;
        public MessagesServices(IServiceProvider provider, IServiceScopeFactory serviceScopeFactory)
        {
            serviceProvider = provider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task AddMessageAsync(long chatId, string typeMessage, string deploymentEnvironment)
        {
            using var context = GetDbContext();
            var newMessage = new MessagesTime
            {
                ChatId = chatId,
                TypeMessage = typeMessage,
                Time = DateTime.UtcNow,
                DeploymentEnvironment = deploymentEnvironment
            };
           
            context.DbContext.MessagesTime.Add(newMessage);
            try
            {
                await context.DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Ошибка базы данных: {ex.InnerException?.Message}");
                throw;
            }
        }   

        public async Task DeleteMessageAsync(long chatId, string partOfMessage, string deploymentEnvironment)
        {
            using var context = GetDbContext();
            var message = await context.DbContext.MessagesTime
                .FirstOrDefaultAsync(m => m.ChatId == chatId
                && m.TypeMessage.Contains(partOfMessage)
                && m.DeploymentEnvironment == deploymentEnvironment);
            Console.WriteLine("удаляем");
            if (message is not null)
            {
                context.DbContext.MessagesTime.Remove(message);
                await context.DbContext.SaveChangesAsync();
            }

            else
            {
                throw new InvalidOperationException($"Нет такого сообщения");
            }
        }

        public async Task<bool> CheckIsNotMessageInTable(long chatId, string partOfMessage, string deploymentEnvironment)
        {
            using var context = GetDbContext();
            var entity = await context.DbContext.MessagesTime
                .SingleOrDefaultAsync(c => c.ChatId == chatId 
                && c.TypeMessage.Contains(partOfMessage) 
                && c.DeploymentEnvironment == deploymentEnvironment);
            if (entity is not null)
            {
                return false;
            }
            return true;
        }

        public async Task<string> TakeProcentFromMessage(long chatId, string partOfMessage, string deploymentEnvironment)
        {
            using var context = GetDbContext();
            var entity = await context.DbContext.MessagesTime
                .SingleOrDefaultAsync(c => c.ChatId == chatId 
                && c.TypeMessage.Contains(partOfMessage) 
                && c.DeploymentEnvironment == deploymentEnvironment);
            if (entity is null)
            {
                throw new InvalidOperationException($"Нет такого сообщения");
            }
            var percentIndex = entity.TypeMessage.IndexOf('%');
            var spaceIndex = entity.TypeMessage.LastIndexOf(' ', percentIndex - 1);
            return entity.TypeMessage.Substring(spaceIndex + 1, percentIndex - (spaceIndex + 1));
        }

        public async Task<DateTimeOffset> TakeTimeFromMessage(long chatId, string partOfMessage, string deploymentEnvironment)
        {
            using var context = GetDbContext();
            var entity = await context.DbContext.MessagesTime
                .SingleOrDefaultAsync(c => c.ChatId == chatId 
                && c.TypeMessage.Contains(partOfMessage)
                && c.DeploymentEnvironment == deploymentEnvironment);
            Console.WriteLine($"ищем время {partOfMessage}");
            if (entity is null)
            {
                throw new InvalidOperationException($"Нет такого сообщения");
            }
            return entity.Time;
        }

        private ScopedDbContext GetDbContext()
        {
            var scope = _serviceScopeFactory.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return new ScopedDbContext { Scope = scope, DbContext = dbcontext };
        }
    }
}
