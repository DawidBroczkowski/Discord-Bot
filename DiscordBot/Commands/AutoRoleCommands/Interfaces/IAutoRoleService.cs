using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Commands.AutoRoleCommands.Interfaces
{
    public interface IAutoRoleService
    {
        Task DeleteAutoRoleAsync();
        Task SetAutoRoleAsync(IRole role);
        void SetContext(SocketInteractionContext context);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}