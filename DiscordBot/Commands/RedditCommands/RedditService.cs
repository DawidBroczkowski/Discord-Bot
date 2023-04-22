using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Commands.RedditCommands.Models;
using DiscordBot.Commands.RedditCommands.Helpers;
using DiscordBot.GlobalServices.Interfaces;
using DiscordBot.Commands.RedditCommands.Helpers.Interfaces;
using DiscordBot.Commands.RedditCommands.Interfaces;

namespace DiscordBot.Commands.RedditCommands
{
    internal class RedditService : IRedditService
    {
        private IServiceProvider? _serviceProvider;
        private IEmbedService? _embedService;
        private SocketInteractionContext? _context;
        private UInt16 _number = 1;
        private IRedditPostHelper? _redditPostHelper;
        private IRedditPostsGetter? _redditPostsGetter;

        public RedditService()
        {
            _redditPostHelper = new RedditPostHelper();
            _redditPostsGetter = new RedditPostsGetter();
        }

        public void SetNumber(ushort number)
        {
            _number = number;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetContext(SocketInteractionContext context)
        {
            _context = context;
        }

        public async Task SendRedditPostAsync(string subreddit, RedditSort sort, RedditTime time, bool nsfw)
        {
            Configure();
            List<PostModel>? posts;
            try
            {
                posts = await _redditPostsGetter!.GetPostsListAsync(subreddit, sort, time)!;
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("reddit", ex.Message);
                return;
            }
            if (posts is null || posts.Count == 0)
            {
                await _embedService!.ReplyErrorAsync("reddit", "Couldn't get any posts. Maybe check the subreddit name?");
                return;
            }

            if (nsfw is false)
            {
                List<PostModel> filteredPosts = _redditPostHelper!.FilterPosts(posts)!;
                if (filteredPosts is null)
                {
                    await _embedService!.ReplyErrorAsync("reddit", "No non-NFSW posts found.");
                    return;
                }
                else
                {
                    var selectedPost = filteredPosts[0];
                    await _embedService!.SendRedditPostAsync(selectedPost);
                }
            }
            else
            {
                var selectedPost = posts[0];
                await _embedService!.SendRedditPostAsync(selectedPost);
            }
        }

        public async Task SendRandomRedditPostAsync(string subreddit, RedditRandomSort sort, RedditTime time, bool nsfw)
        {
            Configure();
            List<PostModel>? posts;
            try
            {
                posts = await _redditPostsGetter!.GetPostsListAsync(subreddit, sort, time)!;
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("reddit", ex.Message);
                return;
            }
            if (posts is null || posts.Count == 0)
            {
                await _embedService!.ReplyErrorAsync("reddit", "Couldn't get any posts. Maybe check the subreddit name?");
                return;
            }

            if (nsfw is false)
            {
                List<PostModel> filteredPosts = _redditPostHelper!.FilterPosts(posts)!;
                if (filteredPosts is null)
                {
                    await _embedService!.ReplyErrorAsync("reddit", "No non-NFSW posts found.");
                    return;
                }
                else
                {
                    var selectedPost = _redditPostHelper!.SelectRandomPost(posts);
                    await _embedService!.SendRedditPostAsync(selectedPost!);
                }
            }
            else
            {
                var selectedPost = _redditPostHelper!.SelectRandomPost(posts);
                await _embedService!.SendRedditPostAsync(selectedPost!);
            }
        }

        public async Task SendBestRedditPostAsync(string subreddit, bool nsfw)
        {
            Configure();
            List<PostModel>? posts;
            try
            {
                posts = await _redditPostsGetter!.GetBestPostsListAsync(subreddit)!;
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("reddit", ex.Message);
                return;
            }
            if (posts is null || posts.Count == 0)
            {
                await _embedService!.ReplyErrorAsync("reddit", "Couldn't get any posts. Maybe check the subreddit name?");
                return;
            }

            if (nsfw is false)
            {
                List<PostModel> filteredPosts = _redditPostHelper!.FilterPosts(posts)!;
                if (filteredPosts is null)
                {
                    await _embedService!.ReplyErrorAsync("reddit", "No non-NFSW posts found.");
                    return;
                }
                else
                {
                    var selectedPost = _redditPostHelper!.SelectRandomPost(posts);
                    await _embedService!.SendRedditPostAsync(selectedPost!);
                }
            }
            else
            {
                var selectedPost = _redditPostHelper!.SelectRandomPost(posts);
                await _embedService!.SendRedditPostAsync(selectedPost!);
            }
        }

        private void Configure()
        {
            _redditPostsGetter!.SetNumber(ref _number);
            _redditPostsGetter.SetServiceProvider(_serviceProvider!);
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            _embedService.SetContext(_context!);
        }
    }
}
