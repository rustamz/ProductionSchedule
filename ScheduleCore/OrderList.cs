using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public class OrderList : BaseScheduleList
    {
        public OrderList()
            : base()
        {
            
        }

        public OrderItem this[int index]
        {
            get { return (OrderItem)items[index]; }
            set { items[index] = value; }
        }

        public void Add(OrderItem Item)
        {
            items.Add((OrderItem)Item.Clone());
        }

        public new object Clone()
        {
            OrderList NewList = new OrderList();
            foreach (OrderItem item in items)
            {
                NewList.Add((OrderItem)item.Clone());
            }
            return NewList;
        }
    }
}
