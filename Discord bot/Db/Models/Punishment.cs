using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_bot.Db.Models
{
    public class Punishment : BaseModel
    {
        public string Reason { get; set; }

        public int userId { get; set; }
    }
}
