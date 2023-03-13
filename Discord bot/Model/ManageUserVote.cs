using Discord;
using Discord.Commands.Builders;
using Discord.WebSocket;
using Discord_bot.Db.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Discord_bot.Model
{
    public class ManageUserVote
    {
        private readonly SocketSlashCommandDataOption _command;
        private readonly DiscordSocketClient _client;

        private string _numbermessage;
        private SocketTextChannel _channel;

        public ManageUserVote(SocketSlashCommandDataOption command, DiscordSocketClient client, string numberMes, SocketTextChannel channel)
        {
            _command = command;
            _client = client;
            _numbermessage = numberMes;
            _channel = channel;
        }

        public List<IUser> GetAllUserDidntVote()
        {
            var role = _command.Options.ElementAt(2).Value.ToString();
            var roleOptionally = _command.Options.Last().Name.ToString() != "Optionally role" ? _command.Options.Last().Value.ToString() : null;


            //Получаем все сообщения и достаем от туда нужное
            var message = GetMessage();
            if (message.Reactions.Count() < 1)
            {
                throw new Exception("Под сообщением нет ни одной реакции");
            }
            //Подучаем всех пользователей с учетом того, что мы отсеиваем пользователей с выбранной ролью
            var users = GetAllUser(true);

            //Избегаем повторений, создаем окончательный список людей, которые не поставили ни одной реакции.
            var properUserList = GetAllUserEmoji(users, message);

            return properUserList;
        }

        public IMessage GetMessage()
        {
            var messages = _channel.GetMessagesAsync().FlattenAsync();
            var message = messages.Result.ElementAt(int.Parse(_numbermessage) - 1);
            return message;
        }

        public List<IUser> GetAlluserDidntVoteByRole()
        {
            var role = _command.Options.ElementAt(2).Value.ToString();
            var roleOptionally = _command.Options.Last().Name.ToString() != "Optionally role" ? _command.Options.Last().Value.ToString() : null;

            var message = GetMessage();
            if (message.Reactions.Count() < 1)
            {
                throw new Exception("Под сообщением нет ни одной реакции");
            }

            var users = GetAllUser(false);

            var properUserList = GetAllUserEmoji(users, message);
            if (properUserList.Count() < 1)
            {
                new Exception("Список по заданным параметрам пуст.");
            }
            return properUserList;
        }

        private List<IUser> GetAllUser(bool avoidRole)
        {
            var role = _command.Options.ElementAt(2).Value.ToString();
            var roleOptionally = _command.Options.Last().Name.ToString() != "Optionally role" ? _command.Options.Last().Value.ToString() : null;

            //Берем выбранные роли, идем в список всех юзеров и записываем на возврат всех пользователей, который либо только имеют выбранные роли или наоборот.
            List<IUser> users = new List<IUser>();
            foreach (var user in _channel.Users.ToList().Where(x => x.IsBot == false))
            {
                if (roleOptionally != null)
                {
                    if (avoidRole)
                    {
                        //Проверяем, чтобы у рользока не было роли, которую мы исключили, а так же доступа к каналу, где проводилось голосование, ибо зачем его выводить как не голсовавшего, если он и так не мог проголосовать.
                        if (!user.Roles.Select(x => x.Name).Contains(role) && !user.Roles.Select(x => x.Name).Contains(roleOptionally))
                        {
                            users.Add(user);
                        }
                    }
                    else
                    {
                        if (user.Roles.Select(x => x.Name).Contains(role) && user.Roles.Select(x => x.Name).Contains(roleOptionally))
                        {
                            users.Add(user);
                        }
                    }
                }
                else
                {
                    if (avoidRole)
                    {
                        if (!user.Roles.Select(x => x.Name).Contains(role))
                        {
                            users.Add(user);
                        }
                    }
                    else
                    {
                        if (user.Roles.Select(x => x.Name).Contains(role))
                        {
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        private List<IUser> GetAllUserEmoji(List<IUser> users, IMessage message)
        {
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

            return usersList;

        }
    }
}
