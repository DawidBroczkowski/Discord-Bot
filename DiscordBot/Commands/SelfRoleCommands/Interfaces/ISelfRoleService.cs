using Discord;
using Discord.Interactions;

namespace DiscordBot.Commands.SelfRoleCommands.Interfaces
{
    public interface ISelfRoleService
    {
        Task AddSelfRoleAsync(IRole role);
        Task RemoveSelfRoleAsync(IRole role);
        void SetContext(SocketInteractionContext context);
        void SetServiceProvider(IServiceProvider serviceProvider);
        Task SpawnAddSelectionMenuAsync();
        Task SpawnRemoveSelectionMenuAsync();
    }
}