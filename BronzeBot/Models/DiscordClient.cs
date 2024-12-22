using BronzeBot.Services;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace BronzeBot.Models;

public class DiscordClient
{
    private readonly DiscordSocketClient _socketClient = new();
    
    private readonly Dictionary<ulong, DiscordClientProps> _clientPropsMap = new(); 
    
    public async Task InitializeClient()
    {
        var token = Environment.GetEnvironmentVariable("BRONZE_BOT_TOKEN");
        if (token == null)
        {
            throw new Exception("Bot token not set");
        }
        
        await _socketClient.LoginAsync(TokenType.Bot, token);
        await _socketClient.StartAsync();

        _socketClient.Log += Log;
        _socketClient.Ready += OnClientReady;
        _socketClient.SlashCommandExecuted += SlashCommandHandler;
        _socketClient.UserVoiceStateUpdated += OnVoiceStateUpdated;
        _socketClient.JoinedGuild += OnGuildJoined;
    }
    
    private async Task OnClientReady()
    {
        foreach (var clientGuild in _socketClient.Guilds)
        {
            Console.WriteLine(clientGuild.Name);
            Console.WriteLine(clientGuild.Id);

            var clientProps = new DiscordClientProps
            {
                GuildId = clientGuild.Id,
                TextChannelId = clientGuild.TextChannels.First(e => e.Name == "general").Id,
                VoiceChannelId = clientGuild.VoiceChannels.First(e => e.Name == "General").Id
            };
            
            // if (!botGuildsRepository.BotGuildExists(clientGuild.Id))
            // {
            //     botGuildsRepository.AddBotGuild(clientGuild.Id, clientProps.TextChannelId, clientProps.VoiceChannelId);
            // }

            _clientPropsMap[clientGuild.Id] = clientProps;
        }
        var globalCommand = new SlashCommandBuilder();
        globalCommand.WithName("ping");
        globalCommand.WithDescription("Will return a pong");

        try
        {
            await _socketClient.CreateGlobalApplicationCommandAsync(globalCommand.Build());
        }
        catch(HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
    
    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var slashCommandService = SlashCommandService.GetInstance();
        var response = slashCommandService.HandleCommand(command.Data.Name);
    
        await command.RespondAsync(response.Text);
    }
    
    private async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (user.IsBot)
        {
            Console.WriteLine("User is bot");
            return;
        }

        if (before.VoiceChannel == null && after.VoiceChannel != null)
        {
            var clientProps = _clientPropsMap.Values.First(e => e.VoiceChannelId == after.VoiceChannel.Id);

            if (_socketClient.GetChannel(clientProps.TextChannelId) is IMessageChannel textChannel)
            {
                var users = after.VoiceChannel.ConnectedUsers;
                if (users?.Count == 1)
                {
                    await textChannel.SendMessageAsync( "@here " + user.Username + " invites you to play Rocket League!");
                }
            }
            else
            {
                Console.WriteLine("channel is null");
            }
        }
    }

    // bot has joined a new guild (server)
    private Task OnGuildJoined(SocketGuild guild)
    {
        Console.WriteLine(guild.Name + " joined");
        Console.WriteLine("Default channel: " + guild.DefaultChannel.Name);
        var clientProps = new DiscordClientProps
        {
            GuildId = guild.Id,
            TextChannelId = guild.TextChannels.First(e => e.Name == "general").Id,
            VoiceChannelId = guild.VoiceChannels.First(e => e.Name == "General").Id
        };

        _clientPropsMap[guild.Id] = clientProps;
        return Task.CompletedTask;
        
        //botGuildsRepository.AddBotGuild(guild.Id, textChannelId, voiceChannelId);
        // foreach (var socketGuildUser in guild.Users)
        // {
        //     foreach (var activity in socketGuildUser.Activities)
        //     {
        //         if (activity.Type == ActivityType.Playing)
        //         {
        //             
        //         }
        //     }
        // }
    }
}