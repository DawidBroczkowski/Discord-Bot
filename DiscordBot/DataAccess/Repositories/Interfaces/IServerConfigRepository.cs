using Discord;
using DiscordBot.DataAccess.Models;

namespace DiscordBot.DataAccess.Repositories.Interfaces
{
    public interface IServerConfigRepository
    {
        Task AddPermissionRoleAsync(ulong serverId, Enums.Commands command, ulong roleId);
        Task AddSelfRoleAsync(ulong serverId, ulong roleId);
        Task<List<Role>> GetAllowedRolesAsync(ulong serverId, Enums.Commands command);
        Task<GuildPermission> GetCommandPermissionAsync(ulong serverId, Enums.Commands command);
        Task<ServerConfig> GetConfigAsync(ulong serverId);
        Task<ulong?> GetConfirmRoleIdAsync(ulong serverId);
        Task<ushort> GetMusicBotVoteTresholdAsync(ulong serverId);
        Task<List<SelfRole>> GetSelfRolesAsync(ulong serverId);
        Task RemovePermissionRoleAsync(ulong serverId, Enums.Commands command, ulong roleId);
        Task RemoveSelfRoleAsync(ulong serverId, ulong roleId);
        Task SaveConfigAsync();
        Task SetAutoRoleAsync(ulong serverId, ulong? roleId);
        Task SetConfirmRoleAsync(ulong serverId, ulong? roleId);
        Task SetPermissionAsync(ulong serverId, Enums.Commands command, GuildPermission permission);
        Task TryDeleteAllowedRoleAsync(ulong serverId, ulong roleId);
        Task TryDeleteAutoRoleAsync(ulong serverId, ulong roleId);
        Task TryDeleteConfirmRoleAsync(ulong serverId, ulong roleId);
        Task TryDeleteSelfRoleAsync(ulong serverId, ulong roleId);
    }
}