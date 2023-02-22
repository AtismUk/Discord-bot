using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_bot.Db.Models
{
    public class User : BaseModel
    {
        public IEnumerable<Punishment> Punishments = new List<Punishment>();
    }
}
