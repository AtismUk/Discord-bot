using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_bot.Db.Models
{
    public class Guild : BaseModel
    {
        public ulong? guildId { get; set; }  

        public List<AvoidRoleGuild> avoidRoles { get; set; }
    }
}
