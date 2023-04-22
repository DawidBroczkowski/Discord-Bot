using DiscordBot.Commands.ImgurCommand.Models;

namespace DiscordBot.Commands.ImgurCommands.Helpers.Interfaces
{
    internal interface IImgurHelper
    {
        List<ImagesData> ExtractGalleryImagesAsync(Images images);
        List<ImagesData> GetFilteredGifs(List<ImagesData> imagesList);
        List<ImagesData> GetFilteredImages(List<ImagesData> imagesList);
    }
}