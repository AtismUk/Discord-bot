using Discord;
using Discord_bot.Db;
using Discord_bot.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_bot.Model
{
    public class ManageRole
    {
        private readonly AppDbContext _context;
        public ManageRole(AppDbContext context)
        {
            _context = context;
        }

        public async Task SetRole(List<AvoidRoleGuild> Roles, ulong guildId)
        {
            var guild = _context.guilds.FirstOrDefault(x => x.guildId == guildId);
            if (guild != null && Roles.Count() > 0)
            {
                string result = AddRole(Roles, guild);
            }
        }

        public string AddRole(List<AvoidRoleGuild> Roles, Guild guild)
        {
            string message = "";
            if (guild.avoidRoles.Count() == 0)
            {
                foreach (var role in Roles)
                {
                    _context.avoidRoleGuilds.Add(role);
                }
                message = "добавленно " + Roles.Count() + " ролей";
            }
            else
            {
                var noRepeatRole = guild.avoidRoles.ToList().Except(Roles).ToList();
                if (noRepeatRole.Count > 0)
                {
                    foreach (var role in noRepeatRole)
                    {
                        _context.avoidRoleGuilds.Add(role);
                        message = "добавленно " + Roles.Count() + " ролей";
                    }
                }
                message = "Добавленно 0 ролей";
            }

            _context.SaveChanges();
            return message;
           
        }
    }
}
