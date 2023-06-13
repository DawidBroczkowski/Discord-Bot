using Discord;
using Discord.Commands;
using Discord.Interactions;
using DiscordBot.Commands.ImgurCommand.Models;
using DiscordBot.Commands.JokeCommands.Models;
using DiscordBot.Commands.RedditCommands.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json.Nodes;

namespace DiscordBot.GlobalServices.Interfaces
{
    public interface IEmbedService
    {
        Task ReplyErrorAsync(string command, string description);
        Task ReplySuccessAsync(string command, string description);
        Task SendCurrentWeatherDataAsync(JsonNode forecastNode);
        Task SendImgurImageAsync(ImagesData data, string content);
        Task SendInfoEmbedAsync(string info, ITextChannel channel);
        Task SendRedditPostAsync(PostModel post);
        Task SendJokeAsync(JokeModel joke);
        void SetContext(SocketCommandContext commandContext);
        void SetContext(SocketInteractionContext interactionContext);
    }
}