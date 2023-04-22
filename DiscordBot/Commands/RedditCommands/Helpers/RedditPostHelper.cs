using DiscordBot.Commands.RedditCommands.Helpers.Interfaces;
using DiscordBot.Commands.RedditCommands.Models;

namespace DiscordBot.Commands.RedditCommands.Helpers
{
    internal class RedditPostHelper : IRedditPostHelper
    {
        public List<PostModel>? FilterPosts(List<PostModel> posts)
        {
            List<PostModel> filteredPosts = new();

            foreach (var post in posts)
            {
                if (post.Nsfw is false)
                {
                    filteredPosts.Add(post);
                }
            }

            if (filteredPosts.Count == 0)
            {
                return null;
            }
            else
            {
                return filteredPosts;
            }
        }

        public PostModel SelectRandomPost(List<PostModel> posts)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int index = random.Next(0, posts.Count);

            return posts[index];
        }
    }
}
