using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_bot.Db.Models
{
    public class AvoidRoleGuild : BaseModel
    {
        public int? roleId { get; set; }
        public string roleName { get; set; }

        public int guildId { get; set; }
        public Guild guild { get; set; }
    }
}
