using System.Text.Json.Nodes;

namespace DiscordBot.GlobalServices.Interfaces
{
    internal interface IParseService
    {
        Task<JsonNode> ParseResponseAsync(HttpResponseMessage response);
    }
}