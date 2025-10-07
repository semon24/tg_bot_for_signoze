using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VneOcherediGuard.DataBase
{
    public class ChatIds
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Role { get; set; }

        public string TypeChat { get; set; }
    }
}
