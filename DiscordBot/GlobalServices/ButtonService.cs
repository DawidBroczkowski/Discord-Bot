using Discord.WebSocket;
using DiscordBot.DataAccess.Repositories.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.GlobalServices
{
    public class ButtonService : IButtonService
    {
        private SocketMessageComponent? _arg;
        private DiscordSocketClient? _client;
        private IServiceProvider? _services;
        private IServerConfigRepository? _serverConfig;
        private HubConnection? _connection;

        public void Configure(IServiceProvider services, SocketMessageComponent arg, DiscordSocketClient client)
        {
            _arg = arg;
            _client = client;
            _services = services;
            _serverConfig = _services.GetRequiredService<IServerConfigRepository>();

            _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7029/chatHub")
            .WithAutomaticReconnect()
            .Build();
            _connection.StartAsync();
        }

        public async Task ConfirmRoleButtonAsync()
        {
            ulong guildId = _arg!.GuildId!.Value;
            var guild = _client!.GetGuild(guildId);
            var user = guild.Users.First(x => x.Id == _arg.User.Id);
            var roleId = await _serverConfig!.GetConfirmRoleIdAsync(guildId);
            if (roleId is null)
            {
                await _arg.ModifyOriginalResponseAsync(x => x.Content = "Confirm-role has been turned off");
                return;
            }

            var role = guild.Roles.FirstOrDefault(x => x.Id == roleId);
            if (role is null)
            {
                await _arg.ModifyOriginalResponseAsync(x => x.Content = "Role no longer exists");
                return;
            }
            await user.AddRoleAsync(role);
            await _arg.ModifyOriginalResponseAsync(x => x.Content = "Success");
        }
    }
}
