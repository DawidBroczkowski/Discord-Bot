using Discord.WebSocket;

namespace DiscordBot
{

    class Applicaton
    {
        private DiscordSocketClient? _client;
        public static void Main(string[] args) => new Applicaton().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            _client = new DiscordSocketClient();
            // _commands = new();

            CommandHandler CH = new(_client /*, _commands*/);
            await CH.InstallCommandsAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }  
}