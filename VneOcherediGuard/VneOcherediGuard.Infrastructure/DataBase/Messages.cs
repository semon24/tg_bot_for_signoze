using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VneOcherediGuard.Infrastructure.DataBase
{
    public class MessagesTime
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string TypeMessage { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string DeploymentEnvironment { get; set; } = string.Empty;
    }
}
