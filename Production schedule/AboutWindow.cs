using System;
using System.Windows.Forms;
using System.Reflection;

namespace Production_schedule
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
            label5.Text += " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            ListViewItem lvi = new ListViewItem("ScheduleCore");
            lvi.SubItems.Add(AssemblyName.GetAssemblyName(Assembly.Load("ScheduleCore").Location).Version.ToString());
            listView1.Items.Add(lvi);

            lvi = new ListViewItem("GantChart2");
            lvi.SubItems.Add(AssemblyName.GetAssemblyName(Assembly.Load("GantChart2").Location).Version.ToString());
            listView1.Items.Add(lvi);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
