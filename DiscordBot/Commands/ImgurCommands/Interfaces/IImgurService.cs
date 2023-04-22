using Discord.Interactions;

namespace DiscordBot.Commands.ImgurCommands.Interfaces
{
    internal interface IImgurService
    {
        Task SendRandomGifAsync(string content);
        Task SendRandomImageAsync(string content);
        Task SendTopGifAsync(string content);
        Task SendTopImageAsync(string content);
        void SetContext(SocketInteractionContext context);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}