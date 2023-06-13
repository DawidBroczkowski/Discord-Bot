using Discord.WebSocket;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace DiscordBot.GlobalServices
{
    public class MenuService : IMenuService
    {
        private SocketMessageComponent? _arg;
        private DiscordSocketClient? _client;
        private HubConnection _connection;

        public void Configure(SocketMessageComponent arg, DiscordSocketClient client)
        {
            _arg = arg;
            _client = client;
            _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7029/chatHub")
            .WithAutomaticReconnect()
            .Build();
            _connection.StartAsync();
        }

        public async Task SelfRoleAddSelectionMenuAsync()
        {
            ulong guildId = _arg!.GuildId!.Value;
            var guild = _client!.GetGuild(guildId);
            var user = guild.Users.First(x => x.Id == _arg.User.Id);
            var roleId = ulong.Parse(_arg.Data.Values.First());
            var role = guild.Roles.FirstOrDefault(x => x.Id == roleId);
            if (role is not null)
            {
                await user.AddRoleAsync(role);
                try
                {
                    await _connection.InvokeAsync("selfrole add", "Bot", $"Success: {role.Name} marked as a self-role", guildId.ToString());
                }
                catch { }
            }
        }

        public async Task SelfRoleRemoveSelectionMenuAsync()
        {
            ulong guildId = _arg!.GuildId!.Value;
            var guild = _client!.GetGuild(guildId);
            var user = guild.Users.First(x => x.Id == _arg.User.Id);
            var roleId = ulong.Parse(_arg.Data.Values.First());
            var role = guild.Roles.FirstOrDefault(x => x.Id == roleId);
            if (role is not null)
            {
                await user.RemoveRoleAsync(role);
                try
                {
                    await _connection.InvokeAsync("selfrole remove", "Bot", $"Success: {role.Name} unmarked as a self-role", guildId.ToString());
                }
                catch { }
            }
        }
    }
}
