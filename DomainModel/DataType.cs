using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
// ReSharper disable All

namespace DomainModel
{
    public class DataType
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 测试int型数据
        /// </summary>

        public int TInt { get; set; }

        /// <summary>
        /// 测试long型数据
        /// </summary>

        public long TLong { get; set; }

        /// <summary>
        /// 测试decimal型数据
        /// </summary>

        public decimal TDecimal { get; set; }

        /// <summary>
        /// 测试float型数据
        /// </summary>

        public float TFloat { get; set; }

        /// <summary>
        /// 测试double型数据
        /// </summary>

        public double TDouble { get; set; }

        /// <summary>
        /// 测试bool型数据
        /// </summary>
        public bool TBool { get; set; }

        /// <summary>
        /// 测试DateTime型数据
        /// </summary>

        public DateTime TDateTime { get; set; }

        public string TString { get; set; }

        /// <summary>
        /// 测试byte[]型数据
        /// </summary>

        public byte[] TBypes { get; set; }

        /// <summary>
        /// 测试enum类型数据
        /// </summary>

        public DbType TEnum { get; set; }

        /// <summary>
        /// 测试Single类型数据
        /// </summary>

        public Single TSingle { get; set; }

    }

}
