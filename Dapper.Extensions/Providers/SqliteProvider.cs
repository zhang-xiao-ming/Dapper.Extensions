using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace Dapper.Extensions
{
    public class SqliteProvider : AbstractDbProvider
    {
        /// <summary>
        /// 驱动名称
        /// </summary>
        public override string ProviderName
        {
            get { return "System.Data.SQLite"; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT LAST_INSERT_ROWID() AS [Id]";
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

            var result = string.Format("{0} LIMIT @_Offset, @_Count", sql);
            dynamicParameters.Add("_Offset", firstResult,DbType.Int32);
            dynamicParameters.Add("_Count", maxResults,DbType.Int32);
            return result;
        }

        public override string GetColumnName(string prefix, string columnName, string alias)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentNullException(columnName, "列名不能为空 。");
            }
            var result = new StringBuilder();
            result.AppendFormat(columnName);
            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }
            return result.ToString();
        }
    }
}
