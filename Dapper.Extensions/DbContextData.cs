using System.Data.Common;

namespace Dapper.Extensions
{
    public class DbContextData
    {
        public IDbProvider DbProvider { get; set; }

        public DbProviderFactory DbProviderFactory { get; set; }

        public string ConnectionString { get; set; }
    }
}
