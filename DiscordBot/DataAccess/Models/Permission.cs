using Discord;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiscordBot.DataAccess.Models
{
#pragma warning disable CS8618
    public record Permission
    {
        [Key]
        [Required]
        public Guid? Id { get; set; }
        [Required]
        public Enums.Commands Command { get; set; }
        [Required]
        public GuildPermission PermissionRequired { get; set; }
        [Required]
        public virtual ServerConfig ServerConfig { get; set; }
        [AllowNull]
        public virtual List<Role>? AllowedRoles { get; set; }
    }
}
