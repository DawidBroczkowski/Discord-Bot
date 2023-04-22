using Discord.Interactions;
using DiscordBot.Commands.ImgurCommand.Models;
using DiscordBot.Commands.ImgurCommands.Helpers.Interfaces;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.ImgurCommands.Helpers
{
    internal class ImgurGetter : IImgurGetter
    {
        private IServiceProvider _serviceProvider;
        private IImgurCallHandler _imgurCallHandler;
        private readonly IImgurDeserializer _imgurDeserializer;
        private readonly IImgurHelper _imgurHelper;

        public ImgurGetter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _imgurCallHandler = new ImgurCallHandler();
            _imgurDeserializer = new ImgurDeserializer();
            _imgurHelper = new ImgurHelper();
        }

        public ImgurGetter()
        {
            _imgurCallHandler = new ImgurCallHandler();
            _imgurDeserializer = new ImgurDeserializer();
            _imgurHelper = new ImgurHelper();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<ImagesData>> GetImagesListAsync(string content)
        {
            _imgurCallHandler.SetServiceProvider(_serviceProvider);
            _imgurCallHandler.ConfigureClient();

            HttpResponseMessage response = await _imgurCallHandler.GetImagesAsync(content);
            if (response is null)
            {
                return null;
            }

            Images images = await _imgurDeserializer.DeserializeImagesAsync(response);
            if (images is null)
            {
                return null;
            }

            List<ImagesData> imagesList = _imgurHelper.ExtractGalleryImagesAsync(images);
            if (imagesList is null)
            {
                return null;
            }

            List<ImagesData> filteredImagesList = _imgurHelper.GetFilteredImages(imagesList);
            if (filteredImagesList is null)
            {
                return null;
            }

            return filteredImagesList;
        }

        public async Task<List<ImagesData>> GetGifsListAsync(string content)
        {
            _imgurCallHandler.SetServiceProvider(_serviceProvider);
            _imgurCallHandler.ConfigureClient();

            HttpResponseMessage response = await _imgurCallHandler.GetImagesAsync(content);
            if (response is null)
            {
                return null;
            }

            Images images = await _imgurDeserializer.DeserializeImagesAsync(response);
            if (images is null)
            {
                return null;
            }

            List<ImagesData> imagesList = _imgurHelper.ExtractGalleryImagesAsync(images);
            if (imagesList is null)
            {
                return null;
            }

            List<ImagesData> filteredImagesList = _imgurHelper.GetFilteredGifs(imagesList);
            if (filteredImagesList is null)
            {
                return null;
            }

            return filteredImagesList;
        }
    }
}
