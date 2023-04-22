namespace DiscordBot.Commands.JokeCommands.Models
{
#pragma warning disable CS8618
    public record JokeModel
    {
        public string type { get; init; }
        public string setup { get; init; }
        public string delivery { get; init; }
        public string joke { get; init; }
    }
}
