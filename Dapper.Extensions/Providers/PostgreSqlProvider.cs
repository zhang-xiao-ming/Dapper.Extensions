using System;
using System.Collections.Generic;

namespace Dapper.Extensions
{
    public class PostgreSqlProvider : AbstractDbProvider
    {
        /// <summary>
        /// 驱动名称
        /// </summary>
        public override string ProviderName
        {
            get { return "Npgsql"; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT LASTVAL() AS Id";
        }


        public override string GetLimitOffsetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            string result = string.Format("{0} LIMIT @_firstResult OFFSET @_pageStartRowNbr", sql);
            parameters.Add("_firstResult", firstResult);
            parameters.Add("_maxResults", maxResults);
            return result;
        }

        public override string GetColumnName(string prefix, string columnName, string alias)
        {
            return base.GetColumnName(null, columnName, alias).ToLower();
        }

        public override string GetTableName(string schemaName, string tableName, string alias)
        {
            return base.GetTableName(schemaName, tableName, alias).ToLower();
        }
    }

}