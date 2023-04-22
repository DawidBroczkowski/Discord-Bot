using DiscordBot.Commands.ImgurCommand.Models;

namespace DiscordBot.Commands.ImgurCommands.Helpers.Interfaces
{
    internal interface IImgurDeserializer
    {
        Task<Images> DeserializeImagesAsync(HttpResponseMessage response);
    }
}