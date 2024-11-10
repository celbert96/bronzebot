namespace BronzeBot.Models;

public abstract class SlashCommandResponseBuilder
{
    protected SlashCommandResponse slashCommandResponse;

    public SlashCommandResponse Build()
    {
        return slashCommandResponse;
    }
    
    public abstract void AddText(string text);
}