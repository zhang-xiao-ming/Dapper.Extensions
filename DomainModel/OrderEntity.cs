using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel
{
    /// <summary>
    /// 订单信息
    /// </summary>
    public class OrderEntity
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public OrderStatusType OrderStatusType { get; set; }

        public string Buyer { get; set; }

        public DateTime? PayTime { get; set; }

        public DateTime? DeliverTime { get; set; }
    }
}
