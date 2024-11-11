using BronzeBot.Services;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace BronzeBot.Models;

public class DiscordClient
{
    private DiscordClient() { }
    
    public static readonly DiscordClient Instance = new();

    private static readonly DiscordSocketClient SocketClient = new();
    
    private static DiscordClientProps _clientProps = null!;

    private static bool _initialized;

    public async Task InitializeClient()
    {
        if (_initialized)
        {
            return;
        }
        
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
        
        await SocketClient.LoginAsync(TokenType.Bot, token);
        await SocketClient.StartAsync();

        SocketClient.Log += Log;
        SocketClient.Ready += OnClientReady;
        SocketClient.SlashCommandExecuted += SlashCommandHandler;
        SocketClient.UserVoiceStateUpdated += OnVoiceStateUpdated;
        SocketClient.JoinedGuild += OnGuildJoined;

        _initialized = true;
    }
    
    private static async Task OnClientReady()
    {
        foreach (var clientGuild in SocketClient.Guilds)
        {
            Console.WriteLine(clientGuild.Name);
        }
        var globalCommand = new SlashCommandBuilder();
        globalCommand.WithName("ping");
        globalCommand.WithDescription("Will return a pong");

        try
        {
            await SocketClient.CreateGlobalApplicationCommandAsync(globalCommand.Build());
        }
        catch(HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
    
    private static async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var slashCommandService = SlashCommandService.GetInstance();
        var response = slashCommandService.HandleCommand(command.Data.Name);
    
        await command.RespondAsync(response.Text);
    }
    
    private static async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (user.IsBot)
        {
            Console.WriteLine("User is bot");
            return;
        }
        
   
        if (before.VoiceChannel == null && after.VoiceChannel != null)
        {
            if (SocketClient.GetChannel(_clientProps.TextChannelId) is IMessageChannel textChannel)
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
    private static Task OnGuildJoined(SocketGuild guild)
    {
        Console.WriteLine(guild.Name + " joined");
        Console.WriteLine("Default channel: " + guild.DefaultChannel.Name);
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