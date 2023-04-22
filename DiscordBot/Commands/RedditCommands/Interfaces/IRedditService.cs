using Discord.Interactions;
using DiscordBot.Commands.RedditCommands.Models;

namespace DiscordBot.Commands.RedditCommands.Interfaces
{
    internal interface IRedditService
    {
        Task SendBestRedditPostAsync(string subreddit, bool nsfw);
        Task SendRandomRedditPostAsync(string subreddit, RedditRandomSort sort, RedditTime time, bool nsfw);
        Task SendRedditPostAsync(string subreddit, RedditSort sort, RedditTime time, bool nsfw);
        void SetContext(SocketInteractionContext context);
        void SetNumber(ushort number);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}