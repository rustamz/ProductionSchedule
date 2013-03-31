using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public class CustomerList : BaseScheduleList
    {
        public CustomerList()
            : base()
        {
            
        }

        public CustomerItem this[int index]
        {
            get { return (CustomerItem)items[index]; }
            set { items[index] = value; }
        }

        public void Add(CustomerItem Item)
        {
            // просматриваем список: нет ли материала с таким же именем или идентификатором
            foreach (CustomerItem item in items)
            {
                if (item.Text == Item.Text)
                    throw new Exception("\"" + Item.Text + "\": Имя материала должно быть уникальным!");
                if (item.Id == Item.Id)
                    throw new Exception("\"" + Item.Text + "\": Идентификатор материала должнен быть уникальным!");
            }
            items.Add((CustomerItem)Item.Clone());
        }

        public new object Clone()
        {
            CustomerList NewList = new CustomerList();
            foreach (CustomerItem item in items)
            {
                NewList.Add((CustomerItem)item.Clone());
            }
            return NewList;
        }

    }
}
