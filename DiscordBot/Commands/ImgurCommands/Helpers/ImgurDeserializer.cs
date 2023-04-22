using DiscordBot.Commands.ImgurCommand.Models;
using DiscordBot.Commands.ImgurCommands.Helpers.Interfaces;
using System.Text.Json;

namespace DiscordBot.Commands.ImgurCommands.Helpers
{
    internal class ImgurDeserializer : IImgurDeserializer
    {
        public async Task<Images> DeserializeImagesAsync(HttpResponseMessage response)
        {
            // Convert response to string.
            string jsonString = await response.Content.ReadAsStringAsync();

            // Deserialize JSON to images:
            Images images = JsonSerializer.Deserialize<Images>(jsonString);

            // Check if any images were found:
            if (images.data.Count == 0)
            {
                return null;
            }

            return images;
        }
    }
}
