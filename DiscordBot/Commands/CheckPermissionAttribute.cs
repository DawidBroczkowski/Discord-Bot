using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands
{
    public class CheckPermissionAttribute : PreconditionAttribute
    {
        private DataAccess.Enums.Commands _command;

        public CheckPermissionAttribute(DataAccess.Enums.Commands command)
        {
            _command = command;
        }

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            // Check if the command is being used in a guild:
            if (context.User is not SocketGuildUser user)
            {
                await context.Interaction.ModifyOriginalResponseAsync(x => x.Content = "You must be in a guild to run this command.");
                return PreconditionResult.FromError(string.Empty);
            }

            // Check if user is owner:
            if (user.Id == context.Guild.OwnerId)
            {
                return PreconditionResult.FromSuccess();
            }

            var serverConfig = services.GetRequiredService<IServerConfigRepository>();
            var allowedRoles = await serverConfig.GetAllowedRolesAsync(context.Guild.Id, _command);

            // Check if user has one of the roles allowed to run this command:
            if (allowedRoles is not null)
            {
                if (user.Roles.Select(x => x.Id).Intersect(allowedRoles.Select(x => x.RoleId)).Any())
                {
                    return PreconditionResult.FromSuccess();
                }
            }

            var permission = await serverConfig.GetCommandPermissionAsync(context.Guild.Id, _command);

            // Check if user has a role with the required permission:
            var roleIds = user.Roles.Select(x => x.Id);
            if (context.Guild.Roles.Where(x => x.Permissions.Has(permission)).Select(x => x.Id).Intersect(roleIds).Any())
            {
                return PreconditionResult.FromSuccess();
            }

            await context.Interaction.ModifyOriginalResponseAsync(x => x.Content = "You don't have the permission to run this command.");
            return PreconditionResult.FromError(string.Empty);
        }
    }
}
