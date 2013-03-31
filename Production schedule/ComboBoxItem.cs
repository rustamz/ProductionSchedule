using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Production_schedule
{
    class ComboBoxItem
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public ComboBoxItem(int Id, string Text)
        {
            this.Id = Id;
            this.Text = Text;
        }
        public override string ToString()
        {
            return Text;
        }
    }
}
