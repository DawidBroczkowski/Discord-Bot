using DiscordBot.Commands.RedditCommands.Helpers.Interfaces;
using DiscordBot.Commands.RedditCommands.Models;
using System.Globalization;
using System.Text.Json.Nodes;

namespace DiscordBot.Commands.RedditCommands.Helpers
{
    internal class RedditDeserialzier : IRedditDeserialzier
    {
        public List<PostModel> DeserializePosts(JsonNode redditNode, RedditSort sort, ref UInt16 number)
        {
            var posts = new List<PostModel>();

            for (UInt16 i = 0; i < number; i++)
            {
                JsonNode? postNode;
                if (sort == RedditSort.random)
                {
                    try
                    {
                        postNode = redditNode?[0]?["data"]?["children"]?[i]?["data"];
                    }
                    catch
                    {
                        number = i;
                        break;
                    }
                }
                else
                {
                    try
                    {
                        postNode = redditNode?["data"]?["children"]?[i]?["data"];
                    }
                    catch
                    {
                        number = i;
                        break;
                    }
                }

                PostModel post = CreatePostModel(postNode!);

                posts.Add(post);
            }

            return posts;
        }
        public List<PostModel> DeserializePosts(JsonNode redditNode, ref UInt16 number)
        {
            var posts = new List<PostModel>();

            for (UInt16 i = 0; i < number; i++)
            {
                JsonNode? postNode;
                try
                {
                    postNode = redditNode?["data"]?["children"]?[i]?["data"];
                }
                catch
                {
                    number = i;
                    break;
                }


                PostModel post = CreatePostModel(postNode!);
                posts.Add(post);
            }

            return posts;
        }

        private PostModel CreatePostModel(JsonNode postNode)
        {
            PostModel post = new()
            {
                Title = postNode["title"]!.ToString(),
                Content = postNode["selftext"]!.ToString(),
                Author = postNode["author"]!.ToString(),
                Subreddit = postNode["subreddit_name_prefixed"]!.ToString(),
                Score = int.Parse(postNode["score"]!.ToString()),
                Nsfw = bool.Parse(postNode["over_18"]!.ToString()),
                ImageUrl = postNode["url"]!.ToString(),
                Link = "https://www.reddit.com" + postNode["permalink"]!.ToString(),
                Date = double.Parse(postNode["created"]!.ToString(), CultureInfo.InvariantCulture)
            };

            return post;
        }
    }
}
