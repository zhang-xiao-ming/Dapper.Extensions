using System;
using System.Collections.Generic;
using System.Data;
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


        public override string GetLimitOffsetSql(string sql, int firstResult, int maxResults, DynamicParameters dynamicParameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            if (dynamicParameters == null)
            {
                throw new ArgumentNullException("dynamicParameters");
            }
            string result = string.Format("{0} LIMIT @_firstResult OFFSET @_pageStartRowNbr", sql);
            dynamicParameters.Add("_firstResult", firstResult,DbType.Int32);
            dynamicParameters.Add("_maxResults", maxResults, DbType.Int32);
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