using System.Collections.Concurrent;
using DiscordBot.Commands.MusicCommands.Interfaces;
using DiscordBot.GlobalServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordBot.Services
{
    public sealed class AudioService : IAudioService
    {
        private LavaNode? _lavaNode;
        public HashSet<ulong>? VoteQueue { get; set; } 
        private ConcurrentDictionary<ulong, CancellationTokenSource>? _disconnectTokens;
        private IEmbedService? _embedService;
        private IServiceProvider? _serviceProvider;


        public void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _lavaNode = _serviceProvider.GetRequiredService<LavaNode>();
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();
            _lavaNode.OnTrackEnded += OnTrackEnded;
            _lavaNode.OnTrackStarted += OnTrackStarted;
            _lavaNode.OnTrackException += OnTrackException;
            _lavaNode.OnTrackStuck += OnTrackStuck;

            VoteQueue = new HashSet<ulong>();
        }

        private async Task OnTrackStarted(TrackStartEventArgs arg)
        {
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            await _embedService.SendInfoEmbedAsync($"Now playing: {arg.Track.Title}", arg.Player.TextChannel);
            if (!_disconnectTokens!.TryGetValue(arg.Player.VoiceChannel.Id, out var value))
            {
                return;
            }

            if (value.IsCancellationRequested)
            {
                return;
            }

            value.Cancel(true);
            await _embedService.SendInfoEmbedAsync("Auto disconnect has been cancelled!", arg.Player.TextChannel);
        }

        private async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            if (args.Reason != TrackEndReason.Finished)
            {
                return;
            }

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var lavaTrack))
            {
                await _embedService!.SendInfoEmbedAsync("Queue completed!", args.Player.TextChannel);
                _ = InitiateDisconnectAsync(args.Player, TimeSpan.FromSeconds(10));
                return;
            }

            if (lavaTrack is null)
            {
                await _embedService!.SendInfoEmbedAsync("Next item in queue is not a track.", args.Player.TextChannel);
                return;
            }

            await args.Player.PlayAsync(lavaTrack);
            await _embedService!.SendInfoEmbedAsync($"{args.Reason}: {args.Track.Title}\nNow playing: {lavaTrack.Title}", args.Player.TextChannel);
        }

        private async Task InitiateDisconnectAsync(LavaPlayer player, TimeSpan timeSpan)
        {
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            if (!_disconnectTokens!.TryGetValue(player.VoiceChannel.Id, out var value))
            {
                value = new CancellationTokenSource();
                _disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
            }
            else if (value.IsCancellationRequested)
            {
                _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
                value = _disconnectTokens[player.VoiceChannel.Id];
            }

            await _embedService.SendInfoEmbedAsync($"Auto disconnect initiated! Disconnecting in {timeSpan}...", player.TextChannel);
            var isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
            if (isCancelled)
            {
                return;
            }

            await _lavaNode!.LeaveAsync(player.VoiceChannel);
            await _embedService.SendInfoEmbedAsync("Disconnected", player.TextChannel);
        }

        private async Task OnTrackException(TrackExceptionEventArgs arg)
        {
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            arg.Player.Queue.Enqueue(arg.Track);
            await _embedService.SendInfoEmbedAsync($"{arg.Track.Title} has been re-added to queue after throwing an exception.", arg.Player.TextChannel);
        }

        private async Task OnTrackStuck(TrackStuckEventArgs arg)
        {
            _embedService = _serviceProvider!.GetRequiredService<IEmbedService>();
            arg.Player.Queue.Enqueue(arg.Track);
            await _embedService.SendInfoEmbedAsync($"{arg.Track.Title} has been re-added to queue after getting stuck.", arg.Player.TextChannel);
        }
    }
}