using BronzeBot.CommandHandlers;
using BronzeBot.Models;

namespace BronzeBot.Services;

public class SlashCommandService
{
    private SlashCommandService(){}

    private static readonly SlashCommandService Instance = new();
    
    private readonly Dictionary<string, ISlashCommandHandler> _commandHandlers = new()
    {
        { PingHandler.CommandName, new PingHandler()}
    };

    public static SlashCommandService GetInstance()
    {
        return Instance;
    }

    public SlashCommandResponse HandleCommand(String command)
    {
        if (_commandHandlers.TryGetValue(command, out var handler))
        {
            return handler.HandleSlashCommand([]);
        }
        
        return new SlashCommandResponse("Unknown command");
    }
}