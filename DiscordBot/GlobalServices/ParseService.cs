using Discord.Interactions;
using DiscordBot.GlobalServices.Interfaces;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DiscordBot.GlobalServices
{
    internal class ParseService : IParseService
    {
        public async Task<JsonNode> ParseResponseAsync(HttpResponseMessage response)
        {
            string jsonString = await response.Content.ReadAsStringAsync();

            JsonNode jsonNode = JsonNode.Parse(jsonString)!;

            return jsonNode;
        }
    }
}
