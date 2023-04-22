using DiscordBot.Commands.RedditCommands.Models;

namespace DiscordBot.Commands.RedditCommands.Helpers.Interfaces
{
    internal interface IRedditPostHelper
    {
        List<PostModel>? FilterPosts(List<PostModel> posts);
        PostModel? SelectRandomPost(List<PostModel> posts);
    }
}