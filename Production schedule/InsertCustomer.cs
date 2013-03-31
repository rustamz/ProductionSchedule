using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScheduleCore;

namespace Production_schedule
{
    public partial class InsertCustomer : Form
    {

        private Configuration conf;
        private int itemId = -1;

        public InsertCustomer(Configuration Сonf)
        {
            InitializeComponent();

            conf = Сonf;

            this.Text = "Добавить нового клиента";
        }

        public InsertCustomer(Configuration Сonf, int ItemId)
        {
            InitializeComponent();

            conf = Сonf;
            itemId = ItemId;

            this.Text = "Изменить параметры клиента";

            int CustIndex = conf.Customers.GetIndexById(itemId);
            textBox1.Text = conf.Customers[CustIndex].Text;
            textBox2.Text = conf.Customers[CustIndex].Phone;
            textBox3.Text = conf.Customers[CustIndex].Address;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string FailMessage = "";

            if (textBox1.Text.Length == 0)
            {
                FailMessage = "Необходимо указать название организации!";
                goto fail_exit;
            }

            if (itemId == -1)
                if (conf.Customers.GetIndexByText(textBox1.Text) != -1)
                {
                    FailMessage = "Организация с таким именем уже существует!";
                    goto fail_exit;
                }

            if (textBox2.Text.Length == 0)
            {
                FailMessage = "Необходимо указать телефон организации!";
                goto fail_exit;
            }

            if (textBox3.Text.Length == 0)
            {
                FailMessage = "Необходимо указать адрес организации!";
                goto fail_exit;
            }

            if (itemId == -1)
            {
                conf.Customers.Add(new CustomerItem(conf.Customers.GetFreeId(),
                    textBox1.Text, textBox2.Text, textBox3.Text));
            }
            else
            {
                int CustIndex = conf.Customers.GetIndexById(itemId);
                conf.Customers[CustIndex].Text = textBox1.Text;
                conf.Customers[CustIndex].Phone = textBox2.Text;
                conf.Customers[CustIndex].Address = textBox3.Text;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
            return;

        fail_exit:
            MessageBox.Show(this, FailMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
    }
}
