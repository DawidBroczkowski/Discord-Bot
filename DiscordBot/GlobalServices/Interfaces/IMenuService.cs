using Discord.WebSocket;

namespace DiscordBot.GlobalServices.Interfaces
{
    public interface IMenuService
    {
        void Configure(SocketMessageComponent arg, DiscordSocketClient client);
        Task SelfRoleAddSelectionMenuAsync();
        Task SelfRoleRemoveSelectionMenuAsync();
    }
}