using Discord;
using Discord.Interactions;
using DiscordBot.Commands.ConfirmRoleCommands.Interfaces;
using DiscordBot.DataAccess.Repositories.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.ConfirmRoleCommands
{
    internal class ConfirmRoleService : IConfirmRoleService
    {
        private IServiceProvider _serviceProvider;
        private IServerConfigRepository _serverConfig;
        private SocketInteractionContext _context;
        private IEmbedService _embedService;

        public ConfirmRoleService(IServiceProvider serviceProvider, SocketInteractionContext context)
        {
            _serviceProvider = serviceProvider;
            _serverConfig = _serviceProvider.GetRequiredService<IServerConfigRepository>();
            _context = context;
        }

        public ConfirmRoleService()
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

        public async Task SetConfirmRoleAsync(IRole role)
        {
            Configure();
            try
            {
                await _serverConfig.SetConfirmRoleAsync(_context.Guild.Id, role.Id);
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("confirmrole set", ex.Message);
                return;
            }
            await _embedService.ReplySuccessAsync("confirmrole set", $"Successfully marked \"{role.Name}\" as an confirmrole.");
        }

        public async Task DeleteConfirmRoleAsync()
        {
            Configure();
            try
            {
                await _serverConfig.SetConfirmRoleAsync(_context.Guild.Id, null);
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("confirmrole delete", ex.Message);
                return;
            }
            await _embedService.ReplySuccessAsync("confirmrole delete", $"Successfully turned off the confirmrole feature.");
        }

        public async Task SpawnConfirmButtonAsync(string? messageText, string buttonText)
        {
            var builder = new ComponentBuilder().WithButton(buttonText, "confirmbutton", style: ButtonStyle.Success);
            await _context.Interaction.FollowupAsync(messageText, components: builder.Build());
        }

        private void Configure()
        {
            _serverConfig = _serviceProvider.GetRequiredService<IServerConfigRepository>();
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(_context);
        }
    }
}
