using Discord.Interactions;
using DiscordBot.Commands.ImgurCommand.Models;
using DiscordBot.Commands.ImgurCommands.Helpers;
using DiscordBot.Commands.ImgurCommands.Helpers.Interfaces;
using DiscordBot.Commands.ImgurCommands.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.ImgurCommands
{
    internal class ImgurService : IImgurService
    {
        private IServiceProvider? _serviceProvider;
        private IEmbedService? _embedService;
        private SocketInteractionContext? _context;
        private IImgurGetter? _imgurGetter;

        public ImgurService()
        {
            _imgurGetter = new ImgurGetter();
        }

        public void SetContext(SocketInteractionContext context)
        {
            _context = context;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendRandomImageAsync(string content)
        {
            Configure();
            List<ImagesData> filteredImagesList;
            try
            {
                filteredImagesList = await _imgurGetter!.GetImagesListAsync(content);
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("image", ex.Message);
                return;
            }
            if (filteredImagesList is null)
            {
                await _embedService!.ReplyErrorAsync("image", "Couldn't get any fitting images.");
                return;
            }

            // Generate a random index
            Random random = new Random((int)DateTime.Now.Ticks);
            int index = random.Next(0, filteredImagesList.Count);

            // Send image in Discord.
            await _embedService!.SendImgurImageAsync(filteredImagesList[index], content);
        }

        public async Task SendTopImageAsync(string content)
        {
            Configure();
            List<ImagesData> filteredImagesList;
            try
            {
                filteredImagesList = await _imgurGetter!.GetImagesListAsync(content);
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("image", ex.Message);
                return;
            }
            if (filteredImagesList is null)
            {
                await _embedService!.ReplyErrorAsync("image", "Couldn't get any fitting images.");
                return;
            }

            var orderedImagesList = filteredImagesList.OrderBy(x => x.views);

            // Send image in Discord.
            await _embedService!.SendImgurImageAsync(orderedImagesList.Last(), content);
        }

        public async Task SendRandomGifAsync(string content)
        {
            Configure();
            List<ImagesData> filteredGifsList;
            try
            {
                filteredGifsList = await _imgurGetter!.GetGifsListAsync(content);
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("gif", ex.Message);
                return;
            }
            if (filteredGifsList is null)
            {
                await _embedService!.ReplyErrorAsync("gif", "Couldn't get any fitting gifs.");
                return;
            }

            // Generate a random index
            Random random = new Random((int)DateTime.Now.Ticks);
            int index = random.Next(0, filteredGifsList.Count);

            // Send gif in Discord.
            await _embedService!.SendImgurImageAsync(filteredGifsList[index], content);
        }

        public async Task SendTopGifAsync(string content)
        {
            Configure();
            List<ImagesData> filteredGifsList;
            try
            {
                filteredGifsList = await _imgurGetter!.GetGifsListAsync(content);
            }
            catch (Exception ex)
            {
                await _embedService!.ReplyErrorAsync("gif", ex.Message);
                return;
            }
            if (filteredGifsList is null)
            {
                await _embedService!.ReplyErrorAsync("gif", "Couldn't get any fitting gifs.");
                return;
            }

            var orderedImagesList = filteredGifsList.OrderBy(x => x.views);

            // Send gif in Discord.
            await _embedService!.SendImgurImageAsync(orderedImagesList.Last(), content);
        }

        private void Configure()
        {
            _imgurGetter!.SetServiceProvider(_serviceProvider!);
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            _embedService.SetContext(_context!);
        }
    }
}
