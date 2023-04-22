using DiscordBot.Commands.RedditCommands.Helpers.Interfaces;
using DiscordBot.Commands.RedditCommands.Models;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;

namespace DiscordBot.Commands.RedditCommands.Helpers
{
    internal class RedditPostsGetter : IRedditPostsGetter
    {
        private IRedditCallHandler ?_redditCallHandler;
        private IParseService? _parseService;
        private IRedditDeserialzier? _redditDeserializer;
        private IServiceProvider? _serviceProvider;
        private UInt16 _number;

        public RedditPostsGetter()
        {
            _redditDeserializer = new RedditDeserialzier();
            _redditCallHandler = new RedditCallHandler();
        }

        public void SetNumber(ref UInt16 number)
        {
            _number = number;
        }
        
        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<PostModel>?>? GetPostsListAsync(string subreddit, RedditSort sort, RedditTime time)
        {
            Configure();
            HttpResponseMessage response = await _redditCallHandler!.GetPostsAsync(subreddit, sort, time, _number);
            if (response is null)
            {
                return null;
            }

            JsonNode redditNode = await _parseService!.ParseResponseAsync(response);
            if (redditNode is null)
            {
                return null;
            }

            List<PostModel> posts = _redditDeserializer!.DeserializePosts(redditNode, sort, ref _number);
            if (posts is null)
            {
                return null;
            }

            return posts;
        }
        
        public async Task<List<PostModel>?>? GetPostsListAsync(string subreddit, RedditRandomSort sort, RedditTime time)
        {
            Configure();
            HttpResponseMessage response = await _redditCallHandler!.GetPostsAsync(subreddit, sort, time, _number);
            if (response is null)
            {
                return null;
            }

            JsonNode redditNode = await _parseService!.ParseResponseAsync(response);
            if (redditNode is null)
            {
                return null;
            }

            List<PostModel> posts = _redditDeserializer!.DeserializePosts(redditNode, ref _number);
            if (posts is null)
            {
                return null;
            }

            return posts;
        }

        public async Task<List<PostModel>?>? GetBestPostsListAsync(string subreddit)
        {
            Configure();
            HttpResponseMessage response = await _redditCallHandler!.GetBestPostsAsync(subreddit, _number);
            if (response is null)
            {
                return null;
            }

            JsonNode redditNode = await _parseService!.ParseResponseAsync(response);
            if (redditNode is null)
            {
                return null;
            }

            List<PostModel> posts = _redditDeserializer!.DeserializePosts(redditNode, RedditSort.top, ref _number);
            if (posts is null)
            {
                return null;
            }

            return posts;
        }

        private void Configure()
        {
            _parseService = _serviceProvider!.GetRequiredService<IParseService>();
        }
    }
}
