using Discord.Interactions;
using DiscordBot.Commands.ImgurCommands;
using DiscordBot.Commands.ImgurCommands.Interfaces;

namespace DiscordBot.Commands.ImgurCommand
{
    public class ImgurModule : InteractionModuleBase<SocketInteractionContext>
    {
        // Dependency injection:
        private IServiceProvider _serviceProvider;
        private IImgurService _imgurService;

        public ImgurModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _imgurService = new ImgurService();
        }
        // End of dependency injection.

        [CheckPermission(DataAccess.Enums.Commands.image)]
        [SlashCommand("image", "Gets an image from imgur (Random from search result)")]
        public async Task GetRandomImageAsync([Summary(description: "What to search for on imgur")]string content)
        {
            Configure();
            await _imgurService.SendRandomImageAsync(content);
        }

        [CheckPermission(DataAccess.Enums.Commands.imagetop)]
        [SlashCommand("imagetop", "Gets an image from imgur (First from search result)")]
        public async Task GetTopImageAsync([Summary(description: "What to search for on imgur")] string content)
        {
            Configure();
            await _imgurService.SendTopImageAsync(content);
        }

        [CheckPermission(DataAccess.Enums.Commands.gif)]
        [SlashCommand("gif", "Gets a gif from imgur (Random from search result)")]
        public async Task GetRandomGifAsync([Summary(description: "What to search for on imgur")] string content)
        {
            Configure();
            await _imgurService.SendRandomGifAsync(content);
        }

        [CheckPermission(DataAccess.Enums.Commands.giftop)]
        [SlashCommand("giftop", "Gets a gif from imgur (First from search result)")]
        public async Task GetTopGifAsync([Summary(description: "What to search for on imgur")] string content)
        {
            Configure();
            await _imgurService.SendTopGifAsync(content);
        }

        private void Configure()
        {
            _imgurService.SetContext(Context);
            _imgurService.SetServiceProvider(_serviceProvider);
        }
    }
}
