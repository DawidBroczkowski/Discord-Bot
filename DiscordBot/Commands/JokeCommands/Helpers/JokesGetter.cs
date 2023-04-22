using DiscordBot.Commands.JokeCommands.Models;
using static DiscordBot.Commands.JokeCommands.Models.Enums;
using DiscordBot.Commands.JokeCommands.Helpers.Interfaces;

namespace DiscordBot.Commands.JokeCommands.Helpers
{


    internal class JokesGetter : IJokesGetter
    {
        private readonly IJokesCallHandler _jokesCallHandler;
        private readonly IJokesDeserializer _jokesDeserializer;

        public JokesGetter()
        {
            _jokesCallHandler = new JokesCallHandler();
            _jokesDeserializer = new JokesDeserializer();
        }

        public async Task<JokeModel> GetJokeAsync(JokeType type)
        {
            _jokesCallHandler.ConfigureClient();

            HttpResponseMessage response = await _jokesCallHandler.GetJokeAsync(type);

            JokeModel joke = await _jokesDeserializer!.DeserializeJokeAsync(response)!;

            return joke;
        }

    }
}
