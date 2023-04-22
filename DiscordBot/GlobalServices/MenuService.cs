using Discord.WebSocket;
using DiscordBot.GlobalServices.Interfaces;

namespace DiscordBot.GlobalServices
{
    public class MenuService : IMenuService
    {
        private SocketMessageComponent? _arg;
        private DiscordSocketClient? _client;

        public void Configure(SocketMessageComponent arg, DiscordSocketClient client)
        {
            _arg = arg;
            _client = client;
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
            }
        }
    }
}
