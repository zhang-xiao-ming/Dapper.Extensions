using System;
using System.Data;

namespace Dapper.Extensions
{
    public static class Extensions
    {

        #region IDataReader
        public static T GetValue<T>(this IDataReader dr, int i)
        {
            var value = dr.GetValue(i);
            if (value == DBNull.Value)
                return default(T);
            return (T)value;
        }

        public static T GetValue<T>(this IDataReader dr, string name)
        {
            return dr.GetValue<T>(dr.GetOrdinal(name));
        }

        public static bool IsDbNull(this IDataReader dr, string name)
        {
            return dr.IsDBNull(dr.GetOrdinal(name));
        }

        #endregion

        #region DataRow

        public static T GetValue<T>(this DataRow dr, string columnName)
        {
            var value = dr[columnName];
            if (value == DBNull.Value)
                return default(T);
            return (T)value;
        }

        public static T GetValue<T>(this DataRow dr, int columnIndex)
        {
            var value = dr[columnIndex];
            if (value == DBNull.Value)
                return default(T);
            return (T)value;
        }
        #endregion


    }
}
