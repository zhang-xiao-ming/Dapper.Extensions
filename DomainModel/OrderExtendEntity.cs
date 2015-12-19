using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel
{
    /// <summary>
    /// 订单扩展信息
    /// </summary>
    public class OrderExtendEntity
    {
        public string Id { get; set; }

        public string Title { get; set; } /*冗余字段*/

        public OrderStatusType OrderStatusType { get; set; }/*冗余字段*/
    }
}
