using Discord.WebSocket;

namespace DiscordBot.GlobalServices.Interfaces
{
    public interface IButtonService
    {
        void Configure(IServiceProvider services, SocketMessageComponent arg, DiscordSocketClient client);
        Task ConfirmRoleButtonAsync();
    }
}