namespace DiscordBot.Commands.ImgurCommand.Models
{
#pragma warning disable CS8618
    public record ImagesGalleryData
    {
        public string link { get; init; }
        public string type { get; init; }
        public int views { get; init; }
        public int datetime { get; init; }
        public string id { get; init; }
    }
}
