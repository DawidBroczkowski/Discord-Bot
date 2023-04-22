using DiscordBot.Commands.RedditCommands.Helpers.Interfaces;
using DiscordBot.Commands.RedditCommands.Models;

namespace DiscordBot.Commands.RedditCommands.Helpers
{
    internal class RedditCallHandler : IRedditCallHandler
    {
        private HttpClient _httpClient;

        public RedditCallHandler()
        {
            _httpClient = new();
        }

        public async Task<HttpResponseMessage> GetPostsAsync(string subreddit, RedditSort sort, RedditTime time, UInt16 number)
        {
            string link = $"https://www.reddit.com/r/{subreddit}/{sort}.json?t={time}&limit={number}";
            var response = await _httpClient.GetAsync(link);
            response.EnsureSuccessStatusCode();
            return response;
        }
        public async Task<HttpResponseMessage> GetPostsAsync(string subreddit, RedditRandomSort sort, RedditTime time, UInt16 number)
        {
            string link = $"https://www.reddit.com/r/{subreddit}/{sort}.json?t={time}&limit={number}";
            var response = await _httpClient.GetAsync(link);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> GetBestPostsAsync(string subreddit, UInt16 number)
        {
            string link = $"https://www.reddit.com/r/{subreddit}/best.json?limit={number}";
            var response = await _httpClient.GetAsync(link);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
