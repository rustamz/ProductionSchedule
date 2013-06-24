using System;
using System.Windows.Forms;
using ScheduleCore;

namespace Production_schedule
{
    public partial class InsertTask : Form
    {
        private ScheduleManager sm;
        private int taskId = -1;
        private TaskList TempTasks;

        public InsertTask(ScheduleManager SchedMan)
        {
            InitializeComponent();
            sm = SchedMan;

            // копируем задания, чтоб можно было нажать отмена
            TempTasks = (TaskList)sm.Data.Tasks.Clone();

            LoadProduction();
            LoadMaterial();
            LoadOrders();

            Text = "Добавить задание";
        }

        public InsertTask(ScheduleManager SchedMan, int TaskId)
        {
            InitializeComponent();
            sm = SchedMan;

            // копируем задания, чтоб можно было нажать отмена
            TempTasks = (TaskList)sm.Data.Tasks.Clone();

            button4.Text = "Изменить";
            
            numericUpDown1.Value = 1;
            numericUpDown1.Enabled = false;

            LoadProduction();
            LoadMaterial();
            LoadOrders();

            int TaskIndex = sm.Data.Tasks.GetIndexById(TaskId);

            comboBox4.Text = sm.Data.Tasks[TaskIndex].Text;


            foreach (object item in comboBox1.Items)
            {
                if (((ComboBoxItem)item).Id == TempTasks[TaskIndex].ProductionId)
                {
                    comboBox1.SelectedItem = item;
                    break;
                }
            }

            int ProdIndex = sm.Data.Productions.GetIndexById(TempTasks[TaskIndex].ProductionId);
            foreach (object item in comboBox3.Items)
            {
                if (((ComboBoxItem)item).Id == TempTasks[TaskIndex].SizeIndex)
                {
                    comboBox3.SelectedItem = item;
                    break;
                }
            }

            foreach (object item in comboBox4.Items)
            {
                if (((ComboBoxItem)item).Id == TempTasks[TaskIndex].OrderId)
                {
                    comboBox4.SelectedItem = item;
                    break;
                }
            }

            foreach (object item in comboBox2.Items)
            {
                if (((ComboBoxItem)item).Id == TempTasks[TaskIndex].MaterialId)
                {
                    comboBox2.SelectedItem = item;
                    break;
                }
            }

            Text = "Изменить задание";
            this.taskId = TaskId;
        }

        public void LoadMaterial()
        {
            comboBox2.Items.Clear();
            for (int i = 0; i < sm.Data.Materials.Count; i++)
            {
                comboBox2.Items.Add(new ComboBoxItem(sm.Data.Materials[i].Id, sm.Data.Materials[i].Text));
            }
        }

        public void LoadProduction()
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < sm.Data.Productions.Count; i++)
            {
                comboBox1.Items.Add(new ComboBoxItem(sm.Data.Productions[i].Id, sm.Data.Productions[i].Text));
            }
        }

        public void LoadOrders()
        {
            comboBox4.Items.Clear();
            for (int i = 0; i < sm.Data.Orders.Count; i++)
            {
                int CustomerIndex = sm.Data.Customers.GetIndexById(sm.Data.Orders[i].CustomerId);
                string OrderText = "Заказ № "  + sm.Data.Orders[i].Id.ToString() + " (" + sm.Data.Customers[CustomerIndex].Text + ")";
                comboBox4.Items.Add(new ComboBoxItem(sm.Data.Orders[i].Id, OrderText));
            }
        }

        /// <summary>
        /// Проверка, можно ли добавить задание используя текцщие параметры
        /// </summary>
        /// <returns></returns>
        private bool AddTask()
        {
            string FailMsg = "";

            if (comboBox4.Text.Length == 0)
            {
                FailMsg = "Вы должны добавить описание!";
                goto fail_exit;
            }

            if (comboBox1.SelectedItem == null)
            {
                FailMsg = "Вы должны выбрать тип продукции!";
                goto fail_exit;
            }

            if (comboBox2.SelectedItem == null)
            {
                FailMsg = "Вы должны выбрать материал продукции!";
                goto fail_exit;
            }

            if (comboBox3.SelectedItem == null)
            {
                FailMsg = "Вы должны выбрать размер продукции!";
                goto fail_exit;
            }

            if (comboBox4.SelectedItem == null)
            {
                FailMsg = "Вы должны указать заказ!";
                goto fail_exit;
            }

            
            if (taskId == -1)
            {
                int TaskCount = (int)numericUpDown1.Value;

                int MaterialId = ((ComboBoxItem)comboBox2.SelectedItem).Id;
                int ProductionId = ((ComboBoxItem)comboBox1.SelectedItem).Id;
                int SizeIndex = ((ComboBoxItem)comboBox3.SelectedItem).Id;
                int OrderId = ((ComboBoxItem)comboBox4.SelectedItem).Id;

                while (TaskCount-- != 0)
                {
                    TempTasks.Add(new ScheduleCore.TaskItem(TempTasks.GetFreeId(), comboBox4.Text, MaterialId, ProductionId, SizeIndex, OrderId));
                }
            }
            else
            {
                int TaskIndex = sm.Data.Tasks.GetIndexById(taskId);
                TempTasks[TaskIndex].MaterialId = ((ComboBoxItem)comboBox2.SelectedItem).Id;
                TempTasks[TaskIndex].ProductionId = ((ComboBoxItem)comboBox1.SelectedItem).Id;
                TempTasks[TaskIndex].SizeIndex = ((ComboBoxItem)comboBox3.SelectedItem).Id;
                TempTasks[TaskIndex].OrderId = ((ComboBoxItem)comboBox4.SelectedItem).Id;
            }
            return true;

        fail_exit:
            MessageBox.Show(this, FailMsg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); 
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sm.Data.Tasks = TempTasks;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)comboBox1.SelectedItem;
                comboBox3.Items.Clear();

                int ProductionIndex = sm.Data.Productions.GetIndexById(cbi.Id);
                if (ProductionIndex == -1)
                    return;

                foreach (ProductionSize item in sm.Data.Productions[ProductionIndex].SupSizes)
                {
                    comboBox3.Items.Add(new ComboBoxItem(item.Index, item.ToString()));
                }

                comboBox3.Enabled = true;
            }
            else
            {
                comboBox3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ((new InsertOrder(sm.Data)).ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                LoadOrders();
            }
        }

        private void UpdateList()
        {
            if (comboBox4.SelectedItem == null)
                return;

            int OrderId = ((ComboBoxItem)comboBox4.SelectedItem).Id;

            // заполняем информацию о заказе
            {
                int OrderIndex = sm.Data.Orders.GetIndexById(OrderId);
                if (OrderIndex != -1)
                {
                    label10.Text = Configuration.DateToString(sm.Data.Orders[OrderIndex].Date);
                    int CustomerIndex = sm.Data.Customers.GetIndexById(sm.Data.Orders[OrderIndex].CustomerId);
                    if (CustomerIndex != -1)
                        label11.Text = sm.Data.Customers[CustomerIndex].Text;
                    label12.Text = sm.Data.Orders[OrderIndex].DeadLine != null ?
                        Configuration.DateToString(sm.Data.Orders[OrderIndex].DeadLine) : "не указан";
                }
            }

            // заполняем список текцущими заданиями
            listView1.BeginUpdate();
            listView1.Items.Clear();
            for (int i = 0, item_index = 1; i < TempTasks.Count; i++)
            {
                if (TempTasks[i].OrderId == OrderId)
                {
                    int TaskId = TempTasks[i].Id;
                    int ProductionId = TempTasks[i].ProductionId;
                    int MaterialId = TempTasks[i].MaterialId;

                    int ProductionIndex = sm.Data.Productions.GetIndexById(ProductionId);

                    string ProductionString = sm.Data.Productions[ProductionIndex].Text;

                    ListViewItem lvi = new ListViewItem(item_index.ToString()); item_index++;
                    lvi.Tag = TaskId;
                    lvi.SubItems.Add(ProductionString);
                    lvi.SubItems.Add(sm.Data.Materials.GetTextById(MaterialId));
                    listView1.Items.Add(lvi);
                }
            }

            listView1.EndUpdate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (AddTask())
            {
                UpdateList();
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedItem != null)
                UpdateList();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                    TempTasks.DeleteById((int)item.Tag);
                UpdateList();
            }
        }
    }
}
