using Discord.WebSocket;
using DiscordBot.DataAccess.Repositories.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.GlobalServices
{
    public class ButtonService : IButtonService
    {
        private SocketMessageComponent? _arg;
        private DiscordSocketClient? _client;
        private IServiceProvider? _services;
        private IServerConfigRepository? _serverConfig;

        public void Configure(IServiceProvider services, SocketMessageComponent arg, DiscordSocketClient client)
        {
            _arg = arg;
            _client = client;
            _services = services;
            _serverConfig = _services.GetRequiredService<IServerConfigRepository>();
        }

        public async Task ConfirmRoleButtonAsync()
        {
            ulong guildId = _arg!.GuildId!.Value;
            var guild = _client!.GetGuild(guildId);
            var user = guild.Users.First(x => x.Id == _arg.User.Id);
            var roleId = await _serverConfig!.GetConfirmRoleIdAsync(guildId);
            if (roleId is null)
            {
                return;
            }

            var role = guild.Roles.FirstOrDefault(x => x.Id == roleId);
            if (role is not null)
            {
                await user.AddRoleAsync(role);
            }
        }
    }
}
