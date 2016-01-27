using System;
using System.Collections.Generic;

namespace Dapper.Extensions
{
    public class MySqlProvider : AbstractDbProvider
    {
        /// <summary>
        /// 驱动名称
        /// </summary>
        public override string ProviderName
        {
            get { return "MySql.Data.MySqlClient"; }
        }

        public override char OpenQuote
        {
            get { return '`'; }
        }

        public override char CloseQuote
        {
            get { return '`'; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID";
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
            string result = string.Format("{0} LIMIT @_firstResult, @_maxResults", sql);
            dynamicParameters.Add("_firstResult", firstResult);
            dynamicParameters.Add("_maxResults", maxResults);
            return result;
        }
    }
}