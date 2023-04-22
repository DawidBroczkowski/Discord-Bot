using System.Runtime.Serialization;

namespace DiscordBot.Commands.JokeCommands.Models
{
    public class Enums
    {
        IEnumerable<string> types = new string[] { "programming", "misc", "dark", "pun", "spooky", "christmas" };
        public enum JokeType
        {

            [EnumMember(Value = "programming")]
            programming,
            [EnumMember(Value = "misc")]
            misc,
            [EnumMember(Value = "dark")]
            dark,
            [EnumMember(Value = "pun")]
            pun,
            [EnumMember(Value = "spooky")]
            spooky,
            [EnumMember(Value = "christmas")]
            christmas,
            [EnumMember(Value = "any")]
            any
        }
    }
}
