using Discord;
using DiscordBot.DataAccess.Models;
using DiscordBot.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.DataAccess.Repositories
{
    public class ServerConfigRepository : IServerConfigRepository
    {
        private BotContext _botContext;

        public ServerConfigRepository(BotContext botContext)
        {
            _botContext = botContext;
        }

        public async Task SaveConfigAsync()
        {
            await _botContext.SaveChangesAsync();
        }

        public async Task<ServerConfig> GetConfigAsync(ulong serverId)
        {
            var config = await _botContext.ServerConfigs
                .IgnoreAutoIncludes()
                .FirstAsync(x => x.ServerId == serverId);
            return config;
        }

        public async Task<ushort> GetMusicBotVoteTresholdAsync(ulong serverId)
        {
            var config = await _botContext.ServerConfigs.FirstAsync(x => x.ServerId == serverId);
            return config.MusicBotVoteTreshold;
        }

        public async Task<GuildPermission> GetCommandPermissionAsync(ulong serverId, Enums.Commands command)
        {
            var permission = await _botContext.Permissions
                .IgnoreAutoIncludes()
                .FirstAsync(x => x.ServerConfig.ServerId == serverId && x.Command == command);
            return permission.PermissionRequired;
        }

        public async Task<List<Role>> GetAllowedRolesAsync(ulong serverId, Enums.Commands command)
        {
            var roles = await _botContext.AllowedRoles
                .IgnoreAutoIncludes()
                .Where(x => x.Permission.ServerConfig.ServerId == serverId && x.Permission.Command == command)
                .ToListAsync();
            return roles;
        }

        public async Task SetPermissionAsync(ulong serverId, Enums.Commands command, Discord.GuildPermission permission)
        {
            var oldPermission = await _botContext.Permissions
                .IgnoreAutoIncludes()
                .FirstAsync(x => x.ServerConfig.ServerId == serverId && x.Command == command);
            oldPermission.PermissionRequired = permission;
            await _botContext.SaveChangesAsync();
        }

        public async Task AddPermissionRoleAsync(ulong serverId, Enums.Commands command, ulong roleId)
        {
            var permissions = await _botContext.Permissions
                .Where(x => x.ServerConfig.ServerId == serverId)
                .IgnoreAutoIncludes()
                .Include(x => x.AllowedRoles)
                .FirstOrDefaultAsync(x => x.Command == command);

            if (permissions.AllowedRoles.Any(x => x.RoleId == roleId) is false)
            {
                Role role = new()
                {
                    RoleId = roleId
                };
                permissions.AllowedRoles.Add(role);
                await _botContext.SaveChangesAsync();
            }
        }

        public async Task RemovePermissionRoleAsync(ulong serverId, Enums.Commands command, ulong roleId)
        {
            var role = await _botContext.AllowedRoles
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(x => x.Permission.ServerConfig.ServerId == serverId && x.Permission.Command == command && x.RoleId == roleId);
            if (role is not null)
            {
                _botContext.AllowedRoles.Remove(role);
                await _botContext.SaveChangesAsync();
            }
        }

        public async Task<List<SelfRole>> GetSelfRolesAsync(ulong serverId)
        {
            var roles = await _botContext.SelfRoles
                .Where(x => x.ServerConfig.ServerId == serverId)
                .ToListAsync();
            return roles;
        }

        public async Task AddSelfRoleAsync(ulong serverId, ulong roleId)
        {
            var serverConfig = await _botContext.ServerConfigs
                .IgnoreAutoIncludes()
                .Include(x => x.SelfRoles)
                .FirstOrDefaultAsync(x => x.ServerId == serverId);

            if (serverConfig is null || serverConfig.SelfRoles.Any(x => x.RoleId == roleId) is true)
            {
                return;
            }

            SelfRole selfRole = new()
            {
                RoleId = roleId
            };
            serverConfig.SelfRoles.Add(selfRole);
            await _botContext.SaveChangesAsync();
        }

        public async Task RemoveSelfRoleAsync(ulong serverId, ulong roleId)
        {
            var selfRole = await _botContext.SelfRoles
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(x => x.ServerConfig.ServerId == serverId && x.RoleId == roleId);
            if (selfRole is not null)
            {
                _botContext.SelfRoles.Remove(selfRole);
                await _botContext.SaveChangesAsync();
            }
        }

        public async Task SetAutoRoleAsync(ulong serverId, ulong? roleId)
        {
            var serverConfig = await _botContext.ServerConfigs.IgnoreAutoIncludes().FirstOrDefaultAsync(x => x.ServerId == serverId);
            if (serverConfig is null)
            {
                return;
            }
            serverConfig.AutoRoleId = roleId;
            await _botContext.SaveChangesAsync();
        }

        public async Task SetConfirmRoleAsync(ulong serverId, ulong? roleId)
        {
            var serverConfig = await _botContext.ServerConfigs.IgnoreAutoIncludes().FirstOrDefaultAsync(x => x.ServerId == serverId);
            if (serverConfig is null)
            {
                return;
            }
            serverConfig.ConfirmRoleId = roleId;
            await _botContext.SaveChangesAsync();
        }

        public async Task<ulong?> GetConfirmRoleIdAsync(ulong serverId)
        {
            var serverConfig = await _botContext.ServerConfigs.IgnoreAutoIncludes().FirstOrDefaultAsync(x => x.ServerId == serverId);
            if (serverConfig is null)
            {
                return null;
            }
            return serverConfig.ConfirmRoleId;
        }

        public async Task TryDeleteAllowedRoleAsync(ulong serverId, ulong roleId)
        {
            var allowedRole = await _botContext.AllowedRoles
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(x => x.RoleId == roleId);
            if (allowedRole is not null)
            {
                _botContext.Remove(allowedRole);
                await _botContext.SaveChangesAsync();
            }
        }

        public async Task TryDeleteSelfRoleAsync(ulong serverId, ulong roleId)
        {
            var selfRole = await _botContext.SelfRoles
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(x => x.RoleId == roleId);
            if (selfRole is not null)
            {
                _botContext.Remove(selfRole);
                await _botContext.SaveChangesAsync();
            }
        }

        public async Task TryDeleteAutoRoleAsync(ulong serverId, ulong roleId)
        {
            var serverConfig = await _botContext.ServerConfigs
                .IgnoreAutoIncludes()
                .FirstAsync(x => x.ServerId == serverId);
            if (serverConfig.AutoRoleId == roleId)
            {
                serverConfig.AutoRoleId = null;
                await _botContext.SaveChangesAsync();
            }
        }

        public async Task TryDeleteConfirmRoleAsync(ulong serverId, ulong roleId)
        {
            var serverConfig = await _botContext.ServerConfigs
                .IgnoreAutoIncludes()
                .FirstAsync(x => x.ServerId == serverId);
            if (serverConfig.AutoRoleId == roleId)
            {
                serverConfig.ConfirmRoleId = null;
                await _botContext.SaveChangesAsync();
            }
        }
    }
}
