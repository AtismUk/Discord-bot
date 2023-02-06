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
            // Получаем все нужные данные
            var channel = (SocketTextChannel)command.Data.Options.First().Value;
            var number = command.Data.Options.ElementAt(1).Value.ToString();
            var role = command.Data.Options.ElementAt(2).Value.ToString();
            var roleOptionally = command.Data.Options.Last().Name.ToString() != "Optionally role" ? command.Data.Options.Last().Value.ToString(): null;


            //Получаем все сообщения и достаем от туда нужное
            var messages = channel.GetMessagesAsync().FlattenAsync();
            var message = messages.Result.ElementAt(int.Parse(number) - 1);
            //Строка с юзерами, которые не отреагировали на сообщение
            string resultUsers = "Выбранное вами сообщение не содержит реакций";
            if (message.Reactions.Count() > 0)
            {
                //Подучаем всех пользователей с учетом того, что мы отсеиваем пользователей с выбранной ролью
                List<IUser> users = new List<IUser>();
                foreach (var user in _client.GetGuild((ulong)command.GuildId).Users.ToList().Where(x => x.IsBot == false))
                {
                    if (roleOptionally != null)
                    {
                        if (!user.Roles.Select(x => x.Name).Contains(role) && !user.Roles.Select(x => x.Name).Contains(roleOptionally))
                        {
                            users.Add(user);
                        }
                    }
                    else
                    {
                        if (!user.Roles.Select(x => x.Name).Contains(role))
                        {
                            users.Add(user);
                        }
                    }
                }

                //Избегаем повторений, создаем окончательный список людей, которые не поставили ни одной реакции.
                List<IUser> usersList = new List<IUser>();
                List<IUser> userReactions = new();
                //Идем в сообщения, вытаскиваем всех пользаков, котоые поставили рекакцию
                foreach (var reactionEmoji in message.Reactions.Keys)
                {
                    var reactions = message.GetReactionUsersAsync(new Emoji(reactionEmoji.Name), 100).FlattenAsync();
                    userReactions.AddRange(reactions.Result);
                }
                //Теперь убераем повторения
                var notRepeat = userReactions.DistinctBy(x => x.Id).ToList();
                //Записываем людей, которые не поставили реакцию
                foreach (var user in users)
                {
                    if (!notRepeat.Select(x => x.Id).Contains(user.Id))
                    {
                        usersList.Add(_client.GetUser(user.Id));
                    }
                }

                resultUsers = "";
                foreach (var item in usersList)
                {
                    resultUsers += "\n" + item.Mention + " -> " + item.Username;
                }
            }


            var embedBuilder = new EmbedBuilder()
                .WithTitle("Люди, которые не проголосовали")
                .WithDescription(resultUsers + "\n" + $@"Канал сообщения: <#{message.Channel.Id}>")
                .WithFooter("Сообщение под номером: " + number)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedBuilder.Build());
        }
    }
}
