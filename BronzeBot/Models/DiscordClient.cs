using BronzeBot.Repositories;
using BronzeBot.Services;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace BronzeBot.Models;

public class DiscordClient(BotGuildsRepository botGuildsRepository)
{
    private readonly DiscordSocketClient _socketClient = new();
    
    private DiscordClientProps _clientProps = null!;

    private readonly BotGuildsRepository _botGuildsRepository = botGuildsRepository;


    public async Task InitializeClient()
    {
        var token = Environment.GetEnvironmentVariable("BRONZE_BOT_TOKEN");
        if (token == null)
        {
            throw new Exception("Bot token not set");
        }
        
        ulong serverId = (ulong)Int64.Parse(Environment.GetEnvironmentVariable("RL_SERVER_ID") ?? string.Empty);
        if (serverId == 0)
        {
            throw new Exception("Server ID not provided");
        }
        
        ulong voiceChannelId = (ulong) Int64.Parse(Environment.GetEnvironmentVariable("RL_VOICE_CHANNEL_ID") ?? string.Empty);
        if (voiceChannelId == 0)
        {
            throw new Exception("No voice channel id provided.");
        }

        _clientProps = new DiscordClientProps
        {
            BotToken = token,
            TextChannelId = serverId,
            VoiceChannelId = voiceChannelId
        };
        
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
            if (_socketClient.GetChannel(_clientProps.TextChannelId) is IMessageChannel textChannel)
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
        _botGuildsRepository.AddBotGuild(guild.Id, _clientProps.TextChannelId, _clientProps.VoiceChannelId);
        return Task.CompletedTask;
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