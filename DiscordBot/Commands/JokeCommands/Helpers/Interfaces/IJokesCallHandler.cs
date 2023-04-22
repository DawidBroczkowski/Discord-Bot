using DiscordBot.Commands.JokeCommands.Models;

namespace DiscordBot.Commands.JokeCommands.Helpers.Interfaces
{
    internal interface IJokesCallHandler
    {
        void ConfigureClient();
        Task<HttpResponseMessage> GetJokeAsync(Enums.JokeType type);
    }
}