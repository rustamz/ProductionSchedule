using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public class CustomerItem : BaseScheduleItem
    {
        public string Phone
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public CustomerItem(int Id, string Text, string Phone, string Address)
            : base(Id,Text)
        {
            this.Phone = Phone;
            this.Address = Address;
        }

        public new object Clone()
        {
            return new CustomerItem(Id, Text, Phone, Address);
        }
    }
}
