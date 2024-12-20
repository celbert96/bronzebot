using BronzeBot.Models;
using BronzeBot.Repositories;
using BronzeBot.Services;

var dbConnectionString = Environment.GetEnvironmentVariable("BRONZE_BOT_DB_CONNECTION_STRING") ?? string.Empty;
if (string.IsNullOrEmpty(dbConnectionString))
{
    throw new Exception("The Bronze Bot DB connection string is missing.");
}

var postgresService = new PostgresDatabaseService(dbConnectionString);
var botGuildsRepository = new BotGuildsRepository(postgresService);
var client = new DiscordClient(botGuildsRepository);
await client.InitializeClient();

await Task.Delay(-1);