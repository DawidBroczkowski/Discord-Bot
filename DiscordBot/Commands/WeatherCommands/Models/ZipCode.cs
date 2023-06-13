using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.WeatherCommands.Models
{
    public record ZipCode
    {
        public string Code { get; init; }
        public string Country { get; init; }
    }
}
