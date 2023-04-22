namespace DiscordBot.Commands.MusicCommands.Interfaces
{
    public interface IAudioService
    {
        HashSet<ulong>? VoteQueue { get; set; }
        void Configure(IServiceProvider serviceProvider);
    }
}