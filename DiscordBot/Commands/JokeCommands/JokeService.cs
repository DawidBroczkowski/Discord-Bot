using Discord.Interactions;
using DiscordBot.Commands.JokeCommands.Helpers;
using DiscordBot.Commands.JokeCommands.Helpers.Interfaces;
using DiscordBot.Commands.JokeCommands.Interfaces;
using DiscordBot.Commands.JokeCommands.Models;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static DiscordBot.Commands.JokeCommands.Models.Enums;

namespace DiscordBot.Commands.JokeCommands
{
    internal class JokeService : IJokeService
    {
        private IServiceProvider _serviceProvider;
        private IEmbedService _embedService;
        private SocketInteractionContext _context;
        private readonly IJokesGetter _jokesGetter;

        public JokeService()
        {
            _jokesGetter = new JokesGetter();
        }

        public void SetContext(SocketInteractionContext context)
        {
            _context = context;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendJokeAsync(JokeType type)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(_context);
            JokeModel joke;
            try
            {
                joke = await _jokesGetter.GetJokeAsync(type);
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("joke", ex.Message);
                return;
            }
            if (joke is null)
            {
                await _embedService.ReplyErrorAsync("joke", "Couldn't find a joke");
                return;
            }

            await _embedService.SendJokeAsync(joke);
        }
    }
}
