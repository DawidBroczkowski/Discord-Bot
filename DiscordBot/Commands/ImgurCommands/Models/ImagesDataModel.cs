namespace DiscordBot.Commands.ImgurCommand.Models
{
#pragma warning disable CS8618
    public record ImagesData
    {
        public string link { get; init; }
        public string type { get; init; }
        public bool nsfw { get; init; }
        public string title { get; init; }
        public string account_url { get; init; }
        public int views { get; init; }
        public int datetime { get; init; }
        public string id { get; init; }
        public bool in_gallery { get; init; }
        public List<ImagesGalleryData> images { get; set; }
    }
}
