using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DomainModel
{
    public class NullableDataType
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 测试 int? 型数据
        /// </summary>
        public int? TInt { get; set; }

        /// <summary>
        /// 测试 long? 型数据
        /// </summary>
        public long? TLong { get; set; }

        /// <summary>
        /// 测试 decimal? 型数据
        /// </summary>
        public decimal? TDecimal { get; set; }

        /// <summary>
        /// 测试 float? 型数据
        /// </summary>
        public float? TFloat { get; set; }

        /// <summary>
        /// 测试 double? 型数据
        /// </summary>
        public double? TDouble { get; set; }

        /// <summary>
        /// 测试 bool? 型数据
        /// </summary>
        public bool? TBool { get; set; }

        /// <summary>
        /// 测试 DateTime? 型数据
        /// </summary>
        public DateTime? TDateTime { get; set; }

        /// <summary>
        /// 测试 string 型数据
        /// </summary>
        public string TString { get; set; }

        /// <summary>
        /// 测试 byte[] 型数据
        /// </summary>
        public byte[] TBypes { get; set; }

        /// <summary>
        /// 测试 DbType? 型数据
        /// </summary>
        public DbType? TEnum { get; set; }

        /// <summary>
        /// 测试 Single? 型数据
        /// </summary>
        public Single? TSingle { get; set; }
    }

}
