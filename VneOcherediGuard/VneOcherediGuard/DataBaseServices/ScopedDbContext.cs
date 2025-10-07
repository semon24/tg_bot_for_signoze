using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VneOcherediGuard.Infrastructure;

namespace VneOcherediGuard.DataBaseServices
{
    public class ScopedDbContext : IDisposable
    {
        public IServiceScope Scope { get; init; }
        public ApplicationDbContext DbContext { get; init; }

        public void Dispose()
        {
            Scope.Dispose();
        }
    }
}
