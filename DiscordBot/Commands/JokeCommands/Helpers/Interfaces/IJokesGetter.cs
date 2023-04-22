using DiscordBot.Commands.JokeCommands.Models;

namespace DiscordBot.Commands.JokeCommands.Helpers.Interfaces
{
    internal interface IJokesGetter
    {
        Task<JokeModel> GetJokeAsync(Enums.JokeType type);
    }
}