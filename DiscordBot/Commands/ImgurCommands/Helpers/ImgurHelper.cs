using DiscordBot.Commands.ImgurCommand.Models;
using DiscordBot.Commands.ImgurCommands.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.ImgurCommands.Helpers
{
    internal class ImgurHelper : IImgurHelper
    {
        public List<ImagesData> ExtractGalleryImagesAsync(Images images)
        {
            // Create list of images from singular images and albums.
            // If an album has 1 image, it will be ignored,
            // due to ImgurAPI not returning it as an array.
            List<ImagesData> imagesList = new();
            foreach (var mainImage in images.data)
            {
                if (mainImage.in_gallery == true)
                {
                    List<ImagesGalleryData> dataList = mainImage.images;
                    if (dataList is not null)
                    {
                        foreach (var galleryImage in dataList)
                        {
                            imagesList.Add(new ImagesData()
                            {
                                link = galleryImage.link,
                                type = galleryImage.type,
                                nsfw = mainImage.nsfw,
                                title = mainImage.title,
                                account_url = mainImage.account_url,
                                views = galleryImage.views,
                                datetime = galleryImage.datetime,
                                id = galleryImage.id,
                                in_gallery = false
                            });
                        }
                    }
                    else
                    {
                        imagesList.Add(mainImage);
                    }
                }
            }

            return imagesList;
        }

        public List<ImagesData> GetFilteredImages(List<ImagesData> imagesList)
        {
            // Filter images
            List<ImagesData> filteredImagesList = new();
            foreach (var image in imagesList)
            {
                if (image.type is not null && image.type.StartsWith("image") && !image.link.EndsWith(".gif") && image.nsfw == false)
                {
                    filteredImagesList.Add(image);
                }
            }

            return filteredImagesList;
        }

        public List<ImagesData> GetFilteredGifs(List<ImagesData> imagesList)
        {
            // Filter images
            List<ImagesData> filteredGifsList = new();
            foreach (var image in imagesList)
            {
                if (image.type is not null && image.type.StartsWith("image") && image.link.EndsWith(".gif") && image.nsfw == false)
                {
                    filteredGifsList.Add(image);
                }
            }

            return filteredGifsList;
        }
    }
}
