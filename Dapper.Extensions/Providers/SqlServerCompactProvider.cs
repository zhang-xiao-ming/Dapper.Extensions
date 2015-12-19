using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Extensions
{
    public class SqlServerCompactProvider : AbstractDbProvider
    {
        public override string ProviderName
        {
            get { return "System.Data.SqlServerCe.4.0"; }
        }

        public override char OpenQuote
        {
            get { return '['; }
        }

        public override char CloseQuote
        {
            get { return ']'; }
        }

        public override bool SupportsMultipleStatements
        {
            get { return false; }
        }

        public override string GetTableName(string schemaName, string tableName, string alias)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            StringBuilder result = new StringBuilder();
            result.Append(OpenQuote);
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                result.AppendFormat("{0}_", schemaName);
            }

            result.AppendFormat("{0}{1}", tableName, CloseQuote);


            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}{1}{2}", OpenQuote, alias, CloseQuote);
            }

            return result.ToString();
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CAST(@@IDENTITY AS BIGINT) AS [Id]";
        }


        public override string GetLimitOffsetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            string result = string.Format("{0} OFFSET @_firstResult ROWS FETCH NEXT @_maxResults ROWS ONLY", sql);
            parameters.Add("_firstResult", firstResult);
            parameters.Add("_maxResults", maxResults);
            return result;
        }
    }
}