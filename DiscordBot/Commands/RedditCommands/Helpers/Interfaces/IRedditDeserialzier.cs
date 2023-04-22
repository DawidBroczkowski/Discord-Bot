using DiscordBot.Commands.RedditCommands.Models;
using System.Text.Json.Nodes;

namespace DiscordBot.Commands.RedditCommands.Helpers.Interfaces
{
    internal interface IRedditDeserialzier
    {
        List<PostModel> DeserializePosts(JsonNode redditNode, RedditSort sort, ref ushort number);
        List<PostModel> DeserializePosts(JsonNode redditNode, ref ushort number);
    }
}