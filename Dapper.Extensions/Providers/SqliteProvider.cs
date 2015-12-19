using System;
using System.Collections.Generic;
using System.Text;

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

        public override string GetLimitOffsetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var result = string.Format("{0} LIMIT @_Offset, @_Count", sql);
            parameters.Add("_Offset", firstResult);
            parameters.Add("_Count", maxResults);
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
