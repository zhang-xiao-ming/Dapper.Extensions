using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;

namespace Dapper.Extensions
{
    public class DbContext : IDbContext
    {

        private static readonly Dictionary<string, DbContextData> DbContextDatas = new Dictionary<string, DbContextData>();
        private bool _disposed;
        private static readonly object SyncRoot = new object();
        public string ConnectionName { get; private set; }

        public DbContext(string connectionName = "Default", string databaseName = null)
        {
            DbContextData data = GetDbContextData(connectionName, databaseName);
            SqlGenerator = new SqlGenerator(data.DbProvider);
            DbConnection = data.DbProviderFactory.CreateConnection();
            if (DbConnection == null)
                throw new ArgumentNullException("connectionName");
            DbConnection.ConnectionString = data.ConnectionString;
            if (DbConnection.State != ConnectionState.Open)
            {
                DbConnection.Open();
            }
        }

        private DbContextData CreateDbContextData(string connectionName = "Default", string databaseName = null)
        {
            DbContextData data = new DbContextData();
            ConnectionName = connectionName;
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[connectionName];
            if (setting == null)
                throw new Exception(string.Format("名为:{0}的节点不存在！", connectionName));

            IDecryptProvider decryptProvider = null;
            string strDecryptProvider = ConfigurationManager.AppSettings["Dapper.Extensions.DecryptProvider"];
            if (!string.IsNullOrWhiteSpace(strDecryptProvider))
            {
                Type encryptProviderType = Type.GetType(strDecryptProvider, true);
                decryptProvider = (IDecryptProvider)Activator.CreateInstance(encryptProviderType);
            }
            string connectionString = setting.ConnectionString;
            if (decryptProvider != null)
            {
                connectionString = decryptProvider.Decrypt(setting.ConnectionString);
            }
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                connectionString = string.Format(connectionString, databaseName);
            }
            Type dbConnectionType = Type.GetType(setting.ProviderName, true);
            IDbProvider dbProvider = (IDbProvider)Activator.CreateInstance(dbConnectionType);
            DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(dbProvider.ProviderName);
            data.ConnectionString = connectionString;
            data.DbProvider = dbProvider;
            data.DbProviderFactory = dbProviderFactory;
            return data;
        }
        private DbContextData GetDbContextData(string connectionName = "Default", string databaseName = null)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                connectionName = "Default";
            }
            string key = string.Format("{0}_{1}", connectionName, databaseName);
            DbContextData data;
            DbContextDatas.TryGetValue(key, out data);
            if (data != null)
                return data;
            if (!DbContextDatas.ContainsKey(key))
            {
                lock (SyncRoot)
                {
                    if (!DbContextDatas.ContainsKey(key))
                    {
                        data = CreateDbContextData(connectionName, databaseName);
                        DbContextDatas.Add(key, data);
                    }
                }
            }
            return data ?? CreateDbContextData(connectionName, databaseName);
        }

        public IDbConnection DbConnection { get; private set; }
        public IDbTransaction DbTransaction { get; private set; }
        public ISqlGenerator SqlGenerator { get; private set; }

        #region DbTransaction
        public IDbContext UseDbTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            DbTransaction = DbConnection.BeginTransaction(isolationLevel);
            return this;
        }

        public void Commit()
        {
            if (DbTransaction == null)
                return;
            DbTransaction.Commit();
            DbTransaction = null;
        }

        public void Rollback()
        {
            if (DbTransaction == null)
                return;
            DbTransaction.Rollback();
            DbTransaction = null;
        }

        #endregion

        #region CURD

        public bool Insert<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class
        {
            bool flag;
            IClassMapper<T> classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            classMapper.BeforeSave(entity);
            var identityColumn = classMapper.Properties.SingleOrDefault(p => p.KeyType == KeyType.Identity);
            IDictionary<string, object> keyValues = new ExpandoObject();
            SqlConvertResult sqlConvertResult = SqlGenerator.Insert(classMapper, null);
            if (identityColumn != null)
            {
                IEnumerable<long> result;
                if (SqlGenerator.SupportsMultipleStatements())
                {
                    sqlConvertResult.Sql += SqlGenerator.DbProvider.BatchSeperator + SqlGenerator.IdentitySql(classMapper);
                    result = DbConnection.Query<long>(sqlConvertResult.Sql, entity, DbTransaction, false, commandTimeout, CommandType.Text);
                }
                else
                {
                    DbConnection.Execute(sqlConvertResult.Sql, entity, DbTransaction, commandTimeout, CommandType.Text);
                    sqlConvertResult.Sql = SqlGenerator.IdentitySql(classMapper);
                    result = DbConnection.Query<long>(sqlConvertResult.Sql, entity, DbTransaction, false, commandTimeout, CommandType.Text);
                }

                long identityValue = result.First();
                int identityInt = Convert.ToInt32(identityValue);
                keyValues.Add(identityColumn.Name, identityInt);
                identityColumn.PropertyInfo.SetValue(entity, identityInt, null);
                flag = identityInt > 0;

            }
            else
            {
                flag = DbConnection.Execute(sqlConvertResult.Sql, entity, DbTransaction, commandTimeout, CommandType.Text) > 0;
            }
            if (flag)
            {
                classMapper.AfterSave(entity);
            }

            return flag;
        }

        public bool Insert<T>(IEnumerable<T> entities, string tableName = null, int? commandTimeout = null) where T : class
        {
            IClassMapper<T> classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            if (entities != null)
            {
                foreach (T entity in entities)
                {
                    classMapper.BeforeSave(entity);
                }
            }
            SqlConvertResult sqlConvertResult = SqlGenerator.Insert(classMapper, null);
            bool flag = DbConnection.Execute(sqlConvertResult.Sql, entities, DbTransaction, commandTimeout, CommandType.Text) > 0;
            if (flag && entities != null)
            {
                foreach (T entity in entities)
                {
                    classMapper.AfterSave(entity);
                }
            }
            return flag;
        }

        public bool Update<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class
        {
            IClassMapper<T> classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            classMapper.BeforeSave(entity);
            KeyConditionResult keyConditionResult = SqlGenerator.GetKeyConditionByEntity(classMapper, entity);
            if (classMapper.GetVersionMap() != null)
            {
                int oldVersion = (int)classMapper.GetVersionMap().PropertyInfo.GetValue(entity, null);
                keyConditionResult.Sql = string.Format("{0} AND {1}={2}", keyConditionResult.Sql,
                    SqlGenerator.DbProvider.QuoteString(classMapper.GetVersionMap().ColumnName), oldVersion);
                classMapper.GetVersionMap().PropertyInfo.SetValue(entity, oldVersion + 1, null);
            }
            SqlConvertResult sqlConvertResult = SqlGenerator.Update(classMapper, keyConditionResult.Sql, keyConditionResult.Parameters);
            bool flag = DbConnection.Execute(sqlConvertResult.Sql, entity, DbTransaction, commandTimeout, CommandType.Text) > 0;
            if (flag)
            {
                classMapper.AfterSave(entity);
            }
            return flag;
        }

        public bool Delete<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class
        {
            IClassMapper classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            KeyConditionResult keyConditionResult = SqlGenerator.GetKeyConditionByEntity(classMapper, entity);
            SqlConvertResult sqlConvertResult = SqlGenerator.Delete(classMapper, keyConditionResult.Sql, keyConditionResult.Parameters);
            return DbConnection.Execute(sqlConvertResult.Sql, entity, DbTransaction, commandTimeout, CommandType.Text) > 0;
        }

        public bool Delete<T>(object id, string tableName = null, int? commandTimeout = null) where T : class
        {
            IClassMapper classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            KeyConditionResult keyConditionResult = SqlGenerator.GetKeyConditionById(classMapper, id);
            SqlConvertResult sqlConvertResult = SqlGenerator.Delete(classMapper, keyConditionResult.Sql, keyConditionResult.Parameters);
            return DbConnection.Execute(sqlConvertResult.Sql, sqlConvertResult.Parameters, DbTransaction, commandTimeout, CommandType.Text) > 0;
        }

        public T Get<T>(object id, string tableName = null, int? commandTimeout = null) where T : class
        {
            IClassMapper classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            KeyConditionResult keyConditionResult = SqlGenerator.GetKeyConditionById(classMapper, id);
            SqlConvertResult sqlConvertResult = SqlGenerator.Select(classMapper, keyConditionResult.Sql,null, keyConditionResult.Parameters, false);
            T result = DbConnection.Query<T>(sqlConvertResult.Sql, sqlConvertResult.Parameters, DbTransaction, true, commandTimeout, CommandType.Text).SingleOrDefault();
            if (result == null) return null;
            IPropertyMap persistedMap = classMapper.GetPersistedMap();
            if (persistedMap != null) persistedMap.PropertyInfo.SetValue(result, true, null);
            return result;
        }

        public bool Save<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class
        {
            IClassMapper classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            IPropertyMap persistedMap = classMapper.GetPersistedMap();
            if (persistedMap == null)
                throw new Exception(string.Format("{0}没有映射持久化标识列，无法使用此方法！", typeof(T).FullName));
            bool isPersisted = (bool)persistedMap.PropertyInfo.GetValue(entity, null);
            return isPersisted ? Update(entity, tableName, commandTimeout) : Insert(entity, tableName, commandTimeout);
        }

        public IList<T> List<T>(string condition, Dictionary<string, object> parameters, int? commandTimeout = null) where T : class
        {
            return List<T>(null, condition, parameters, commandTimeout);
        }
        public IList<T> List<T>(string tableName, string condition,IDictionary<string, object> parameters, int? commandTimeout = null) where T : class
        {
            IClassMapper classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            SqlConvertResult sqlConvertResult = SqlGenerator.Select(classMapper, condition, null,parameters);
            return DbConnection.Query<T>(sqlConvertResult.Sql, sqlConvertResult.Parameters, DbTransaction, true, commandTimeout, CommandType.Text).ToList();
        }

        public IList<T> List<T>(string tableName, string condition, object parameters, int? commandTimeout = null) where T : class
        {
            IDictionary<string, object> paramValues = ReflectionHelper.GetObjectValues(parameters);
            return List<T>(tableName, condition,paramValues, commandTimeout);
        }

        public IList<T> List<T>(string condition, object parameters, int? commandTimeout = null) where T : class
        {
            return List<T>(null, condition,parameters, commandTimeout);
        }

        public IList<T> List<T>(string condition, string orderBy, IDictionary<string, object> parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class
        {
            return List<T>(null, condition,orderBy, parameters, firstResult,maxResults, commandTimeout);
        }
        public IList<T> List<T>(string tableName, string condition, string orderBy,IDictionary<string, object> parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class
        {
            IClassMapper classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            SqlConvertResult sqlConvertResult = SqlGenerator.Select(classMapper, firstResult, maxResults, condition,orderBy, parameters);
            return DbConnection.Query<T>(sqlConvertResult.Sql, sqlConvertResult.Parameters, DbTransaction, true, commandTimeout, CommandType.Text).ToList();
        }

        public IList<T> List<T>(string tableName, string condition, string orderBy, object parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class
        {
            IDictionary<string, object> paramValues = ReflectionHelper.GetObjectValues(parameters);
            return List<T>(tableName, condition, orderBy, paramValues, firstResult, maxResults, commandTimeout);
        }

        public IList<T> List<T>(string condition, string orderBy, object parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class
        {
            return List<T>(null, condition, orderBy,parameters, firstResult, maxResults, commandTimeout);
        }

        public int Count<T>(string condition, IDictionary<string, object> parameters, int? commandTimeout = null) where T : class
        {
            return Count<T>(null, condition, parameters, commandTimeout);
        }
        public int Count<T>(string tableName, string condition, IDictionary<string, object> parameters, int? commandTimeout = null) where T : class
        {
            IClassMapper classMapper = ClassMapperFactory.GetMapper<T>(tableName);
            SqlConvertResult sqlConvertResult = SqlGenerator.Count(classMapper, condition, parameters);
            return DbConnection.Query<int>(sqlConvertResult.Sql, sqlConvertResult.Parameters, DbTransaction, true, commandTimeout, CommandType.Text).SingleOrDefault();
        }

        public int Count<T>(string tableName, string condition, object parameters, int? commandTimeout = null) where T : class
        {
            IDictionary<string, object> paramValues = ReflectionHelper.GetObjectValues(parameters);
            return Count<T>(tableName, condition, paramValues, commandTimeout);
        }

        public int Count<T>(string condition, object parameters, int? commandTimeout = null) where T : class
        {
            return Count<T>(null, condition, parameters, commandTimeout);
        }


        public PagingResult<T> Paging<T>(string condition, string orderBy, IDictionary<string, object> parameters, int pageIndex, int pageSize, int? commandTimeout = null) where T : class
        {
            return Paging<T>(null, condition, orderBy, parameters, pageIndex, pageSize, commandTimeout);
        }

        public PagingResult<T> Paging<T>(string tableName, string condition, string orderBy, IDictionary<string, object> parameters, int pageIndex, int pageSize, int? commandTimeout = null) where T : class
        {
            int startValue = (pageIndex-1) * pageSize;
            int totalRecords = Count<T>(tableName, condition, parameters, commandTimeout);
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            IList<T> list = List<T>(tableName, condition, orderBy,parameters, startValue, pageSize, commandTimeout);
            PagingResult<T> result = new PagingResult<T>
            {
                List = list,
                TotalRecords = totalRecords,
                TotalPages= totalPages
            };
            return result;
        }

        public PagingResult<T> Paging<T>(string tableName, string condition, object parameters, int pageIndex, int pageSize, int? commandTimeout = null) where T : class
        {
            IDictionary<string, object> paramValues = ReflectionHelper.GetObjectValues(parameters);
            return Paging<T>(tableName, condition, paramValues, pageIndex, pageSize, commandTimeout);
        }

        public PagingResult<T> Paging<T>(string condition, object parameters, int pageIndex, int pageSize, int? commandTimeout = null) where T : class
        {
            return Paging<T>(null, condition, parameters, pageIndex, pageSize, commandTimeout);
        }

        public DataTable GetDataTable(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            IDataReader dr = ExecuteReader(sql, param, commandTimeout, commandType);
            return GetDataTableFromIDataReader(dr);
        }

        private DataTable GetDataTableFromIDataReader(IDataReader reader)
        {
            DataTable dt = new DataTable();
            bool init = false;
            dt.BeginLoadData();
            object[] vals = new object[0];
            while (reader.Read())
            {
                if (!init)
                {
                    init = true;
                    int fieldCount = reader.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                    }
                    vals = new object[fieldCount];
                }
                reader.GetValues(vals);
                dt.LoadDataRow(vals, true);
            }
            reader.Close();
            dt.EndLoadData();
            return dt;
        }

        #endregion

        #region Dapper

        public int Execute(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Execute(sql, param, DbTransaction, commandTimeout, commandType);
        }

        public IDataReader ExecuteReader(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.ExecuteReader(sql, param, DbTransaction, commandTimeout, commandType);
        }


        public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.ExecuteScalar<T>(sql, param, DbTransaction, commandTimeout, commandType);
        }

        public object ExecuteScalar(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.ExecuteScalar(sql, param, DbTransaction, commandTimeout, commandType);
        }

        public IList<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(sql, param, DbTransaction, buffered, commandTimeout, commandType).ToList();
        }

        public IList<object> Query(Type type, string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(type, sql, param, DbTransaction, buffered, commandTimeout, commandType).ToList();
        }

        public IList<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query<T>(sql, param, DbTransaction, buffered, commandTimeout, commandType).ToList();
        }

        public IList<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(sql, map, param, DbTransaction, buffered, splitOn, commandTimeout, commandType).ToList();
        }

        public IList<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(sql, map, param, DbTransaction, buffered, splitOn, commandTimeout, commandType).ToList();
        }

        public IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(sql, map, param, DbTransaction, buffered, splitOn, commandTimeout, commandType).ToList();
        }

        public IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(sql, map, param, DbTransaction, buffered, splitOn, commandTimeout, commandType).ToList();
        }

        public IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(sql, map, param, DbTransaction, buffered, splitOn, commandTimeout, commandType).ToList();
        }

        public IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.Query(sql, map, param, DbTransaction, buffered, splitOn, commandTimeout, commandType).ToList();
        }

        public SqlMapper.GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            sql = SqlGenerator.ReplaceQuote(sql);
            return DbConnection.QueryMultiple(sql, param, DbTransaction, commandTimeout, commandType);
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_disposed || !isDisposing) return;
            Close();
            _disposed = true;
        }

        public void Close()
        {
            if (DbConnection == null)
                return;

            Rollback();

            if (DbConnection.State != ConnectionState.Closed)
                DbConnection.Close();
        }

        #endregion
    }
}