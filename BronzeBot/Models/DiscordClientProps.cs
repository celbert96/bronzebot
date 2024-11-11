namespace BronzeBot.Models;

public class DiscordClientProps
{
    public required string BotToken { get; set;  }
    public required ulong TextChannelId { get; set; }
    public required ulong VoiceChannelId { get; set; }
}