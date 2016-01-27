using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Extensions
{
    public interface IDbProvider
    {
        /// <summary>
        /// 驱动名称
        /// </summary>
        string ProviderName { get; }
        char OpenQuote { get; }
        char CloseQuote { get; }
        string BatchSeperator { get; }
        bool SupportsMultipleStatements { get; }
        char ParameterPrefix { get; }
        string EmptyExpression { get; }
        string GetTableName(string schemaName, string tableName, string alias);
        string GetColumnName(string prefix, string columnName, string alias);
        string GetIdentitySql(string tableName);
        string GetLimitOffsetSql(string sql, int firstResult, int maxResults, DynamicParameters dynamicParameters);
        bool IsQuoted(string value);
        string QuoteString(string value);

        string NoLock { get; }
    }

    public abstract class AbstractDbProvider : IDbProvider
    {
        /// <summary>
        /// 驱动名称
        /// </summary>
        public virtual string ProviderName
        {
            get { return "System.Data.SqlClient"; }
        }

        public virtual string NoLock
        {
            get { return string.Empty; }
        }

        public virtual char OpenQuote
        {
            get { return '"'; }
        }

        public virtual char CloseQuote
        {
            get { return '"'; }
        }

        public virtual string BatchSeperator
        {
            get { return ";" + Environment.NewLine; }
        }

        public virtual bool SupportsMultipleStatements
        {
            get { return true; }
        }

        public virtual char ParameterPrefix
        {
            get { return '@'; }
        }

        public string EmptyExpression
        {
            get { return "1=1"; }
        }

        public virtual string GetTableName(string schemaName, string tableName, string alias)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "表名不能为空。");
            }

            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                result.AppendFormat(QuoteString(schemaName) + ".");
            }

            result.AppendFormat(QuoteString(tableName));

            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }
            return result.ToString();
        }

        public virtual string GetColumnName(string prefix, string columnName, string alias)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentNullException("columnName", "列名不能为空。");
            }

            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                result.AppendFormat(QuoteString(prefix) + ".");
            }

            result.AppendFormat(QuoteString(columnName));

            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }

            return result.ToString();
        }

        public abstract string GetIdentitySql(string tableName);

        public abstract string GetLimitOffsetSql(string sql, int firstResult, int maxResults, DynamicParameters dynamicParameters);

        public virtual bool IsQuoted(string value)
        {
            if (value.Trim()[0] == OpenQuote)
            {
                return value.Trim().Last() == CloseQuote;
            }

            return false;
        }

        public virtual string QuoteString(string value)
        {
            return IsQuoted(value) ? value : string.Format("{0}{1}{2}", OpenQuote, value.Trim(), CloseQuote);
        }

        public virtual string UnQuoteString(string value)
        {
            return IsQuoted(value) ? value.Substring(1, value.Length - 2) : value;
        }
    }
}