using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GantChart2
{
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public int Index { get; set; }

        public ComboBoxItem(int Index, string Text)
        {
            this.Index = Index;
            this.Text = Text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
