using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dapper.Extensions
{
    public class SqlGenerator : ISqlGenerator
    {
        public SqlGenerator(IDbProvider dbProvider)
        {
            DbProvider = dbProvider;
        }

        public IDbProvider DbProvider { get; private set; }

        public virtual bool SupportsMultipleStatements()
        {
            return DbProvider.SupportsMultipleStatements;
        }

        public virtual string GetTableName(IClassMapper classMapper)
        {
            return DbProvider.GetTableName(classMapper.SchemaName, classMapper.TableName, null);
        }

        public virtual string IdentitySql(IClassMapper classMapper)
        {
            return DbProvider.GetIdentitySql(GetTableName(classMapper));
        }

        public virtual string GetColumnName(IClassMapper classMapper, IPropertyMap property, bool includeAlias)
        {
            string alias = null;
            if (property.ColumnName != property.Name && includeAlias)
            {
                alias = property.Name;
            }
            return DbProvider.GetColumnName(null, property.ColumnName, alias);
        }

        public virtual string GetColumnName(IClassMapper classMapper, string propertyName, bool includeAlias)
        {
            IPropertyMap propertyMap = classMapper.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyMap == null)
            {
                throw new ArgumentException(string.Format("未能找到属性 '{0}' 的映射。", propertyName));
            }

            return GetColumnName(classMapper, propertyMap, includeAlias);
        }

        public virtual string BuildSelectColumns(IClassMapper classMapper)
        {
            var columns = classMapper.Properties
                .Where(p => !p.IsIgnored)
                .Select(p => GetColumnName(classMapper, p, true));
            return columns.AppendStrings();
        }

        public virtual KeyConditionResult GetKeyConditionByEntity<T>(IClassMapper classMapper, T entity) where T : class
        {
            var whereFields = classMapper.Properties.Where(p => p.KeyType != KeyType.NotAKey);
            if (!whereFields.Any())
            {
                throw new ArgumentException("必须定义一个主键列。");
            }
            KeyConditionResult result = new KeyConditionResult { Parameters = new DynamicParameters() };
            List<string> list = new List<string>();
            foreach (IPropertyMap property in whereFields)
            {
                list.Add(string.Concat(DbProvider.OpenQuote, property.ColumnName, DbProvider.CloseQuote, "=", DbProvider.ParameterPrefix, property.Name));
                result.Parameters.Add(property.Name, property.PropertyInfo.GetValue(entity, null), property.DbType, null, property.Size);
            }
            result.Sql = list.AppendStrings(" AND ");
            return result;
        }

        public virtual KeyConditionResult GetKeyConditionById(IClassMapper classMapper, object id)
        {
            bool isSimpleType = ReflectionHelper.IsSimpleType(id.GetType());
            KeyConditionResult result = new KeyConditionResult { Parameters = new DynamicParameters() };
            if (isSimpleType)
            {
                IPropertyMap property = classMapper.Properties.Single(p => p.KeyType != KeyType.NotAKey);
                result.Sql = string.Concat(DbProvider.OpenQuote, property.ColumnName, DbProvider.CloseQuote, "=", DbProvider.ParameterPrefix, property.Name);
                result.Parameters.Add(property.Name, id, property.DbType, null, property.Size);
            }
            else
            {
                var keys = classMapper.Properties.Where(p => p.KeyType != KeyType.NotAKey);
                IDictionary<string, object> paramValues = ReflectionHelper.GetObjectValues(id);
                List<string> list = new List<string>();
                foreach (IPropertyMap property in keys)
                {
                    list.Add(string.Concat(DbProvider.OpenQuote, property.ColumnName, DbProvider.CloseQuote, "=", DbProvider.ParameterPrefix, property.Name));
                    result.Parameters.Add(property.Name, paramValues[property.Name], property.DbType, null, property.Size);
                }
                result.Sql = list.AppendStrings(" AND ");
            }
            return result;
        }

        public virtual string SqlConvert(IClassMapper classMapper, string sql)
        {
            SqlConvertResult result = SqlConvert(classMapper, sql, null);
            return result == null ? string.Empty : result.Sql;
        }

        public virtual SqlConvertResult SqlConvert(IClassMapper classMapper, string sql, DynamicParameters dynamicParameters)
        {
            SqlConvertResult result = new SqlConvertResult();
            if (!string.IsNullOrWhiteSpace(sql) && sql.Contains("#"))
            {
                foreach (IPropertyMap property in classMapper.Properties)
                {
                    if (property == null)
                        continue;
                    sql = sql.Replace("#" + property.Name, string.Concat(DbProvider.OpenQuote, property.ColumnName, DbProvider.CloseQuote));
                }
            }
            result.Sql = sql;
            if (dynamicParameters == null) return result;
            result.Parameters = dynamicParameters;
            return result;
        }

        public virtual SqlConvertResult Select(IClassMapper classMapper, string condition, string orderBy, DynamicParameters dynamicParameters, bool hasNoLock = true)
        {
            SqlConvertResult result = SqlConvert(classMapper, condition, dynamicParameters);
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ").Append(BuildSelectColumns(classMapper)).Append(" FROM ").Append(GetTableName(classMapper));
            if (hasNoLock)
                sql.Append(DbProvider.NoLock);

            if (!string.IsNullOrWhiteSpace(condition))
            {
                sql.Append(" WHERE ").Append(result.Sql);
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                sql.Append(" ORDER BY ").Append(orderBy);
            }
            result.Sql = sql.ToString();
            return result;
        }

        public virtual SqlConvertResult Count(IClassMapper classMapper, string condition, DynamicParameters dynamicParameters)
        {
            SqlConvertResult result = SqlConvert(classMapper, condition, dynamicParameters);
            StringBuilder sql = new StringBuilder(string.Format("SELECT COUNT(*) AS {0}Total{1} FROM {2}",
                DbProvider.OpenQuote, DbProvider.CloseQuote, GetTableName(classMapper)));
            if (!string.IsNullOrWhiteSpace(condition))
            {
                sql.Append(" WHERE ").Append(result.Sql);
            }
            result.Sql = sql.ToString();
            return result;
        }

        public virtual SqlConvertResult Insert(IClassMapper classMapper, DynamicParameters dynamicParameters)
        {
            SqlConvertResult result = SqlConvert(classMapper, null, dynamicParameters);
            var columnMaps = classMapper.GetColumnMaps();
            if (!columnMaps.Any())
            {
                throw new ArgumentException("没有被映射的列。");
            }

            var columnNames = columnMaps.Select(p => GetColumnName(classMapper, p, false));
            var parameterNames = columnMaps.Select(p => DbProvider.ParameterPrefix + p.Name);

            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", GetTableName(classMapper),
                columnNames.AppendStrings(), parameterNames.AppendStrings());
            result.Sql = sql;
            return result;
        }

        public virtual SqlConvertResult Update(IClassMapper classMapper, string condition, DynamicParameters dynamicParameters, object entity = null)
        {
            if (dynamicParameters == null)
            {
                throw new ArgumentNullException("dynamicParameters");
            }

            var columnMaps = classMapper.GetColumnMaps();
            if (!columnMaps.Any())
            {
                throw new ArgumentException("没有被映射的列。");
            }

            if (entity != null)
            {
                IPropertyMap propertyChangedListMap = classMapper.GetPropertyChangedListMap();
                if (propertyChangedListMap != null)
                {
                    IList<string> propertyChangedList = propertyChangedListMap.PropertyInfo.GetValue(entity, null) as IList<string>;
                    if (propertyChangedList != null && propertyChangedList.Count > 0)
                    {
                        propertyChangedList = propertyChangedList.Distinct().ToList();
                        IList<IPropertyMap> propertyMaps = new List<IPropertyMap>();
                        foreach (string name in propertyChangedList)
                        {
                            if (string.IsNullOrWhiteSpace(name))
                                continue;
                            IPropertyMap propertyMap = columnMaps.FirstOrDefault(m => m != null && string.Compare(m.Name, name.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
                            if (propertyMap != null)
                                propertyMaps.Add(propertyMap);
                        }
                        columnMaps = propertyMaps;
                    }
                }
            }

            SqlConvertResult result = SqlConvert(classMapper, condition, dynamicParameters);
            var setSql = columnMaps.Select(p =>
                string.Format("{0} = {1}{2}", GetColumnName(classMapper, p, false), DbProvider.ParameterPrefix, p.Name));

            string updateSql = string.Format("UPDATE {0} SET {1} WHERE {2}", GetTableName(classMapper),
                setSql.AppendStrings(), result.Sql);
            result.Sql = updateSql;
            return result;
        }

        public virtual SqlConvertResult Update(string tableName, IList<string> updateFields, string condition,
            DynamicParameters dynamicParameters)
        {
            if (dynamicParameters == null)
            {
                throw new ArgumentNullException("dynamicParameters");
            }
            if (updateFields == null)
            {
                throw new ArgumentNullException("updateFields");
            }

            var setSql = updateFields.Select(p =>
                string.Format("{0} = {1}{2}", p, DbProvider.ParameterPrefix, p));
            string updateSql = string.Format("UPDATE {0} SET {1} WHERE {2}", tableName,
                setSql.AppendStrings(), condition);
            SqlConvertResult result = new SqlConvertResult
            {
                Sql = updateSql,
                Parameters = dynamicParameters
            };
            return result;
        }

        public virtual SqlConvertResult Delete(IClassMapper classMapper, string condition, DynamicParameters dynamicParameters)
        {
            if (dynamicParameters == null)
            {
                throw new ArgumentNullException("dynamicParameters");
            }
            SqlConvertResult result = SqlConvert(classMapper, condition, dynamicParameters);
            StringBuilder sql = new StringBuilder(string.Format("DELETE FROM {0}", GetTableName(classMapper)));
            sql.Append(" WHERE ").Append(result.Sql);
            result.Sql = sql.ToString();
            return result;
        }

        public virtual SqlConvertResult Select(IClassMapper classMapper, int firstResult, int maxResults, string condition, string orderBy, DynamicParameters dynamicParameters)
        {
            if (dynamicParameters == null)
                dynamicParameters = new DynamicParameters();
            SqlConvertResult sqlConvertResult = Select(classMapper, condition, orderBy, null);
            string sql = DbProvider.GetLimitOffsetSql(sqlConvertResult.Sql, firstResult, maxResults, dynamicParameters);
            sqlConvertResult = SqlConvert(classMapper, sql, dynamicParameters);
            return sqlConvertResult;
        }

        public virtual string ReplaceQuote(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;
            if (DbProvider.OpenQuote != '[' && DbProvider.CloseQuote != ']')
            {
                sql = sql.Replace('[', DbProvider.OpenQuote);
                sql = sql.Replace(']', DbProvider.CloseQuote);
            }
            if (DbProvider.ParameterPrefix != '@')
            {
                sql = sql.Replace('@', DbProvider.ParameterPrefix);
            }
            return sql;
        }

        /// <summary>
        /// 检查输入字符串是否有sql关键字。
        /// </summary>
        /// <param name="sql">字符串内容</param>
        /// <returns>包含sql关键字时返回true,否则返回false</returns>
        public virtual bool CheckSqlKeyWord(string sql)
        {
            if (sql == null)
                return false;
            const string pattern = @"<|>|select|insert|delete|from|count\(|drop table|update|truncate|asc\(|mid\(|char\(|xp_cmdshell|exec|master|netlocalgroup administrators|:|net user|""|or|and|[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']";
            return Regex.IsMatch(sql, pattern);
        }
    }

    public class SqlConvertResult
    {
        public string Sql { get; set; }

        public DynamicParameters Parameters { get; set; }
    }

    public class KeyConditionResult
    {
        public string Sql { get; set; }

        public DynamicParameters Parameters { get; set; }
    }
}