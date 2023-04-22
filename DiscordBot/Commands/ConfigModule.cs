using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.DataAccess.Repositories.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands
{
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    [Group("permission", "Management of command permissions.")]
    public class ConfigModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private IEmbedService _embedService;
        private readonly IServerConfigRepository _serverConfig;

        public ConfigModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serverConfig = _serviceProvider.GetRequiredService<IServerConfigRepository>();
        }

        [SlashCommand("set", "Sets a permission requirement of the command.")]
        public async Task PermissionSetAsync(
            [Summary(description: "Command you want to set the permission of.")] DataAccess.Enums.Commands command,
            [Summary(description: "The permission (Case sensitive, use \"/permission get\" to get a list of permissions).")]DataAccess.Enums.Permissions permission)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            try
            {
                await _serverConfig.SetPermissionAsync(Context.Guild.Id, command, (GuildPermission)permission);
                await _serverConfig.SaveConfigAsync();
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("permission set", ex.Message);
                return;
            }
            await _embedService.ReplySuccessAsync("permission set", "Permission set successfully.");
        }

        [SlashCommand("add", "Adds a role to roles permitted to use this command.")]
        public async Task RoleAddAsync(
            [Summary(description: "Command you want to add the permission to.")] DataAccess.Enums.Commands command,
            [Summary(description: "The role.")] SocketRole role)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            try
            {
                await _serverConfig.AddPermissionRoleAsync(Context.Guild.Id, command, role.Id);
               // await _serverConfig.SaveConfigAsync();
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("permission add", ex.Message);
                return;
            }
            await _embedService.ReplySuccessAsync("permission add", "Successfully added the role to the command's allowed roles.");
        }

        [SlashCommand("remove", "Removes a role from roles permitted to use this command")]
        public async Task RemoveRoleAync(
            [Summary(description: "Command you want to remove the permission from.")] DataAccess.Enums.Commands command,
            [Summary(description: "The role.")] SocketRole role)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            try
            {
                await _serverConfig.RemovePermissionRoleAsync(Context.Guild.Id, command, role.Id);
                //await _serverConfig.SaveConfigAsync();
            }
            catch (Exception ex)
            {
                await _embedService.ReplyErrorAsync("permission remove", ex.Message);
                return;
            }
            await _embedService.ReplySuccessAsync("permission remove", "Successfully removed the role to the command's allowed roles.");
        }

        [SlashCommand("get", "Lists all possible permissions.")]
        public async Task GetPermissionsAsync()
        {
            string reply = "```";

            foreach (var permission in (DataAccess.Enums.Permissions[]) Enum.GetValues(typeof(DataAccess.Enums.Permissions)))
            {
                reply += $"\n{permission.ToString()}";
            }

            reply += "```";

            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Content = reply);
        }
    }
}
