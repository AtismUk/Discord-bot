using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Discord_bot
{
    internal class Program
    {


        DiscordSocketClient _client = new(new()
        {
            GatewayIntents = GatewayIntents.All
        });

        static Task Main(string[] args) => new Program().MainAsync();

        async Task MainAsync()
        {
            _client.Log += Log;
            _client.SlashCommandExecuted += SlashCommandHandler;

            var token = "MTA2OTk3NjQ3MTkzNTkxNDA0Ng.G_NkGV.efHPdkfb5MXGFL3Ejj5uaY2IQSZ8Q-U6Be-7_M";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "get-all-users-didnt-vote":
                    await GetAllUsersDidntVote(command);
                    break;
            }
        }

        private async Task GetAllUsersDidntVote(SocketSlashCommand command)
        {
            var number = command.Data.Options.Last().Value.ToString();
            var channel = (SocketTextChannel)command.Data.Options.First().Value;

            //Получаем все сообщения и достае т от туда нам нужное
            var messages = channel.GetMessagesAsync().FlattenAsync();
            var message = messages.Result.ElementAt(int.Parse(number)-1);

            var users = _client.GetGuild((ulong)command.GuildId).Users;
            List<IUser> usersList = new List<IUser>();
            List<IUser> userReactions = new();
            foreach (var reactionEmoji in message.Reactions.Keys)
            {
                var reactions = message.GetReactionUsersAsync(new Emoji(reactionEmoji.Name), 100).FlattenAsync();
                userReactions.AddRange(reactions.Result);
            }
            //Все люди, которые поставили реакцию
            var notRepeat = userReactions.DistinctBy(x => x.Id).ToList();
            foreach (var user in users)
            {
                if (!notRepeat.Select(x => x.Id).Contains(user.Id))
                {
                    usersList.Add(_client.GetUser(user.Id));
                }
            }


            string properUsers = "";
            foreach (var item in usersList)
            {
                properUsers += "\n" + item.Mention;
            }
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Reactions")
                .WithDescription(properUsers)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        private async Task Client_Ready()
        {

            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("get-all-users-didnt-vote");
            globalCommand.WithDescription("Получить всех, кто не голосовал");
            globalCommand.AddOption("channel", ApplicationCommandOptionType.Channel, "Канал с голосованием", isRequired: true);

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