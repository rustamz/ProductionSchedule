using System;
using System.Windows.Forms;
using ScheduleCore;

namespace Production_schedule
{
    public partial class InsertOrder : Form
    {
        private Configuration conf;
        private int itemId = -1;

        public InsertOrder(Configuration Conf)
        {
            InitializeComponent();

            conf = Conf;
            textBox1.Text = conf.Orders.GetFreeId().ToString();
            
            LoadCustomers();
        }

        public InsertOrder(Configuration Conf, int ItemId)
        {
            InitializeComponent();

            conf = Conf;
            itemId = ItemId;

            LoadCustomers();

            int OrderIndex = conf.Orders.GetIndexById(itemId);

            textBox1.Text = conf.Orders[OrderIndex].Id.ToString();

            if (conf.Orders[OrderIndex].DeadLine != null)
            {
                checkBox1.Checked = true;
                dateTimePicker2.Value = conf.Orders[OrderIndex].DeadLine.Value;
            }

            foreach (ComboBoxItem item in comboBox1.Items)
            {
                if (item.Id == conf.Orders[OrderIndex].CustomerId)
                {
                    comboBox1.SelectedItem = item;
                    break;
                }
            }

            dateTimePicker1.Value = conf.Orders[OrderIndex].Date;
        }

        private void LoadCustomers()
        {
            comboBox1.Items.Clear();

            for (int i = 0; i < conf.Customers.Count; i++)
            {
                comboBox1.Items.Add(new ComboBoxItem(conf.Customers[i].Id, conf.Customers[i].Text));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string FailMsg = "";

            int OrderId = -1;

            try
            {
                OrderId = Convert.ToInt32(textBox1.Text);
            }
            catch(Exception)
            {
                FailMsg = "Индекс заказа должен быть целым неотрицательным числом!";
                goto fail_exit;
            }

            if (itemId == -1)
            {
                if (conf.Orders.GetIndexById(OrderId) != -1)
                {
                    FailMsg = "Заказ с таким индексом уже существует!";
                    goto fail_exit;
                }
            }

            if (comboBox1.SelectedItem == null)
            {
                FailMsg = "Вы должны выбрать заказчика!";
                goto fail_exit;
            }

            if (itemId == -1)
            {
                if (checkBox1.Checked) 
                    conf.Orders.Add(new OrderItem(OrderId, ((ComboBoxItem)comboBox1.SelectedItem).Id, dateTimePicker1.Value, dateTimePicker2.Value));
                else
                    conf.Orders.Add(new OrderItem(OrderId, ((ComboBoxItem)comboBox1.SelectedItem).Id, dateTimePicker1.Value, null));
            }
            else
            {
                int OrderIndex = conf.Orders.GetIndexById(itemId);

                conf.Orders[OrderIndex].Id = OrderId;
                conf.Orders[OrderIndex].CustomerId = ((ComboBoxItem)comboBox1.SelectedItem).Id;
                conf.Orders[OrderIndex].Date = dateTimePicker1.Value;
                if (checkBox1.Checked)
                    conf.Orders[OrderIndex].DeadLine = dateTimePicker2.Value;
                else
                    conf.Orders[OrderIndex].DeadLine = null;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
            return;

        fail_exit:
            MessageBox.Show(this, FailMsg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Enabled = checkBox1.Checked;
        }
    }
}
