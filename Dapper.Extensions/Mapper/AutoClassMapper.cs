using System;

namespace Dapper.Extensions
{
    public sealed class AutoClassMapper<T> : ClassMapper<T> where T : class
    {
        public AutoClassMapper()
        {
            Type type = typeof(T);
            TableName = type.Name;
            AutoMap();
        }
    }
}