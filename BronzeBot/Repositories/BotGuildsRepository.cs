using BronzeBot.Services;

namespace BronzeBot.Repositories;

/*
 * create table bot_guilds (
                     guild_id varchar(255),
                     text_channel_id varchar(255),
                     voice_channel_id varchar(255)
                 )
 */
public class BotGuildsRepository
{
    private readonly IDatabaseService _databaseService;
    private const string GuildIdBindVarTag = "guildId";
    private const string TextChannelBindVarTag = "textChannelId";
    private const string VoiceChannelIdBindVarTag = "voiceChannelId";

    public BotGuildsRepository(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public bool AddBotGuild(ulong guildId, ulong textChannelId, ulong voiceChannelId)
    {
        var bindVars = new Dictionary<string, object>
        {
            { GuildIdBindVarTag, guildId.ToString() },
            { TextChannelBindVarTag, textChannelId.ToString() },
            { VoiceChannelIdBindVarTag, voiceChannelId.ToString() }
        };

        var sql =
            $"insert into bot_guilds (guild_id, text_channel_id, voice_channel_id) values (@{GuildIdBindVarTag}, @{TextChannelBindVarTag}, @{VoiceChannelIdBindVarTag})";
        
        var numRows = _databaseService.PerformNonQuery(sql, bindVars);
        return numRows > 0;
    }
}