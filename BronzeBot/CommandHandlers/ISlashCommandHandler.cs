using BronzeBot.Models;

namespace BronzeBot.CommandHandlers;

public interface ISlashCommandHandler
{
    public SlashCommandResponse HandleSlashCommand(List<String> args);
}