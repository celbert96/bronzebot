﻿using BronzeBot.Services;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

var client = new DiscordSocketClient();
var token = Environment.GetEnvironmentVariable("BRONZE_BOT_TOKEN");
if (token == null)
{
    Console.WriteLine("No discord access token provided.");
    return;
}
ulong serverId = (ulong)Int64.Parse(Environment.GetEnvironmentVariable("RL_SERVER_ID") ?? string.Empty);
if (serverId == 0)
{
    Console.WriteLine("No server id provided.");
    return;
}
ulong voiceChannelId = (ulong) Int64.Parse(Environment.GetEnvironmentVariable("RL_VOICE_CHANNEL_ID") ?? string.Empty);
if (voiceChannelId == 0)
{
    Console.WriteLine("No voice channel id provided.");
    return;
}

static Task Log(LogMessage msg)
{
    Console.WriteLine(msg.ToString());
    return Task.CompletedTask;
}

async Task ClientReady()
{
    foreach (var clientGuild in client.Guilds)
    {
        Console.WriteLine(clientGuild.Name);
    }
    var globalCommand = new SlashCommandBuilder();
    globalCommand.WithName("ping");
    globalCommand.WithDescription("Will return a pong");

    try
    {
        await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
    }
    catch(HttpException exception)
    {
        var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
        Console.WriteLine(json);
    }
}

async Task SlashCommandHandler(SocketSlashCommand command)
{
    var slashCommandService = SlashCommandService.GetInstance();
    var response = slashCommandService.HandleCommand(command.Data.Name);
    
    await command.RespondAsync(response.Text);
}

async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
{
    if (user.IsBot)
    {
        Console.WriteLine("User is bot");
        return;
    }

   
    if (before.VoiceChannel == null && after.VoiceChannel != null)
    {
        if (client.GetChannel(serverId) is IMessageChannel channel)
        {
            var users = after.VoiceChannel.ConnectedUsers;
            if (users?.Count == 1)
            {
                await channel.SendMessageAsync( "@here " + user.Username + " invites you to play Rocket League!");
            }
        }
        else
        {
            Console.WriteLine("channel is null");
        }
    }
}


Task OnGuildJoined(SocketGuild guild)
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

client.Log += Log;
client.Ready += ClientReady;
client.JoinedGuild += OnGuildJoined;
client.UserVoiceStateUpdated += OnVoiceStateUpdated;
client.SlashCommandExecuted += SlashCommandHandler;


await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await Task.Delay(-1);