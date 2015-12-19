using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel
{

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserEntity
    {
        public UserEntity()
        {
            CreatedTime = DateTime.Now;
        }

        public string Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 角色编号
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 创建时间 
        /// </summary>
        public virtual DateTime CreatedTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public virtual DateTime LastModifyTime { get; set; }

    }
}
