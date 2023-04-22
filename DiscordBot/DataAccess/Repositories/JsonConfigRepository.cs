using System.Text.Json;
using DiscordBot.Entities;
using DiscordBot.DataAccess.Repositories.Interfaces;

namespace DiscordBot.Repositories
{
#pragma warning disable CS8618
    public class JsonConfigRepository : IConfigRepository
    {
        public Config Config { get; set; }

        public void SaveConfig()
        {
            string jsonString = JsonSerializer.Serialize(Config);
            File.WriteAllText("config.json", jsonString);
        }

        public void LoadConfig()
        {
            FileStream stream = File.OpenRead("config.json");
            Config = JsonSerializer.Deserialize<Config>(stream)!;
        }
    }
}
