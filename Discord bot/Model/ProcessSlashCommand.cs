using Discord;
using Discord.WebSocket;
using Discord_bot.Db;
using Discord_bot.Db.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord_bot.Model
{
    public class ProcessSlashCommand
    {
        private readonly DiscordSocketClient _client;
        private readonly AppDbContext _context;

        public ProcessSlashCommand(DiscordSocketClient client, AppDbContext context)
        {
            _client = client;
            _context = context;
        }

        public async Task SetAvoidRole(SocketSlashCommand command)
        {
            Guild guildFromDb;
            if (!_context.guilds.All(x => x.guildId == command.GuildId))
            {
                var guild = new Guild()
                {
                    guildId = command.GuildId
                };
                _context.guilds.Add(guild);

                guildFromDb = guild;
            }
            else
            {
                guildFromDb = _context.guilds.First(x => x.guildId == command.GuildId);
            }

            var role = command.Data.Options.First().Value.ToString();
            var roleOptionally = command.Data.Options.Last().Name.ToString() != "Optionally role" ? command.Data.Options.Last().Value.ToString() : null;

            List<AvoidRoleGuild> avoidRoles = new List<AvoidRoleGuild>()
            {
                new() {roleName = role, guildId = guildFromDb.Id}
            };
            if (roleOptionally != null)
            {
                avoidRoles.Add(new() { roleName = roleOptionally, guildId = guildFromDb.Id });
            }

        }

        public async Task GetAllUsersDidntReaction(SocketSlashCommand command)
        {
            string resultUsers = "Выбранное вами сообщение не содержит реакций";
            var embedBuilder = new EmbedBuilder();
            //Начальные данные:
            var properCommand = command.Data.Options.First();
            var channel = (SocketTextChannel)properCommand.Options.First().Value;
            var number = properCommand.Options.ElementAt(1).Value.ToString();

            ManageUserVote manageUserVote = new(properCommand, _client, number, channel);
            try
            {
                var message = manageUserVote.GetMessage();
                List<IUser> users = new();
                switch (command.Data.Options.First().Name)
                {
                    case "avoid-role":
                        users = manageUserVote.GetAllUserDidntVote();
                        break;
                    case "get-role":
                        users = manageUserVote.GetAlluserDidntVoteByRole();
                        break;
                }

                resultUsers = "";
                foreach (var item in users)
                {
                    resultUsers += "\n" + item.Mention + " -> " + item.Username;
                }
                embedBuilder.WithTitle("Люди, которые не проголосовали");
                embedBuilder.WithDescription(resultUsers + "\n" + $@"Канал сообщения: <#{message.Channel.Id}>");
                embedBuilder.WithFooter("Сообщение под номером: " + number);
                embedBuilder.WithCurrentTimestamp();
            }
            catch (Exception ex)
            {
                resultUsers = ex.Message.ToString();
                embedBuilder.WithTitle("Ошибка");
                embedBuilder.WithDescription(resultUsers);
                embedBuilder.WithCurrentTimestamp();
            }


            await command.RespondAsync(embed: embedBuilder.Build());
        }
    }
}