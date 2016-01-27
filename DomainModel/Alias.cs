using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace DomainModel
{
    /// <summary>
    /// 表名与类名，字段名与属性名不一致的情况
    /// </summary>
    public class Alias
    {
        /// <summary>
        /// AliasId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// sName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// iAge
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// dCreatedTime
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }

}
