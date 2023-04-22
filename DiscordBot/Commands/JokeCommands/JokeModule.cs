using Discord.Interactions;
using DiscordBot.Commands.JokeCommands;
using DiscordBot.Commands.JokeCommands.Interfaces;
using static DiscordBot.Commands.JokeCommands.Models.Enums;

namespace DiscordBot.Commands
{
    public class JokeModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private IJokeService _jokeService;

        public JokeModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _jokeService = new JokeService();    
        }

        [CheckPermission(DataAccess.Enums.Commands.joke)]
        [SlashCommand("joke", "Sends a joke from JokesAPI")]
        public async Task GetJokeAsync([Summary(description: "Optional type of joke")]JokeType type = JokeType.any)
        {
            _jokeService.SetServiceProvider(_serviceProvider);
            _jokeService.SetContext(Context);
            await _jokeService.SendJokeAsync(type);
        }
    }
}