using Discord;
using Discord.Interactions;
using DiscordBot.Commands.ConfirmRoleCommands.Interfaces;

namespace DiscordBot.Commands.ConfirmRoleCommands
{
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    [Group("confirmrole", "Management of a role assigned on clicking a button, e.g. confirm reading rules.")]
    public class ConfirmRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private IConfirmRoleService _confirmRoleService;

        public ConfirmRoleModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _confirmRoleService = new ConfirmRoleService();
        }

        [SlashCommand("set", "Sets a role to be to be given after clicking the button.")]
        public async Task SetConfirmRoleAsync([Summary(description: "The role you want to mark as an auto-role.")] IRole role)
        {
            Configure();
            await _confirmRoleService.SetConfirmRoleAsync(role);
        }

        [SlashCommand("delete", "Turns off the confirmrole feature.")]
        public async Task DeleteConfirmRoleAsync()
        {
            Configure();
            await _confirmRoleService.DeleteConfirmRoleAsync();
        }

        [SlashCommand("spawn", "Creates the button.")]
        public async Task SpawnAddSelectionMenuAsync(
            [Summary(description: "Text contained in the message.")] string? messageText,
            [Summary(description: "Text contained in the button.")] string buttonText)
        {
            Configure();
            await _confirmRoleService.SpawnConfirmButtonAsync(messageText, buttonText);
        }

        private void Configure()
        {
            _confirmRoleService.SetContext(Context);
            _confirmRoleService.SetServiceProvider(_serviceProvider);
        }
    }
}
