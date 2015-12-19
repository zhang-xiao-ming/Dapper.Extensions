using System;
using System.Collections.Generic;
using System.Data;

namespace Dapper.Extensions
{
    public interface IDbContext : IDisposable
    {
        IDbConnection DbConnection { get; }
        ISqlGenerator SqlGenerator { get; }

        T Get<T>(object id, string tableName = null, int? commandTimeout = null) where T : class;

        bool Insert<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class;

        bool Insert<T>(IEnumerable<T> entities, string tableName = null, int? commandTimeout = null) where T : class;

        bool Update<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class;

        bool Save<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class;

        bool Delete<T>(T entity, string tableName = null, int? commandTimeout = null) where T : class;

        bool Delete<T>(object id, string tableName = null, int? commandTimeout = null) where T : class;

        IList<T> List<T>(string condition, IDictionary<string, object> parameters, int? commandTimeout = null) where T : class;
        IList<T> List<T>(string tableName, string condition, IDictionary<string, object> parameters, int? commandTimeout = null) where T : class;
        IList<T> List<T>(string tableName, string condition, object parameters, int? commandTimeout = null) where T : class;
        IList<T> List<T>(string condition, object parameters, int? commandTimeout = null) where T : class;

        IList<T> List<T>(string condition, IDictionary<string, object> parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class;
        IList<T> List<T>(string tableName, string condition, IDictionary<string, object> parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class;
        IList<T> List<T>(string tableName, string condition, object parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class;
        IList<T> List<T>(string condition, object parameters, int firstResult, int maxResults, int? commandTimeout = null) where T : class;
        int Count<T>(string condition, IDictionary<string, object> parameters, int? commandTimeout = null) where T : class;
        int Count<T>(string tableName, string condition, IDictionary<string, object> parameters, int? commandTimeout = null) where T : class;


        PagingResult<T> Paging<T>(string condition, IDictionary<string, object> parameters, int pageIndex, int pageSize, int? commandTimeout = null) where T : class;
        PagingResult<T> Paging<T>(string tableName, string condition, IDictionary<string, object> parameters, int pageIndex, int pageSize, int? commandTimeout = null) where T : class;

        void Commit();

        void Rollback();

        void Close();


        int Execute(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IDataReader ExecuteReader(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        object ExecuteScalar(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        DataTable GetDataTable(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<object> Query(Type type, string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        IList<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
        SqlMapper.GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?));
    }
}
