using System.ComponentModel.DataAnnotations;

namespace DiscordBot.DataAccess.Models
{
#pragma warning disable CS8618
    public record Role
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public ulong RoleId { get; set; }
        [Required]
        public virtual Permission Permission { get; set; }
    }
}
