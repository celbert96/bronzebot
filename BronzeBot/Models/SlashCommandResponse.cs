namespace BronzeBot.Models;

public class SlashCommandResponse(String text)
{
    public string Text { get; } = text;
}