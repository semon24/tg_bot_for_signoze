using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VneOcherediGuard.Diologs
{
    public class ConversationStateUpdateRole
    {
        public string CurrentHandler { get; set; } 
        public string UserName { get; set; }
        public string NewRole { get; set; }
    }
}
