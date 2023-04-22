using System.Runtime.Serialization;

namespace DiscordBot.Commands.RedditCommands.Models
{
    public enum RedditSort
    {
        [EnumMember(Value = "top")]
        top,
        [EnumMember(Value = "new")]
        @new,
        [EnumMember(Value = "controversial")]
        controversial,
        [EnumMember(Value = "random")]
        random
    }

    public enum RedditRandomSort
    {
        [EnumMember(Value = "top")]
        top,
        [EnumMember(Value = "new")]
        @new,
        [EnumMember(Value = "controversial")]
        controversial
    }

    public enum RedditTime
    {
        [EnumMember(Value = "hour")]
        hour,
        [EnumMember(Value = "day")]
        day,
        [EnumMember(Value = "week")]
        week,
        [EnumMember(Value = "month")]
        month,
        [EnumMember(Value = "year")]
        year,
        [EnumMember(Value = "all")]
        all
    }
}
