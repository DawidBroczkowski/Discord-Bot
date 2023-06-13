using Discord;
using Discord.Interactions;
using DiscordBot.Commands.AutoRoleCommands.Interfaces;
using DiscordBot.DataAccess.Repositories.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using DiscordBot.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.AutoRoleCommands
{
    public class AutoRoleService : IAutoRoleService
    {
        private IServiceProvider _serviceProvider;
        private IServerConfigRepository _serverConfig;
        private SocketInteractionContext _context;
        private IEmbedService _embedService;

        public AutoRoleService(IServiceProvider serviceProvider, SocketInteractionContext context)
        {
            _serviceProvider = serviceProvider;
            _serverConfig = _serviceProvider.GetRequiredService<IServerConfigRepository>();
            _context = context;
        }

        public AutoRoleService()
        {
        }

        public void SetContext(SocketInteractionContext context)
        {
            _context = context;
        }
    
        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SetAutoRoleAsync(IRole role)
        {
            Configure();
            try
            {
                await _serverConfig.SetAutoRoleAsync(_context.Guild.Id, role.Id);
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("autorole set", ex.Message);
                return;
            }
            await _embedService.ReplySuccessAsync("autorole set", $"Successfully marked \"{role.Name}\" as an autorole.");
        }

        public async Task DeleteAutoRoleAsync()
        {
            Configure();
            try
            {
                await _serverConfig.SetAutoRoleAsync(_context.Guild.Id, null);
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("autorole delete", ex.Message);
                return;
            }
            await _embedService.ReplySuccessAsync("autorole delete", $"Successfully turned off the autorole feature.");
        }

        private void Configure()
        {
            _serverConfig = _serviceProvider.GetRequiredService<IServerConfigRepository>();
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(_context);
        }
    }
}
