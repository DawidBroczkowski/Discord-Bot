using DiscordBot.Commands.RedditCommands.Models;

namespace DiscordBot.Commands.RedditCommands.Helpers.Interfaces
{
    internal interface IRedditPostsGetter
    {
        Task<List<PostModel>?>? GetBestPostsListAsync(string subreddit);
        Task<List<PostModel>?>? GetPostsListAsync(string subreddit, RedditRandomSort sort, RedditTime time);
        Task<List<PostModel>?>? GetPostsListAsync(string subreddit, RedditSort sort, RedditTime time);
        void SetNumber(ref ushort number);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}