using System;
using System.Drawing;
using System.Windows.Forms;

namespace Production_schedule
{
    public partial class ProgramConfiguration : Form
    {
        public ProgramConfiguration()
        {
            InitializeComponent();
        }

        public bool DrawItemText { get { return GraphDrawItemTextChBox.Checked; } set { GraphDrawItemTextChBox.Checked = value; } }
        

        public Color TaskColorFill             { get { return panel1.BackColor; } set { panel1.BackColor = value; } }
        public Color TaskColorPerimeter        { get { return panel2.BackColor; } set { panel2.BackColor = value; } }
        public Color TaskColorFillAlert        { get { return panel3.BackColor; } set { panel3.BackColor = value; } }
        public Color ConfColor1                { get { return panel4.BackColor; } set { panel4.BackColor = value; } }
        public Color ConfColor2                { get { return panel5.BackColor; } set { panel5.BackColor = value; } }
        public Color ServColor1                { get { return panel6.BackColor; } set { panel6.BackColor = value; } }
        public Color ServColor2                { get { return panel7.BackColor; } set { panel7.BackColor = value; } }
        public Color TaskColorFillOrder        { get { return panel14.BackColor; } set { panel14.BackColor = value; } }


        public Color TaskColorFillShadow       { get { return panel8.BackColor; } set { panel8.BackColor = value; } }
        public Color ConfColor1Shadow          { get { return panel9.BackColor; } set { panel9.BackColor = value; } }
        public Color ConfColor2Shadow          { get { return panel10.BackColor; } set { panel10.BackColor = value; } }
        public Color ServColor1Shadow          { get { return panel11.BackColor; } set { panel11.BackColor = value; } }
        public Color ServColor2Shadow          { get { return panel12.BackColor; } set { panel12.BackColor = value; } }
        public Color AllPerimeterColorShadow   { get { return panel13.BackColor; } set { panel13.BackColor = value; } }
        public Color TaskColorFillOrderShadow  { get { return panel15.BackColor; } set { panel15.BackColor = value; } }

        public bool GraphAutoSize { get { return checkBox1.Checked; } set { checkBox1.Checked = value; } }
        public int GraphWidth { get { return Int32.Parse(textBox1.Text); } set { textBox1.Text = value.ToString(); } }
        public int GraphHeight { get { return Int32.Parse(textBox2.Text); } set { textBox2.Text = value.ToString(); } }

        private void GraphColorChange(object sender, EventArgs e)
        {
            Panel currPanel = (Panel)sender;
            ColorDialog dlg = new ColorDialog();
            dlg.Color = currPanel.BackColor;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currPanel.BackColor = dlg.Color; 
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = textBox2.Enabled = !checkBox1.Checked;
        }

        private void Num_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case (char)0x08:
                    return;
            }

            e.KeyChar = (char)0;
            return;
        }



    }
}
