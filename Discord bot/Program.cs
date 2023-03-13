using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord_bot.Db;
using Discord_bot.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Discord_bot
{
    internal class Program
    {
        private readonly AppDbContext _context = new(new());

        DiscordSocketClient _client = new(new()
        {
            GatewayIntents = GatewayIntents.All,
            UseInteractionSnowflakeDate = false
        });


        static Task Main(string[] args) => new Program().MainAsync();

        async Task MainAsync()
        {
            _client.Log += Log;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await _client.LoginAsync(TokenType.Bot, "MTA2OTk3NjQ3MTkzNTkxNDA0Ng.G_NkGV.efHPdkfb5MXGFL3Ejj5uaY2IQSZ8Q-U6Be-7_M");
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            ProcessSlashCommand slashCommand = new(_client, _context);
            switch (command.Data.Name)
            {
                case "get-all-users-didnt-vote":
                    await slashCommand.GetAllUsersDidntReaction(command);
                    break;
            }
        }
        private async Task Client_Ready()
        {
            var globalCommand = new SlashCommandBuilder()
            .WithName("Create-vote")
            .WithDescription("Создание голосование")
            .AddOption("Titel", ApplicationCommandOptionType.String, "Заголовок", isRequired: true)
            .AddOption("Description", ApplicationCommandOptionType.String, "Описание", isRequired: true)
            .AddOption("Nutton_One", ApplicationCommandOptionType.String, "Кнопка согласия", isRequired: false)
            .AddOption("Nutton_Two", ApplicationCommandOptionType.String, "Кнопка отмены", isRequired: false)
            .AddOption("Nutton_optional", ApplicationCommandOptionType.String, "Кнопка, опциональна", isRequired: false);

            //.WithName("get-all-users-didnt-vote")
            //.WithDescription("Получить всех, кто не голосовал")
            //.AddOption(new SlashCommandOptionBuilder()
            //    .WithName("avoid-role")
            //    .WithDescription("Получить список без учета роли")
            //    .WithType(ApplicationCommandOptionType.SubCommand)
            //    .AddOption("channel", ApplicationCommandOptionType.Channel, "Канал с голосованием", isRequired: true)
            //    .AddOption("number", ApplicationCommandOptionType.Number, "Номер сообщения", isRequired: true)
            //    .AddOption("role", ApplicationCommandOptionType.Role, "Роль, которую не учитываем", isRequired: true)
            //    .AddOption("role_optional", ApplicationCommandOptionType.Role, "Дополнительная роль", isRequired: false)
            //).AddOption(new SlashCommandOptionBuilder()
            //    .WithName("get-role")
            //    .WithDescription("Получить список людей одной роли")
            //    .WithType(ApplicationCommandOptionType.SubCommand)
            //    .AddOption("channel", ApplicationCommandOptionType.Channel, "Канал с голосованием", isRequired: true)
            //    .AddOption("number", ApplicationCommandOptionType.Number, "Номер сообщения", isRequired: true)
            //    .AddOption("role", ApplicationCommandOptionType.Role, "Роль, которая учитывается", isRequired: true)
            //    .AddOption("role_optional", ApplicationCommandOptionType.Role, "Дополнительная роль", isRequired: false)
            //);

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