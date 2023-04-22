using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Enums
{
    public enum Commands
    {
        [EnumMember(Value = "image")]
        image,
        [EnumMember(Value = "imagetop")]
        imagetop,
        [EnumMember(Value = "gif")]
        gif,
        [EnumMember(Value = "giftop")]
        giftop,
        [EnumMember(Value = "joke")]
        joke,
        [EnumMember(Value = "join")]
        join,
        [EnumMember(Value = "forcejoin")]
        forcejoin,
        [EnumMember(Value = "leave")]
        leave,
        [EnumMember(Value = "play")]
        play,
        [EnumMember(Value = "forceplay")]
        forceplay,
        [EnumMember(Value = "pause")]
        pause,
        [EnumMember(Value = "resume")]
        resume,
        [EnumMember(Value = "stop")]
        stop,
        [EnumMember(Value = "skip")]
        skip,
        [EnumMember(Value = "forceskip")]
        forceskip,
        [EnumMember(Value = "seek")]
        seek,
        [EnumMember(Value = "volume")]
        volume,
        [EnumMember(Value = "nowplaying")]
        nowplaying,
        [EnumMember(Value = "queue")]
        queue,
        [EnumMember(Value = "queueremove")]
        queueremove,
        [EnumMember(Value = "campfire")]
        campfire,
        [EnumMember(Value = "reddit")]
        reddit,
        [EnumMember(Value = "redditrandom")]
        redditrandom,
        [EnumMember(Value = "redditbest")]
        redditbest,
        [EnumMember(Value = "weather")]
        weather,
        [EnumMember(Value = "weatherzip")]
        weatherzip
    }

    [Flags]
    public enum Permissions : ulong
    {
        [EnumMember(Value = "CreateInstantInvite")]
        CreateInstantInvite = 0x1uL,
        [EnumMember(Value = "KickMembers")]
        KickMembers = 0x2uL,
        [EnumMember(Value = "BanMembers")]
        BanMembers = 0x4uL,
        [EnumMember(Value = "Administrator")]
        Administrator = 0x8uL,
        [EnumMember(Value = "ManageChannels")]
        ManageChannels = 0x10uL,
        [EnumMember(Value = "ManageGuild")]
        ManageGuild = 0x20uL,
        [EnumMember(Value = "ViewGuildInsights")]
        ViewGuildInsights = 0x80000uL,
        [EnumMember(Value = "AddReactions")]
        AddReactions = 0x40uL,
        [EnumMember(Value = "ViewAuditLog")]
        ViewAuditLog = 0x80uL,
        [EnumMember(Value = "ViewChannel")]
        ViewChannel = 0x400uL,
        [EnumMember(Value = "SendMessages")]
        SendMessages = 0x800uL,
        [EnumMember(Value = "SendTTSMessages")]
        SendTTSMessages = 0x1000uL,
        [EnumMember(Value = "ManageMessages")]
        ManageMessages = 0x2000uL,
        [EnumMember(Value = "EmbedLinks")]
        EmbedLinks = 0x4000uL,
        [EnumMember(Value = "AttachFiles")]
        AttachFiles = 0x8000uL,
        [EnumMember(Value = "ReadMessageHistory")]
        ReadMessageHistory = 0x10000uL,
        [EnumMember(Value = "MentionEveryone")]
        MentionEveryone = 0x20000uL,
        [EnumMember(Value = "UseExternalEmojis")]
        UseExternalEmojis = 0x40000uL,
        [EnumMember(Value = "Connect")]
        Connect = 0x100000uL,
        [EnumMember(Value = "Speak")]
        Speak = 0x200000uL,
        [EnumMember(Value = "MuteMembers")]
        MuteMembers = 0x400000uL,
        [EnumMember(Value = "DeafenMembers")]
        DeafenMembers = 0x800000uL,
        [EnumMember(Value = "MoveMembers")]
        MoveMembers = 0x1000000uL,
        [EnumMember(Value = "UseVAD")]
        UseVAD = 0x2000000uL,
        [EnumMember(Value = "PrioritySpeaker")]
        PrioritySpeaker = 0x100uL,
        [EnumMember(Value = "Stream")]
        Stream = 0x200uL,
        [EnumMember(Value = "ChangeNickname")]
        ChangeNickname = 0x4000000uL,
        [EnumMember(Value = "ManageNicknames")]
        ManageNicknames = 0x8000000uL,
        [EnumMember(Value = "ManageRoles")]
        ManageRoles = 0x10000000uL,
        [EnumMember(Value = "ManageWebhooks")]
        ManageWebhooks = 0x20000000uL,
        [EnumMember(Value = "ManageEmojisAndStickers")]
        ManageEmojisAndStickers = 0x40000000uL,
        [EnumMember(Value = "UseApplicationCommands")]
        UseApplicationCommands = 0x80000000uL,
        [EnumMember(Value = "RequestToSpeak")]
        RequestToSpeak = 0x100000000uL,
        [EnumMember(Value = "ManageEvents")]
        ManageEvents = 0x200000000uL,
        [EnumMember(Value = "ManageThreads")]
        ManageThreads = 0x400000000uL,
        [EnumMember(Value = "CreatePublicThreads")]
        CreatePublicThreads = 0x800000000uL,
        [EnumMember(Value = "CreatePrivateThreads")]
        CreatePrivateThreads = 0x1000000000uL,
        [EnumMember(Value = "UseExternalStickers")]
        UseExternalStickers = 0x2000000000uL,
        [EnumMember(Value = "SendMessagesInThreads")]
        SendMessagesInThreads = 0x4000000000uL,
        [EnumMember(Value = "StartEmbeddedActivities")]
        StartEmbeddedActivities = 0x8000000000uL,
        [EnumMember(Value = "ModerateMembers")]
        ModerateMembers = 0x10000000000uL
    }
}
