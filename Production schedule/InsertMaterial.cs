using System;
using System.Windows.Forms;
using ScheduleCore;

namespace Production_schedule
{
    public partial class InsertMaterial : Form
    {
        private Configuration conf;
        private int itemId = -1;

        public InsertMaterial(Configuration Conf)
        {
            InitializeComponent();
            conf = Conf;

            Text = "Добавить материал";
        }

        public InsertMaterial(Configuration Conf, int ItemId)
        {
            InitializeComponent();
            conf = Conf;
            itemId = ItemId;
            Text = "Изменить материал";

            int itemIndex = conf.Materials.GetIndexById(itemId);
            textBox1.Text = conf.Materials[itemIndex].Text;
            textBox2.Text = conf.Materials[itemIndex].SawingTime.ToString();
            textBox3.Text = conf.Materials[itemIndex].PolishingTime.ToString();
            textBox4.Text = conf.Materials[itemIndex].Description;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string FailMessage = "";

            if (textBox1.Text.Length == 0)
            {
                FailMessage = "Поле название не может быть пустым!";
                goto fail_exit;
            }

            string ItemName = textBox1.Text;

            if (textBox2.Text.Length == 0)
            {
                FailMessage = "Вы должны задать время распиливания!";
                goto fail_exit;
            }

            int SawingTime = -1;

            try
            {
                SawingTime = Convert.ToInt32(textBox2.Text);
            }
            catch (Exception ex)
            {
                FailMessage = label2 + " " + ex.Message;
                goto fail_exit; 
            }

            if (textBox3.Text.Length == 0)
            {
                FailMessage = "Вы должны задать время шлифования!";
                goto fail_exit;
            }

            int PolishingTime = -1;

            try
            {
                PolishingTime = Convert.ToInt32(textBox3.Text);
            }
            catch (Exception ex)
            {
                FailMessage = label3 + " " + ex.Message;
                goto fail_exit;
            }

            if (itemId == -1)
            {
                try
                {
                    conf.Materials.Add(new MaterialItem(ItemName, textBox4.Text, SawingTime, PolishingTime));
                }
                catch (Exception ex)
                {
                    FailMessage = ex.Message;
                    goto fail_exit;
                }
            }
            else
            {
                int itemIndex = conf.Materials.GetIndexById(itemId);
                conf.Materials[itemIndex].Text = ItemName;
                conf.Materials[itemIndex].SawingTime = SawingTime;
                conf.Materials[itemIndex].PolishingTime = PolishingTime;
                conf.Materials[itemIndex].Description = textBox4.Text;
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
