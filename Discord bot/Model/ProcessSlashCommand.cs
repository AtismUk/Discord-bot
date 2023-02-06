using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_bot.Model
{
    public class ProcessSlashCommand
    {
        private readonly DiscordSocketClient _client;

        public ProcessSlashCommand(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task GetAllUsersDidntReaction(SocketSlashCommand command)
        {
            var channel = (SocketTextChannel)command.Data.Options.First().Value;
            var number = command.Data.Options.ElementAt(1).Value.ToString();
            var role = command.Data.Options.Last().Name.ToString() != "number" ? command.Data.Options.Last().Value.ToString(): null;


            //Получаем все сообщения и достае т от туда нам нужное
            var messages = channel.GetMessagesAsync().FlattenAsync();
            var message = messages.Result.ElementAt(int.Parse(number) - 1);

            var usersAll = _client.GetGuild((ulong)command.GuildId).Users.ToList();

            List<IUser> usersList = new List<IUser>();
            List<IUser> userReactions = new();
            foreach (var reactionEmoji in message.Reactions.Keys)
            {
                var reactions = message.GetReactionUsersAsync(new Emoji(reactionEmoji.Name), 100).FlattenAsync();
                userReactions.AddRange(reactions.Result);
            }
            //Все люди, которые поставили реакцию
            var notRepeat = userReactions.DistinctBy(x => x.Id).ToList();
            foreach (var user in notRepeat)
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
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedBuilder.Build());
        }
    }
}
