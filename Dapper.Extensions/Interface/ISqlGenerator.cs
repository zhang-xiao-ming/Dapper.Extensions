using System.Collections.Generic;

namespace Dapper.Extensions
{
    public interface ISqlGenerator
    {
        IDbProvider DbProvider { get; }
        string BuildSelectColumns(IClassMapper classMapper);
        SqlConvertResult Count(IClassMapper classMapper, string condition, IDictionary<string, object> parameters);
        SqlConvertResult Delete(IClassMapper classMapper, string condition, IDictionary<string, object> parameters);
        KeyConditionResult GetKeyConditionByEntity<T>(IClassMapper classMapper, T entity) where T : class;
        KeyConditionResult GetKeyConditionById(IClassMapper classMapper, object id);

        string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias);
        string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias);
        string GetTableName(IClassMapper classMapper);
        string IdentitySql(IClassMapper classMapper);
        SqlConvertResult Insert(IClassMapper classMapper, IDictionary<string, object> parameters);
        SqlConvertResult Select(IClassMapper classMapper, string condition, string orderBy, IDictionary<string, object> parameters, bool hasNoLock = true);
        SqlConvertResult Select(IClassMapper classMapper, int firstResult, int maxResults, string condition, string orderBy, IDictionary<string, object> parameters);
        SqlConvertResult SqlConvert(IClassMapper classMapper, string sql, IDictionary<string, object> parameters);

        string SqlConvert(IClassMapper classMapper, string sql);
        bool SupportsMultipleStatements();
        SqlConvertResult Update(IClassMapper classMapper, string condition, IDictionary<string, object> parameters);

        string ReplaceQuote(string sql);

        /// <summary>
        /// 检查输入字符串是否有sql关键字。
        /// </summary>
        /// <param name="sql">字符串内容</param>
        /// <returns>包含sql关键字时返回true,否则返回false</returns>
        bool CheckSqlKeyWord(string sql);
    }
}
