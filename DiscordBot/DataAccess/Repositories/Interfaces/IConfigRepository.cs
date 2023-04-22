using DiscordBot.Entities;

namespace DiscordBot.DataAccess.Repositories.Interfaces
{
    public interface IConfigRepository
    {
        Config Config { get; set; }
        void LoadConfig();
        void SaveConfig();
    }
}