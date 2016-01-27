using System.Collections.Generic;

namespace Dapper.Extensions
{
    public interface ISqlGenerator
    {
        IDbProvider DbProvider { get; }
        string BuildSelectColumns(IClassMapper classMapper);
        SqlConvertResult Count(IClassMapper classMapper, string condition, DynamicParameters dynamicParameters);
        SqlConvertResult Delete(IClassMapper classMapper, string condition, DynamicParameters dynamicParameters);
        KeyConditionResult GetKeyConditionByEntity<T>(IClassMapper classMapper, T entity) where T : class;
        KeyConditionResult GetKeyConditionById(IClassMapper classMapper, object id);

        string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias);
        string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias);
        string GetTableName(IClassMapper classMapper);
        string IdentitySql(IClassMapper classMapper);
        SqlConvertResult Insert(IClassMapper classMapper, DynamicParameters dynamicParameters);
        SqlConvertResult Select(IClassMapper classMapper, string condition, string orderBy, DynamicParameters dynamicParameters, bool hasNoLock = true);
        SqlConvertResult Select(IClassMapper classMapper, int firstResult, int maxResults, string condition, string orderBy, DynamicParameters dynamicParameters);
        SqlConvertResult SqlConvert(IClassMapper classMapper, string sql, DynamicParameters dynamicParameters);

        string SqlConvert(IClassMapper classMapper, string sql);
        bool SupportsMultipleStatements();
        SqlConvertResult Update(IClassMapper classMapper, string condition, DynamicParameters dynamicParameters, object entity = null);
        SqlConvertResult Update(string tableName, IList<string> updateFields, string condition,DynamicParameters dynamicParameters);
        string ReplaceQuote(string sql);

        /// <summary>
        /// 检查输入字符串是否有sql关键字。
        /// </summary>
        /// <param name="sql">字符串内容</param>
        /// <returns>包含sql关键字时返回true,否则返回false</returns>
        bool CheckSqlKeyWord(string sql);
    }
}
