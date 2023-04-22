namespace DiscordBot.Commands.ImgurCommands.Helpers.Interfaces
{
    internal interface IImgurCallHandler
    {
        void ConfigureClient();
        Task<HttpResponseMessage> GetImagesAsync(string content);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}