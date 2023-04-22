using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiscordBot.DataAccess.Models
{
#pragma warning disable CS8618
    public record ServerConfig
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public ulong ServerId { get; set; }
        [Required]
        public ushort MusicBotVoteTreshold { get; init; }
        [Required]
        public virtual List<Permission> Permissions { get; set; }
        [AllowNull]
        public virtual List<SelfRole> SelfRoles { get; set; }
        [AllowNull]
        public ulong? AutoRoleId { get; set; }
        [AllowNull]
        public ulong? ConfirmRoleId { get; set; }
    }
}
