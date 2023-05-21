using Discord;
using Discord.Interactions;
using DiscordBot.Commands.SelfRoleCommands.Interfaces;

namespace DiscordBot.Commands.SelfRoleCommands
{
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    [Group("selfroles", "Management of self assigned roles.")]
    public class SelfRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private ISelfRoleService _selfRoleService;

        public SelfRoleModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _selfRoleService = new SelfRoleService();
        }

        [SlashCommand("addspawn", "Creates a role selection menu that assigns a role you click on.")]
        public async Task SpawnAddSelectionMenuAsync()
        {
            Configure();
            await _selfRoleService.SpawnAddSelectionMenuAsync();
        }

        [SlashCommand("removespawn", "Creates a role selection menu that removes role you click on.")]
        public async Task SpawnRemoveSelectionMenuAsync()
        {
            Configure();
            await _selfRoleService.SpawnRemoveSelectionMenuAsync();
        }

        [SlashCommand("add", "Adds a self-role.")]
        public async Task AddSelfRoleAsync([Summary(description: "The role you want to mark as a self-role.")] IRole role)
        {
            Configure();
            await _selfRoleService.AddSelfRoleAsync(role);
        }

        [SlashCommand("remove", "Adds a self-role.")]
        public async Task RemoveSelfRoleAsync([Summary(description: "The role you want to unmark as a self-role.")] IRole role)
        {
            Configure();
            await _selfRoleService.RemoveSelfRoleAsync(role);
        }

        private void Configure()
        {
            _selfRoleService.SetContext(Context);
            _selfRoleService.SetServiceProvider(_serviceProvider);
        }
    }
}
