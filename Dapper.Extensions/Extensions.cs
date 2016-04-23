using System;
using System.Data;
using System.Xml.Linq;

namespace Dapper.Extensions
{
    public static class Extensions
    {
        private static readonly DateTime DateTime19000101 = new DateTime(1900, 1, 1);
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

        #region DynamicParameters

        public static void Add(this DynamicParameters dynamicParameters, string name, char? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.AnsiString);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, char value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.AnsiString);
        }

        public static void Add(this DynamicParameters dynamicParameters,string name,bool? value)
        {
            if(dynamicParameters==null)
                return;
            dynamicParameters.Add(name, value, DbType.Boolean);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, bool value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Boolean);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, byte? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int16);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, byte value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int16);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, short? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int16);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, short value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int16);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, int? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int32);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, int value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int32);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, long? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int64);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, long value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Int64);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, decimal? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Decimal);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, decimal value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Decimal);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, float? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Single);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, float value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Single);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, double? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Double);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, double value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Double);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, DateTime? value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value==null ||value.Value < DateTime19000101 ? DateTime19000101 : value.Value, DbType.DateTime);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, DateTime value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value < DateTime19000101 ? DateTime19000101 : value, DbType.DateTime);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, string value, int size, bool isUnicodeString = false, bool isLengthFixed = false)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, isUnicodeString ? (isLengthFixed ? DbType.StringFixedLength : DbType.String) : (isLengthFixed ? DbType.AnsiStringFixedLength : DbType.AnsiString), size: size);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, byte[] value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Binary);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, Guid value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Guid);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, XDocument value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Xml);
        }

        public static void Add(this DynamicParameters dynamicParameters, string name, DataTable value)
        {
            if (dynamicParameters == null)
                return;
            dynamicParameters.Add(name, value, DbType.Object);
        }
        #endregion
    }
}
