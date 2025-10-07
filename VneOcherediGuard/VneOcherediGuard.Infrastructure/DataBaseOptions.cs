using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VneOcherediGuard.Options;

public sealed class DataBaseOptions
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host {  get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
}

