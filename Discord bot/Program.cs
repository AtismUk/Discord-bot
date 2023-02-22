using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord_bot.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Discord_bot
{
    internal class Program
    {


        DiscordSocketClient _client = new(new()
        {
            GatewayIntents = GatewayIntents.All,
            UseInteractionSnowflakeDate = false
        });


        static Task Main(string[] args) => new Program().MainAsync();

        async Task MainAsync()
        {
            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await _client.LoginAsync(TokenType.Bot, "MTA2OTk3NjQ3MTkzNTkxNDA0Ng.G_NkGV.efHPdkfb5MXGFL3Ejj5uaY2IQSZ8Q-U6Be-7_M");
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            ProcessSlashCommand slashCommand = new(_client);
            switch (command.Data.Name)
            {
                case "get-all-users-didnt-vote":
                    await slashCommand.GetAllUsersDidntReaction(command);
                    break;
            }
        }
        private async Task Client_Ready()
        {

            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("get-all-users-didnt-vote");
            globalCommand.WithDescription("Получить всех, кто не голосовал");
            globalCommand.AddOption("channel", ApplicationCommandOptionType.Channel, "Канал с голосованием", isRequired: true);
            globalCommand.AddOption("number", ApplicationCommandOptionType.Number, "Номер сообщения", isRequired: true);
            globalCommand.AddOption("role", ApplicationCommandOptionType.Role, "Роль, которую не учитываем", isRequired: true);
            globalCommand.AddOption("role_optional", ApplicationCommandOptionType.Role, "Дополнительная роль", isRequired: false);

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
  
            }
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}