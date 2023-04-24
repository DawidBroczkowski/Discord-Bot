using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Responses.Search;
using Victoria.Enums;
using Discord.Interactions;
using DiscordBot.GlobalServices.Interfaces;
using DiscordBot.Commands.MusicCommands.Interfaces;
using DiscordBot.DataAccess.Repositories.Interfaces;

// TODO:
// Refactor everything, might not even work

namespace DiscordBot.Commands.M
{
    public class MusicModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LavaNode _lavaNode;
        private readonly IAudioService _audioService;
        private readonly IConfigRepository _globalConfig;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);
        private IEmbedService _embedService;
        private IServerConfigRepository _serverConfig;

        public MusicModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _lavaNode = _serviceProvider.GetRequiredService<LavaNode>();
            _audioService = _serviceProvider.GetRequiredService<IAudioService>();
            _audioService.Configure(_serviceProvider);
            _globalConfig = _serviceProvider.GetRequiredService<IConfigRepository>();
            _serverConfig = _serviceProvider.GetRequiredService<IServerConfigRepository>();
        }

        [CheckPermission(DataAccess.Enums.Commands.join)]
        [SlashCommand("join", "Bot joins the voice channel you are currently in")]
        public async Task JoinAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            // Check if the user is in a channel:
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await _embedService.ReplyErrorAsync("join", "You must be connected to a voice channel!");
                return;
            }

            // Check if the bot is already connected to a channel:
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await _embedService.ReplyErrorAsync("join", "I'm already connected to a channel!");
                return;
            }

            // Connect to the channel:
            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await _embedService.ReplySuccessAsync("join", $"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("join", exception.Message);
            }
        }

        [CheckPermission(DataAccess.Enums.Commands.forcejoin)]
        [SlashCommand("forcejoin", "Bot force joins the voice channel you are currently in")]
        public async Task ForceJoinAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            // Check if the user is in a channel:
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await _embedService.ReplyErrorAsync("join", "You must be connected to a voice channel!");
                return;
            }

            // Check if the bot is connected to a channel.
            // If it's already in the same channel as the user, leave anyway:
            if (_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel ?? player.VoiceChannel;
                try
                {
                    await _lavaNode.LeaveAsync(voiceChannel);
                }
                catch (Exception exception)
                {
                    await _embedService.ReplyErrorAsync("leave", exception.Message);
                }
            }

            // Connect to the channel:
            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await _embedService.ReplySuccessAsync("join", $"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("join", exception.Message);
            }
        }

        [CheckPermission(DataAccess.Enums.Commands.leave)]
        [SlashCommand("leave", "Bot leaves the channel it is currently in")]
        public async Task LeaveAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("pause");

            if (player is null)
            {
                return;
            }

            var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null)
            {
                await _embedService.ReplyErrorAsync("leave", "Not sure which voice channel to disconnect from.");
                return;
            }

            // Leave the channel:
            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);
                await _embedService.ReplySuccessAsync("leave", $"I've left {voiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("leave", exception.Message);
            }
        }

        [CheckPermission(DataAccess.Enums.Commands.play)]
        [SlashCommand("play", "Bot plays a song from youtube music / a video from youtube")]
        public async Task PlayAsync([Summary(description: "Name of a *SONG* to search on youtube music or a direct link to search on youtube")]string searchQuery)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            // Check if the bot is connected to a channel.
            // If not, join:
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                // Check if the user is connected to a channel:
                var voiceState = Context.User as IVoiceState;
                if (voiceState?.VoiceChannel == null)
                {
                    await _embedService.ReplyErrorAsync("join", "You must be connected to a voice channel!");
                    return;
                }  

                // Join the channel:
                try
                {
                    await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                }
                catch (Exception exception)
                {
                    await _embedService.ReplyErrorAsync("join", exception.Message);
                }
            }
            else
            {
                // Check if the user is connected to the same voice channel:
                var voiceState = Context.User as IVoiceState;
                if (voiceState?.VoiceChannel != _lavaNode.GetPlayer(Context.Guild).VoiceChannel)
                {
                    await _embedService.ReplyErrorAsync("play", "You must be connected to the same voice channel as the bot");
                    return;
                }
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            SearchResponse searchResponse;

            // Check if the query is a direct link or a song name, then get the tracks:
            if (searchQuery.StartsWith("https://"))
            {
                searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, searchQuery);
            }
            else
            {
                searchResponse = await _lavaNode.SearchAsync(SearchType.YouTubeMusic, searchQuery);
            }
          
            if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
            {
                await _embedService.ReplyErrorAsync("play", $"I wasn't able to find anything for `{searchQuery}`.");
                return;
            }

            // Check if it's a playlist and enqueue the tracks:
            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                player.Queue.Enqueue(searchResponse.Tracks);
                await _embedService.ReplySuccessAsync("play", $"Enqueued {searchResponse.Tracks.Count} songs.");
            }
            else
            {
                var track = searchResponse.Tracks.FirstOrDefault();
                player.Queue.Enqueue(track);

                await _embedService.ReplySuccessAsync("play", $"Enqueued {track?.Title}");
            }

            // If the players is playing or paused, return:
            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                return;
            }

            // Else dequeue and play the track:
            player.Queue.TryDequeue(out var lavaTrack);
            await player.PlayAsync(x => {
                x.Track = lavaTrack;
                x.ShouldPause = false;
            });

        }

        [CheckPermission(DataAccess.Enums.Commands.forceplay)]
        [SlashCommand("forceplay", "Bot clears the queue and plays a song from youtube music / a video from youtube")]
        public async Task ForcePlayAsync([Summary(description: "Name of a *SONG* or a direct link to a youtube video")]string searchQuery)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            // Check if the user is connected to a voice channel:
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await _embedService.ReplyErrorAsync("join", "You must be connected to a channel.");
                return;
            }

            // If the bot is in another voice channel, leave:
            if (_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel ?? player.VoiceChannel;
                if (voiceState?.VoiceChannel != _lavaNode.GetPlayer(Context.Guild).VoiceChannel)
                {
                    try
                    {
                        await _lavaNode.LeaveAsync(voiceChannel);
                    }
                    catch (Exception exception)
                    {
                        await _embedService.ReplyErrorAsync("leave", exception.Message);
                        return;
                    }
                }
            }

            // Join the voice channel:
            try
            {
                await _lavaNode.JoinAsync(voiceState?.VoiceChannel, Context.Channel as ITextChannel);
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("join", exception.Message);
                return;
            }

            // Clear the queue:
            player.Queue.Clear();
            await player.StopAsync();

            SearchResponse searchResponse;

            // Check if the query is a direct link or a song name, then get the tracks:
            if (searchQuery.StartsWith("https://"))
            {
                searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, searchQuery);
            }
            else
            {
                searchResponse = await _lavaNode.SearchAsync(SearchType.YouTubeMusic, searchQuery);
            }

            if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
            {
                await _embedService.ReplyErrorAsync("play", $"I wasn't able to find anything for `{searchQuery}`.");
                return;
            }

            // Check if it's a playlist and enqueue the tracks:
            player = _lavaNode.GetPlayer(Context.Guild);
            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                player.Queue.Enqueue(searchResponse.Tracks);
                await _embedService.ReplySuccessAsync("play", $"Enqueued {searchResponse.Tracks.Count} songs.");
            }
            else
            {
                var track = searchResponse.Tracks.FirstOrDefault();
                player.Queue.Enqueue(track);

                await _embedService.ReplySuccessAsync("play", $"Enqueued {track?.Title}");
            }

            // If the players is playing or paused, return:
            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                return;
            }

            // Else dequeue and play the track:
            player.Queue.TryDequeue(out var lavaTrack);
            await player.PlayAsync(x => {
                x.Track = lavaTrack;
                x.ShouldPause = false;
            });

        }
    
        [CheckPermission(DataAccess.Enums.Commands.pause)]
        [SlashCommand("pause", "Pauses the current track")]
        public async Task PauseAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("pause");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not playing any tracks, return:
            if (player.PlayerState != PlayerState.Playing)
            {
                await _embedService.ReplyErrorAsync("pause", "I cannot pause when I'm not playing anything!");
                return;
            }
        
            // Pause the playing track:
            try
            {
                await player.PauseAsync();
                await _embedService.ReplySuccessAsync("pause", $"Paused: {player.Track.Title}");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("pause", exception.Message);
            }
        }
        
        [CheckPermission(DataAccess.Enums.Commands.resume)]
        [SlashCommand("resume", "Resumes playing the paused track")]
        public async Task ResumeAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("resume");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not paused, return:
            if (player.PlayerState != PlayerState.Paused)
            {
                await _embedService.ReplyErrorAsync("resume", "I cannot resume when I'm not playing anything!");
                return;
            }
        
            // Resume the paused track:
            try
            {
                await player.ResumeAsync();
                await _embedService.ReplySuccessAsync("resume", $"Resumed: {player.Track.Title}");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("resume", exception.Message);
            }
        }
        
        [CheckPermission(DataAccess.Enums.Commands.stop)]
        [SlashCommand("stop", "Stops playing all tracks")]
        public async Task StopAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("stop");
        
            if (player is null)
            {
                return;
            }
        
            // If player is already stopped, return:
            if (player.PlayerState == PlayerState.Stopped)
            {
                await _embedService.ReplyErrorAsync("stop", "I'm already stopped.");
                return;
            }
            // Stop playing all tracks:
            try
            {
                await player.StopAsync();
                await _embedService.ReplySuccessAsync("stop", "No longer playing anything.");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("stop", exception.Message);
            }
        }
        
        [CheckPermission(DataAccess.Enums.Commands.skip)]
        [SlashCommand("skip", "Votes to skip the current track")]
        public async Task SkipAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("skip");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not playing, return:
            if (player.PlayerState != PlayerState.Playing)
            {
                await _embedService.ReplyErrorAsync("skip", "I can't skip when nothing is playing.");
                return;
            }
        
            // Add all human users in voice chat into a list:
            var voiceChannelUsers = (player.VoiceChannel as SocketVoiceChannel)?.Users
                .Where(x => !x.IsBot)
                .ToArray();
        
            // If user already voted, return:
            if (_audioService.VoteQueue!.Contains(Context.User.Id))
            {
                await _embedService.ReplyErrorAsync("skip", "You can't vote again.");
                return;
            }
        
            // Add the vote to a queue.
            // If votes are insufficient, return:
            _audioService.VoteQueue.Add(Context.User.Id);
            if (voiceChannelUsers != null)
            {
                double percentage = _audioService.VoteQueue.Count / voiceChannelUsers.Length * 100;
                double requiredPercentage = await _serverConfig.GetMusicBotVoteTresholdAsync(Context.Guild.Id);
                if (percentage < requiredPercentage)
                {
                    double result = requiredPercentage / 100 * voiceChannelUsers.Length;
                    double requiredUsers = Math.Ceiling(result);
        
                    var embed = new EmbedBuilder()
                    {
                        Title = $"{_audioService.VoteQueue.Count}/{requiredUsers} required votes to skip.",
                        Color = Color.Gold
                    };
        
                    await ReplyAsync(embed: embed.Build());
                    return;
                }
            }
        
            // Skip the track:
            try
            {
                if(player.Queue.Count == 0)
                {
                    await player.StopAsync();
                    await _embedService.ReplySuccessAsync("skip", $"Skipped the last track.");
                }
                else
                {
                    var (oldTrack, currenTrack) = await player.SkipAsync();
                    await _embedService.ReplySuccessAsync("skip", $"Skipped: {oldTrack.Title}\nNow Playing: {player.Track.Title}");
                }
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("skip", exception.Message);
            }
        
            // Clear vote queue:
            _audioService.VoteQueue.Clear();
        }
        
        [CheckPermission(DataAccess.Enums.Commands.forceskip)]
        [SlashCommand("forceskip", "Force skips current track")]
        public async Task ForceSkipAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("skip");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not playing, return:
            if (player.PlayerState != PlayerState.Playing)
            {
                await _embedService.ReplyErrorAsync("skip", "I can't skip when nothing is playing.");
                return;
            }
        
            // Skip the track:
            try
            {
                if (player.Queue.Count == 0)
                {
                    await player.StopAsync();
                    await _embedService.ReplySuccessAsync("skip", $"Skipped the last track.");
                }
                else
                {
                    var (oldTrack, currenTrack) = await player.SkipAsync();
                    await _embedService.ReplySuccessAsync("skip", $"Skipped: {oldTrack.Title}\nNow Playing: {player.Track.Title}");
                }      
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("skip", exception.Message);
            }
        
            // Clear vote queue:
            _audioService.VoteQueue!.Clear();
        }
        
        [CheckPermission(DataAccess.Enums.Commands.seek)]
        [SlashCommand("seek", "Skips to a given moment in the video")]
        public async Task SeekAsync([Summary(description: "Where to skip, ex: 1m2s, 1h20m34s")]TimeSpan timeSpan)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("seek");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not playing, return:
            if (player.PlayerState != PlayerState.Playing)
            {
                await _embedService.ReplyErrorAsync("seek", "I can't seek when nothing is playing.");
                return;
            }
        
            // Skip to a given moment in time:
            try
            {
                await player.SeekAsync(timeSpan);
                await _embedService.ReplySuccessAsync("seek", $"I've seeked {player.Track.Title} to {timeSpan} seconds.");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("seek", exception.Message);
            }
        }
        
        [CheckPermission(DataAccess.Enums.Commands.volume)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [SlashCommand("volume", "Changes the bot's volume")]
        public async Task VolumeAsync([Summary(description: "Volume to change to")]ushort volume)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("volume");
        
            if (player is null)
            {
                return;
            }
        
            //Change the bot's volume:
            try
            {
                await player.UpdateVolumeAsync(volume);
                await _embedService.ReplySuccessAsync("volume", $"I've changed the player volume to {volume}.");
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("volume", exception.Message);
            }
        }
        
        [CheckPermission(DataAccess.Enums.Commands.nowplaying)]
        [SlashCommand("nowplaying", "Responds with the currently playing track")]
        public async Task NowPlayingAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("nowplaying");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not playing, return:
            if (player.PlayerState != PlayerState.Playing)
            {
                await _embedService.ReplyErrorAsync("nowplaying", "I'm not playing any tracks.");
                return;
            }
        
            // Reply with information about the track:
            var track = player.Track;
            var artwork = await track.FetchArtworkAsync();
        
            var embed = new EmbedBuilder()
                .WithAuthor(track.Author, Context.Client.CurrentUser.GetAvatarUrl(), track.Url)
                .WithTitle($"Now Playing: {track.Title}")
                .WithImageUrl(artwork)
                .WithFooter($"{track.Position}/{track.Duration}")
                .WithColor(Color.Red);
        
            await RespondAsync(embed: embed.Build());
        }
        
        [CheckPermission(DataAccess.Enums.Commands.queue)]
        [SlashCommand("queue", "Responds with all tracks currently in queue")]
        public async Task QueueAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("queue");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not playing, return:
            if (player.PlayerState != PlayerState.Playing)
            {
                await _embedService.ReplyErrorAsync("queue", "I'm not playing any tracks.");
                return;
            }
        
            // Reply with all tracks in queue:
            var embed = new EmbedBuilder()
            {
                Title = "Queue:",
                Description = $"{player.Queue.Count} tracks in queue",
                Color = Color.DarkGreen
            }.WithCurrentTimestamp()
            .WithFooter(x => x.WithText($"{Context.Interaction.User.Username}"));
        
            int i = 1;
        
            foreach(var track in player.Queue)
            {
                embed.AddField($"Position #{i++}", track.Title);
            }
        
            await RespondAsync(embed: embed.Build());
        }
        
        [CheckPermission(DataAccess.Enums.Commands.queueremove)]
        [SlashCommand("queueremove", "Removes a track with given ID from the queue")]
        public async Task QueueRemoveAsync([Summary(description: "Position in queue of the track you want to remove")]int id)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            var player = await ValidateConnectionAsync("pause");
        
            if (player is null)
            {
                return;
            }
        
            // If player is not playing, return:
            if (player.PlayerState != PlayerState.Playing)
            {
                await _embedService.ReplyErrorAsync("queue remove", "I'm not playing any tracks.");
                return;
            }
        
            // If the queue is empty, return:
            if (player.Queue.Count == 0)
            {
                await _embedService.ReplyErrorAsync("queue remove", "The queue is empty.");
                return;
            }
        
            // If the given id is greater than the number of tracks in queue, return:
            if (player.Queue.Count < id)
            {
                await _embedService.ReplyErrorAsync("queue remove", $"The queue contains only {player.Queue.Count} position(s).");
                return;
            }
        
            // Remove the track from queue:
            string title = player.Queue.ToList()[id-1].Title;
            player.Queue.RemoveAt(id-1);
        
            await _embedService.ReplySuccessAsync("queue remove", $"Successfully deleted {title} from the queue");
        }
        
        [CheckPermission(DataAccess.Enums.Commands.campfire)]
        [SlashCommand("campfire", "Makes a campfire")]
        public async Task CampfireAsync()
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            // Check if the bot is connected to a voice channel
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await _embedService.ReplyErrorAsync("campfire", "You must be connected to a channel.");
                return;
            }
        
            // If the bot is in another voice channel, leave:
            if (_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel ?? player.VoiceChannel;
                if (voiceState?.VoiceChannel != _lavaNode.GetPlayer(Context.Guild).VoiceChannel)
                {
                    try
                    {
                        await _lavaNode.LeaveAsync(voiceChannel);
                    }
                    catch (Exception exception)
                    {
                        await _embedService.ReplyErrorAsync("join", exception.Message);
                        return;
                    }
                }
            }
        
            // Join the voice channel:
            try
            {
                await _lavaNode.JoinAsync(voiceState!.VoiceChannel, Context.Channel as ITextChannel);
            }
            catch (Exception exception)
            {
                await _embedService.ReplyErrorAsync("join", exception.Message);
                return;
            }
          
            // Clear the queue:
            player = _lavaNode.GetPlayer(Context.Guild);
            player.Queue.Clear();
            await player.StopAsync();
          
        
            // Play the Outer Wilds OST:
            var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, "https://www.youtube.com/watch?v=YR_wIb_n4ZU");
            var track = searchResponse.Tracks.FirstOrDefault();
            player.Queue.Enqueue(track);
        
            player.Queue.TryDequeue(out var lavaTrack);
            await player.PlayAsync(x => {
                x.Track = lavaTrack;
                x.ShouldPause = false;
            });
        
            // Send the campfire gif:
            var embed = new EmbedBuilder()
            {
                Color = Color.Orange,
                ImageUrl = "https://c.tenor.com/v5lxzTqe79AAAAAC/outer-wilds.gif",
                Title = "Everything's going to be all right."
            };
        
            await ModifyOriginalResponseAsync(x => x.Embed = embed.Build());
        }

        private async Task<LavaPlayer?> ValidateConnectionAsync(string cmd)
        {
            _embedService = _serviceProvider.GetRequiredService<IEmbedService>();
            _embedService.SetContext(Context);
            // Check if the bot is connected to a voice channel
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await _embedService.ReplyErrorAsync(cmd, "I'm not connected to a voice channel.");
                return null;
            }

            // Check if the user is connected to a voice channel:
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await _embedService.ReplyErrorAsync(cmd, "You must be connected to a channel.");
                return null;
            }

            // Check if the user is connected to the same voice channel:
            if (voiceState?.VoiceChannel != _lavaNode.GetPlayer(Context.Guild).VoiceChannel)
            {
                await _embedService.ReplyErrorAsync(cmd, "You must be connected to the same voice channel as the bot");
                return null;
            }

            return player;
        }
    }
}
