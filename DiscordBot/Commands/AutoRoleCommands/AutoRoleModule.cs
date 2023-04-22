using Discord;
using Discord.Interactions;
using DiscordBot.Commands.AutoRoleCommands.Interfaces;
using DiscordBot.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.AutoRoleCommands
{
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    [Group("autorole", "Management of automatically assigned roles.")]
    public class AutoRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private IAutoRoleService _autoRoleService;

        public AutoRoleModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _autoRoleService = new AutoRoleService();
        }

        [SlashCommand("set", "Sets a role to be automatically given to a user entering the server.")]
        public async Task SetAutoRoleAsync([Summary(description: "The role you want to mark as an auto-role.")] IRole role)
        {
            Configure();
            await _autoRoleService.SetAutoRoleAsync(role);
        }

        [SlashCommand("delete", "Turns off the autorole feature.")]
        public async Task DeleteAutoRoleAsync()
        {
            Configure();
            await _autoRoleService.DeleteAutoRoleAsync();
        }

        private void Configure()
        {
            _autoRoleService.SetContext(Context);
            _autoRoleService.SetServiceProvider(_serviceProvider);
        }
    }
}
