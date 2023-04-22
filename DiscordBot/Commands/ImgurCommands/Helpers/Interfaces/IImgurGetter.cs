using DiscordBot.Commands.ImgurCommand.Models;

namespace DiscordBot.Commands.ImgurCommands.Helpers.Interfaces
{
    internal interface IImgurGetter
    {
        Task<List<ImagesData>> GetGifsListAsync(string content);
        Task<List<ImagesData>> GetImagesListAsync(string content);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}