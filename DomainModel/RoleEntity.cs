using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel
{
    /// <summary>
    /// 角色信息
    /// </summary>
    [Serializable]
    public sealed class RoleEntity
    {
        public RoleEntity()
        {
            CreatedTime = DateTime.Now;
        }

        public int Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据更新版本号记录，用于乐观并发锁控制
        /// </summary>
        public int Version { get; set; }


        /// <summary>
        /// 创建时间 
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }

        /// <summary>
        /// 此记录是否已被持久化过
        /// </summary>
        public bool IsPersisted { get; set; }
    }
}
