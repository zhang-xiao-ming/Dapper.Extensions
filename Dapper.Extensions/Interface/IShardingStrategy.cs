namespace Dapper.Extensions
{
    public interface IShardingStrategy
    {
        object ShardingParamers { get; set; }

        string GetConnectionName();

        string GetTableName();
    }
}
