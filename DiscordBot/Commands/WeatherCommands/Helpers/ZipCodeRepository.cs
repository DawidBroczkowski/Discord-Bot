using DiscordBot.Commands.WeatherCommands.Models;
using System.Text.Json;

namespace DiscordBot.Commands.WeatherCommands.Helpers
{
    public class ZipCodeRepository
    {
        private List<ZipCode> _zipCodes = new();
        
        public ZipCodeRepository()
        {
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            string jsonString = File.ReadAllText("ZipCodes.json");
            _zipCodes = JsonSerializer.Deserialize<List<ZipCode>>(jsonString)!;
        }
        
        public string? GetZipCode(string countryCode)
        {
            return _zipCodes.FirstOrDefault(x => x.Country == countryCode)?.Code;
        }
    }
}
