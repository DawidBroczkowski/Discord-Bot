using Discord.Interactions;
using DiscordBot.Commands.RedditCommands.Interfaces;
using DiscordBot.Commands.RedditCommands.Models;

namespace DiscordBot.Commands.RedditCommands
{
    public class RedditModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private IRedditService _redditService;

        public RedditModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _redditService = new RedditService();
        }

        [CheckPermission(DataAccess.Enums.Commands.reddit)]
        [SlashCommand("reddit", "Gets the top post from a subreddit in a given sort mode")]
        public async Task GetRedditPostAsync([Summary(description: "Subreddit")]string subreddit, 
            [Summary(description: "Sort mode")]RedditSort sort, 
            [Summary(description: "Time mode")]RedditTime time)
        {
            Configure();
            await _redditService.SendRedditPostAsync(subreddit, sort, time, false);
        }

        [CheckPermission(DataAccess.Enums.Commands.redditrandom)]
        [SlashCommand("redditrandom", "Gets a random post from the top 30 posts in a given sort mode")]
        public async Task GetRandomRedditPostAsync([Summary(description: "Subreddit")] string subreddit,
            [Summary(description: "Sort mode")] RedditRandomSort sort,
            [Summary(description: "Time mode")] RedditTime time)
        {
            Configure();
            await _redditService.SendRandomRedditPostAsync(subreddit, sort, time, false);
        }

        [CheckPermission(DataAccess.Enums.Commands.redditbest)]
        [SlashCommand("redditbest", "Gets a random post from the top 30 posts in Best sort mode")]
        public async Task GetBestRedditPostAsync([Summary(description: "Subreddit")] string subreddit)
        {
            Configure();
            await _redditService.SendBestRedditPostAsync(subreddit, false);
        }

        private void Configure()
        {
            _redditService.SetContext(Context);
            _redditService.SetServiceProvider(_serviceProvider);
            _redditService.SetNumber(30);
        }
    }
}
