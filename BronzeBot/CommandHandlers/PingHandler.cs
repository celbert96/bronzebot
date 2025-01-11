using BronzeBot.Models;

namespace BronzeBot.CommandHandlers;

public class PingHandler : ISlashCommandHandler
{
    public const string CommandName = "ping";

    public SlashCommandResponse HandleSlashCommand(List<String> args)
    {
        return new SlashCommandResponse("pong");
    }
}