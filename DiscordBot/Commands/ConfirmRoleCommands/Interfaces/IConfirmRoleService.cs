using Discord;
using Discord.Interactions;

namespace DiscordBot.Commands.ConfirmRoleCommands.Interfaces
{
    internal interface IConfirmRoleService
    {
        Task DeleteConfirmRoleAsync();
        Task SetConfirmRoleAsync(IRole role);
        void SetContext(SocketInteractionContext context);
        void SetServiceProvider(IServiceProvider serviceProvider);
        Task SpawnConfirmButtonAsync(string? messageText, string buttonText);
    }
}