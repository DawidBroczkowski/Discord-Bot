using Discord;
using Discord.Interactions;
using DiscordBot.Commands.SelfRoleCommands.Interfaces;
using DiscordBot.DataAccess.Repositories.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.SelfRoleCommands
{
    public class SelfRoleService : ISelfRoleService
    {
        private SocketInteractionContext? _context;
        private IServiceProvider? _serviceProvider;
        private IEmbedService? _embedService;
        private IServerConfigRepository? _serverConfig;

        public void SetContext(SocketInteractionContext context)
        {
            _context = context;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SpawnAddSelectionMenuAsync()
        {
            Configure();
            var selfRoles = await _serverConfig!.GetSelfRolesAsync(_context!.Guild.Id);
            if (selfRoles.Count == 0)
            {
                await _embedService!.ReplyErrorAsync("selfroles addspawn", "There are no self roles assigned in this server.");
                return;
            }

            List<string> roleNames = new();
            foreach (var role in selfRoles)
            {
                roleNames.Add(_context.Guild.Roles.First(x => x.Id == role.RoleId).Name);
            }

            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select a role you want to get.")
                .WithCustomId("SelfRolesAddMenu")
                .WithMinValues(1)
                .WithMaxValues(1);

            foreach (var roleName in roleNames)
            {
                menuBuilder.AddOption(roleName, _context.Guild.Roles.FirstOrDefault(x => x.Name == roleName)!.Id.ToString());
            }

            var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
            await _context.Interaction.FollowupAsync("Select a role you want to get.", components: builder.Build());
        }

        public async Task SpawnRemoveSelectionMenuAsync()
        {
            Configure();
            var selfRoles = await _serverConfig!.GetSelfRolesAsync(_context!.Guild.Id);
            if (selfRoles.Count == 0)
            {
                await _embedService!.ReplyErrorAsync("selfroles removespawn", "There are no self roles assigned in this server.");
                return;
            }

            List<string> roleNames = new();
            foreach (var role in selfRoles)
            {
                roleNames.Add(_context.Guild.Roles.First(x => x.Id == role.RoleId).Name);
            }

            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select a role you want to abandon.")
                .WithCustomId("SelfRolesRemoveMenu")
                .WithMinValues(1)
                .WithMaxValues(1);

            foreach (var roleName in roleNames)
            {
                menuBuilder.AddOption(roleName, _context.Guild.Roles.FirstOrDefault(x => x.Name == roleName)!.Id.ToString());
            }

            var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
            await _context.Interaction.FollowupAsync("Select a role you want to abandon.", components: builder.Build());
        }

        public async Task AddSelfRoleAsync(IRole role)
        {
            Configure();
            try
            {
                await _serverConfig!.AddSelfRoleAsync(_context!.Guild.Id, role.Id);
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("selfroles add", ex.Message);
            }

            await _embedService!.ReplySuccessAsync("selfroles add", $"Successfully marked \"{role.Name}\" as a self role.");
        }

        public async Task RemoveSelfRoleAsync(IRole role)
        {
            Configure();
            try
            {
                await _serverConfig!.RemoveSelfRoleAsync(_context!.Guild.Id, role.Id);
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("selfroles remove", ex.Message);
            }

            await _embedService!.ReplySuccessAsync("selfroles remove", $"Successfully unmarked \"{role.Name}\" as a self role.");
        }

        private void Configure()
        {
            _serverConfig = _serviceProvider!.GetRequiredService<IServerConfigRepository>();
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            _embedService.SetContext(_context!);
        }
    }
}
