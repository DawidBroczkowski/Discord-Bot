using Discord;
using Discord.Commands;
using Discord.Interactions;
using DiscordBot.Commands.ImgurCommand.Models;
using DiscordBot.Commands.JokeCommands.Models;
using DiscordBot.Commands.RedditCommands.Models;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Nodes;

namespace DiscordBot.Services
{
    public class EmbedService : IEmbedService
    {
        private SocketCommandContext? _commandContext;
        private SocketInteractionContext? _interactionContext;
        private HubConnection _connection;

        public EmbedService()
        {
            _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7029/chatHub")
            .WithAutomaticReconnect()
            .Build();
            _connection.StartAsync();
        }

        public void SetContext(SocketInteractionContext interactionContext)
        {
            _interactionContext = interactionContext;
        }

        public void SetContext(SocketCommandContext commandContext)
        {
            _commandContext = commandContext;
        }

        public async Task ReplyErrorAsync(string command, string description)
        {
            var embed = new EmbedBuilder()
            {
                Title = $"Error:  {command}",
                Description = description,
                Color = Color.DarkRed
            }.WithCurrentTimestamp()
            .WithFooter(x => x.WithText($"{_interactionContext!.Interaction.User.Username}"));

            
            await _interactionContext!.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed.Build());
            try
            {
                await _connection.InvokeAsync("SendMessage", "Bot", $"Error: {description}", _interactionContext.Guild.Id.ToString());
            }
            catch{}
        }

        public async Task ReplySuccessAsync(string command, string description)
        {
            var embed = new EmbedBuilder()
            {
                Title = $"Success:  {command}",
                Description = description,
                Color = Color.DarkGreen
            }.WithCurrentTimestamp()
            .WithFooter(x => x.WithText($"{_interactionContext!.Interaction.User.Username}"));
        
            await _interactionContext!.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed.Build());
            try
            {
                await _connection.InvokeAsync("SendMessage", "Bot", $"Success: {description}", _interactionContext.Guild.Id.ToString());
            }
            catch { }
        }

        public async Task SendInfoEmbedAsync(string info, ITextChannel channel)
        {
            var embed = new EmbedBuilder()
            {
                Title = info,
                Color = Color.Gold
            }.WithCurrentTimestamp();
 
            await channel.SendMessageAsync(embed: embed.Build());
            try
            {
                await _connection.InvokeAsync("SendMessage", "Bot", $"Success: {info}", _interactionContext.Guild.Id.ToString());
            }
            catch { }
        }

        public async Task SendImgurImageAsync(ImagesData data, string content)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            string authorUrl = $"https://imgur.com/user/{data.account_url}";
            string postUrl = $"https://imgur.com/{data.id}";

            var embed = new EmbedBuilder()
            {
                Color = Color.Green,
                Title = $"Search result of \"{content}\" from imgur:",
                Description = data.title,
                ImageUrl = data.link,
                Timestamp = time.AddSeconds(data.datetime).ToLocalTime(),
            }.WithFooter(footer => footer.Text = $"Imgur.com • {data.views} views")
            .WithAuthor(author => author.WithName(data.account_url)
            .WithUrl(authorUrl)).WithUrl(postUrl);

            await _interactionContext!.Interaction.DeleteOriginalResponseAsync();
            await _interactionContext!.Interaction.Channel.SendMessageAsync(embed: embed.Build());
            try
            {
                await _connection.InvokeAsync("SendMessage", "Bot", $"Success: {content}", _interactionContext.Guild.Id.ToString());
            }
            catch { }
        }

        public async Task SendRedditPostAsync(PostModel post)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            var embed = new EmbedBuilder()
            {
                Color = Color.Orange,
                Title = CutContent(post.Title, 250),
                Timestamp = time.AddSeconds(post.Date).ToLocalTime()
            }.WithFooter(footer => footer.Text = $"{post.Subreddit} • {post.Score} upvotes")
            .WithAuthor(Author => Author.Name = post.Author)
            .WithUrl(post.Link);

            if (post.Content is not null)
            {
                embed.WithDescription(CutContent(post.Content, 1000));
            }
            if (post.ImageUrl!.EndsWith(".jpg") || post.ImageUrl.EndsWith(".png") || post.ImageUrl.EndsWith(".gif"))
            {
                embed.WithImageUrl(post.ImageUrl);
            }

            await _interactionContext!.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed.Build());
            try
            {
                await _connection.InvokeAsync("SendMessage", "Bot", $"Success: {post.Title} - {post.Link}", _interactionContext.Guild.Id.ToString());
            }
            catch { }
        }

        public async Task SendCurrentWeatherDataAsync(JsonNode forecastNode)
        {
            var embed = new EmbedBuilder()
            {
                Color = Color.Green,
                Title = $"Current weather in: {forecastNode!["name"]!}, {forecastNode!["sys"]!["country"]!}",
                Description = $"\nTemperature: {ConvertKtoC(forecastNode!["main"]!["temp"]!.GetValue<double>()).ToString("N2")}°C" +
                $"\nHumidity: {forecastNode!["main"]!["humidity"]!}%" +
                $"\nWeather: {forecastNode!["weather"]![0]!["description"]}",
            }.WithFooter(footer => footer.Text = "OpenWeatherAPI")
            .WithCurrentTimestamp();

            await _interactionContext!.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed.Build());
            try
            {
                await _connection.InvokeAsync("SendMessage", "Bot", $"Success: {forecastNode!["name"]!}, {forecastNode!["sys"]!["country"]!}", _interactionContext.Guild.Id.ToString());
            }
            catch { }
        }

        public async Task SendJokeAsync(JokeModel joke)
        {
            
            if (joke.type == "twopart")
            {
                var embed = new EmbedBuilder()
                {
                    Color = Color.Green,
                    Description = $"{joke.setup}\n\n{joke.delivery}"
                }.WithFooter(footer => footer.Text = "JokeAPI")
                .WithCurrentTimestamp();
                await _interactionContext!.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed.Build());
                try
                {
                    await _connection.InvokeAsync("SendMessage", "Bot", $"Success: {joke.setup} - {joke.delivery}", _interactionContext.Guild.Id.ToString());
                }
                catch { }
            }
            else
            {
                var embed = new EmbedBuilder()
                {
                    Color = Color.Green,
                    Description = joke.joke
                }.WithFooter(footer => footer.Text = "JokeAPI")
                .WithCurrentTimestamp();
                await _interactionContext!.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed.Build());
                try
                {
                    await _connection.InvokeAsync("SendMessage", "Bot", $"Success: {joke.joke}", _interactionContext.Guild.Id.ToString());
                }
                catch { }
            }
        }

        private string CutContent(string content, int length)
        {
            if (content.Length > length)
            {
                content = content.Substring(0, length) + "...";
            }

            return content;
        }

        private static double ConvertKtoC(double value)
        {
            value -= 273.15;
            return value;
        }
    }
}
