using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Commands.MusicCommands.Interfaces;
using DiscordBot.DataAccess;
using DiscordBot.DataAccess.Models;
using DiscordBot.DataAccess.Repositories;
using DiscordBot.DataAccess.Repositories.Interfaces;
using DiscordBot.GlobalServices;
using DiscordBot.GlobalServices.Interfaces;
using DiscordBot.Repositories;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Victoria;

namespace DiscordBot
{
    // Setup and event handling

    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfigRepository _configRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly LavaNode _lavaNode;
        private InteractionService _interactionService;
        private BotContext _botContext;
        private IServerConfigRepository _serverConfigRepository;

        public CommandHandler(DiscordSocketClient client)
        {
            _configRepository = new JsonConfigRepository();
            _configRepository.LoadConfig();

            _serviceProvider = ConfigureServices();
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BotContext>();
                dbContext.Database.EnsureCreated();
            }

            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _configRepository = _serviceProvider.GetRequiredService<IConfigRepository>();
            _lavaNode = _serviceProvider.GetRequiredService<LavaNode>();
            _botContext = _serviceProvider.GetRequiredService<BotContext>();

            InteractionServiceConfig cfg = new();
            cfg.DefaultRunMode = Discord.Interactions.RunMode.Async;
            _interactionService = new InteractionService(_client, cfg);

            _configRepository.LoadConfig();
            _serverConfigRepository = _serviceProvider.GetRequiredService<IServerConfigRepository>();   
        }

        public async Task InstallCommandsAsync()
        {
            

            // Hook the MessageReceived event into our command handler
            _client.Ready += OnReadyAsync;
            //_client.MessageReceived += HandleCommandAsync;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;
            _interactionService.SlashCommandExecuted += SlashCommandExecuted;
            _client.Log += Log;
            _client.JoinedGuild += ConfigureDatabaseAsync;
            _client.SelectMenuExecuted += HandleMenuAsync;
            _client.ButtonExecuted += HandleButtonAsync;
            _client.UserJoined += OnUserJoinedAsync;
            _client.RoleDeleted += OnRoleDeletedAsync;

            var token = _configRepository.Config.Token;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            await command.DeferAsync(); 
        }

        private async Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            await arg2.Interaction.DeferAsync();
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    //case InteractionCommandError.UnmetPrecondition:
                    //    await arg2.Interaction.RespondAsync($"Unmet Precondition: {arg3.ErrorReason}");
                    //    break;
                    case InteractionCommandError.UnknownCommand:
                        await arg2.Interaction.ModifyOriginalResponseAsync(x => x.Content = "Unknown command");
                        break;
                    case InteractionCommandError.BadArgs:
                        await arg2.Interaction.ModifyOriginalResponseAsync(x => x.Content = "Invalid number or arguments");
                        break;
                    case InteractionCommandError.ConvertFailed:
                        await arg2.Interaction.ModifyOriginalResponseAsync(x => x.Content = "Convert to an enum failed. There's a list of acceptable argument values." +
                        " Check the spelling and remember case sensitivity.");
                        break;
                    //case InteractionCommandError.Exception:
                    //    await arg2.Interaction.RespondAsync($"Command exception: {arg3.ErrorReason}");
                    //    break;
                    //case InteractionCommandError.Unsuccessful:
                    //    await arg2.Interaction.RespondAsync("Command could not be executed");
                    //    break;
                    default:
                        break;
                }
            }
        }

        private async Task OnReadyAsync()
        {
            
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }

            foreach (var guild in _client.Guilds.ToList())
            {
                if (await _serverConfigRepository!.GetConfigAsync(guild.Id) is null)
                {
                    await ConfigureDatabaseAsync(guild);
                }
            }

            //await _interactionService.RestClient.DeleteAllGlobalCommandsAsync();
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            await _interactionService.RegisterCommandsGloballyAsync();
            
            _client.InteractionCreated += async interaction =>
            {
                var ctx = new SocketInteractionContext(_client, interaction);
                await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
            };
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
             .AddSingleton<DiscordSocketClient>()
             .AddSingleton<IConfigRepository, JsonConfigRepository>()
             .AddSingleton<IAudioService, AudioService>()
             .AddTransient<IEmbedService, EmbedService>()
             .AddTransient<IParseService, ParseService>()
             .AddTransient<IServerConfigRepository, ServerConfigRepository>()
             .AddTransient<IButtonService, ButtonService>()
             .AddTransient<IMenuService, MenuService>()
             .AddLavaNode(x => 
             {
                 x.SelfDeaf = true;
                 x.Hostname = _configRepository.Config.LavaHostname;
                 x.Authorization = _configRepository.Config.LavaAuthorization;
                 x.Port = _configRepository.Config.LavaPort;
             })
             .AddDbContext<BotContext>(options =>
             {
                 options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DiscordBotDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
             }, contextLifetime: ServiceLifetime.Transient)
             .BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task ConfigureDatabaseAsync(IGuild guild)
        {

            ServerConfig config = new()
            {
                ServerId = guild.Id,
                MusicBotVoteTreshold = 50,
                Permissions = new()
            };

            foreach (var command in (DataAccess.Enums.Commands[])Enum.GetValues(typeof(DataAccess.Enums.Commands)))
            {
                Permission permission = new()
                {
                    Command = command,
                    PermissionRequired = GuildPermission.SendMessages
                };
                config.Permissions.Add(permission);
            }

            config.Permissions.First(x => x.Command == DataAccess.Enums.Commands.forcejoin).PermissionRequired = GuildPermission.Administrator;
            config.Permissions.First(x => x.Command == DataAccess.Enums.Commands.forceplay).PermissionRequired = GuildPermission.Administrator;
            config.Permissions.First(x => x.Command == DataAccess.Enums.Commands.forceskip).PermissionRequired = GuildPermission.Administrator;
            config.Permissions.First(x => x.Command == DataAccess.Enums.Commands.volume).PermissionRequired = GuildPermission.Administrator;
            config.Permissions.First(x => x.Command == DataAccess.Enums.Commands.campfire).PermissionRequired = GuildPermission.Administrator;

            _botContext.Add(config);
            await _botContext.SaveChangesAsync();
        }

        private async Task HandleMenuAsync(SocketMessageComponent arg)
        {
            await arg.RespondAsync("Loading...", ephemeral: true);
            var menuId = arg.Data.CustomId;
            IMenuService menuService = _serviceProvider.GetRequiredService<IMenuService>();
            menuService.Configure(arg, _client);

            switch (menuId)
            {
                case "SelfRolesAddMenu":
                    await menuService.SelfRoleAddSelectionMenuAsync();
                    break;
                case "SelfRolesRemoveMenu":
                    await menuService.SelfRoleRemoveSelectionMenuAsync();
                    break;
            }
            await arg.ModifyOriginalResponseAsync(x => x.Content = "Success");
        }

        private async Task HandleButtonAsync(SocketMessageComponent arg)
        {
            await arg.RespondAsync("Loading...", ephemeral: true);
            var menuId = arg.Data.CustomId;

            IButtonService buttonService = _serviceProvider.GetRequiredService<IButtonService>();
            buttonService.Configure(_serviceProvider, arg, _client);

            switch (menuId)
            {
                case "confirmbutton":
                    await buttonService.ConfirmRoleButtonAsync();
                    break;
            }
            await arg.ModifyOriginalResponseAsync(x => x.Content = "Success");
        }

        private async Task OnUserJoinedAsync(SocketGuildUser user)
        {
            var guildId = user.Guild.Id;
            var serverConfigRepository = _serviceProvider.GetRequiredService<IServerConfigRepository>();
            var serverConfig = await serverConfigRepository.GetConfigAsync(guildId);
            
            if (serverConfig.AutoRoleId is not null)
            {
                var autoRole = user.Guild.Roles.FirstOrDefault(x => x.Id == serverConfig.AutoRoleId);
                if (autoRole is null)
                {
                    await serverConfigRepository.SetAutoRoleAsync(user.Guild.Id, null);
                    return;
                }
                await user.AddRoleAsync(autoRole);
            }
        }

        private async Task OnRoleDeletedAsync(SocketRole role)
        {
            var serverConfigRepository = _serviceProvider.GetRequiredService<IServerConfigRepository>();
            await serverConfigRepository.TryDeleteSelfRoleAsync(role.Guild.Id, role.Id);
            await serverConfigRepository.TryDeleteAutoRoleAsync(role.Guild.Id, role.Id);
            await serverConfigRepository.TryDeleteAllowedRoleAsync(role.Guild.Id, role.Id);
            await serverConfigRepository.TryDeleteConfirmRoleAsync(role.Guild.Id, role.Id);
        }
    }
}
