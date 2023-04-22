namespace DiscordBot.Entities;

public record Config
{
#pragma warning disable CS8618
    public string Prefix { get; set; }
    public string Token { get; init; }
    public string WeatherApiToken { get; init; }
    public string ImgurClientId { get; init; }
    public int MusicBotVoteTreshold { get; init; }
    public string LavaHostname { get; init; }
    public string LavaAuthorization { get; init; }
    public ushort LavaPort { get; init; }
}
