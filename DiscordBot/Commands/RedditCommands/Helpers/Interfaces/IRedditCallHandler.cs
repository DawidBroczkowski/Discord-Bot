using DiscordBot.Commands.RedditCommands.Models;

namespace DiscordBot.Commands.RedditCommands.Helpers.Interfaces
{
    internal interface IRedditCallHandler
    {
        Task<HttpResponseMessage> GetBestPostsAsync(string subreddit, ushort number);
        Task<HttpResponseMessage> GetPostsAsync(string subreddit, RedditRandomSort sort, RedditTime time, ushort number);
        Task<HttpResponseMessage> GetPostsAsync(string subreddit, RedditSort sort, RedditTime time, ushort number);
    }
}