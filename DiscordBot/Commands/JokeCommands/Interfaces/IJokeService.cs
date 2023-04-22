using Discord.Interactions;
using DiscordBot.Commands.JokeCommands.Models;

namespace DiscordBot.Commands.JokeCommands.Interfaces
{
    internal interface IJokeService
    {
        Task SendJokeAsync(Enums.JokeType type);
        void SetContext(SocketInteractionContext context);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}