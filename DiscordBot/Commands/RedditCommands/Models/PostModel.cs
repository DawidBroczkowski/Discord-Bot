namespace DiscordBot.Commands.RedditCommands.Models
{
#pragma warning disable CS8618
    public record PostModel
    {
        public string Title { get; set; }
        public string? Content { get; set; }
        public string Author { get; init; } 
        public string Subreddit { get; init; }
        public int Score { get; init; }
        public bool Nsfw { get; init; }
        public string? ImageUrl { get; init; }
        public string Link { get; init; }
        public double Date { get; init; }
    }
}
