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
    public partial class InsertProduction : Form
    {
        private Configuration conf;
        private int itemId = -1;

        public InsertProduction(Configuration Conf)
        {
            InitializeComponent();
            conf = Conf;
            Text = "Добавить продукцию";
        }

        public InsertProduction(Configuration Conf, int ItemId)
        {
            InitializeComponent();
            conf = Conf;
            itemId = ItemId;
            Text = "Изменить продукцию";

            int ProdIndex = conf.Productions.GetIndexById(ItemId);

            textBox1.Text = conf.Productions[ProdIndex].Text;
            ProductionSizeList psl = conf.Productions[ProdIndex].SupSizes;
            foreach (ProductionSize item in psl)
            {
                listBox1.Items.Add(item.Clone());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string FailMessage = "";
            double length = -1;
            double width = -1;
            double height = -1;

            try
            {
                length = Double.Parse(textBox2.Text);
            }
            catch (Exception ex)
            {
                FailMessage = "Длина заготовки: " + ex.Message;
                goto fail_exit; 
            }

            if (length <= 0)
            {
                FailMessage = "Длина заготовки должна задаваться положительным числом!";
                goto fail_exit;  
            }

            try
            {
                width = Double.Parse(textBox3.Text);
            }
            catch (Exception ex)
            {
                FailMessage = "Ширина заготовки: " + ex.Message;
                goto fail_exit;
            }

            if (width <= 0)
            {
                FailMessage = "Ширина заготовки должна задаваться положительным числом!";
                goto fail_exit;
            }

            try
            {
                height = Double.Parse(textBox4.Text);
            }
            catch (Exception ex)
            {
                FailMessage = "Высота заготовки: " + ex.Message;
                goto fail_exit;
            }

            if (height <= 0)
            {
                FailMessage = "Высота заготовки должна задаваться положительным числом!";
                goto fail_exit;
            }

            // проверка на сощуствование элемента с такими же параметрами
            foreach (object item in listBox1.Items)
            {
                ProductionSize psi = (ProductionSize)item;
                if (psi.Length == length)
                    if (psi.Width == width)
                        if (psi.Height == height)
                        {
                            FailMessage = "Подобный размер уже установлен в качестве поддерживаемого!";
                            goto fail_exit; 
                        }
            }


            listBox1.Items.Add(new ProductionSize(-1, length, width, height));

            return;

        fail_exit:
            MessageBox.Show(this, FailMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string FailMessage = "";

            string ProdName = textBox1.Text;

            if (ProdName.Length == 0)
            {
                FailMessage = "Вы должны задать имя продукции!";
                goto fail_exit;
            }

            if (itemId == -1)
            {
                if (conf.Productions.GetIdByText(ProdName) != -1)
                {
                    FailMessage = "Продукция с таким именем уже существует!";
                    goto fail_exit;
                }
            }

            ProductionSizeList SizeList = new ProductionSizeList();
            foreach (object item in listBox1.Items)
            {
                ProductionSize psi = (ProductionSize)item;

                // если у элемента уже был индекс, то сохраняем его, иначе
                // создаём новый
                SizeList.Add(new ProductionSize(psi.Index == -1 ? SizeList.GetFreeIndex() : psi.Index, psi.Length,
                    psi.Width, psi.Height));
            }
            if (itemId == -1)
            {
                conf.Productions.Add(new ProductionItem(conf.Productions.GetFreeId(), ProdName, SizeList));
            }
            else
            {
                int ProdIndex = conf.Productions.GetIndexById(itemId);
                conf.Productions[ProdIndex].Text = ProdName;
                conf.Productions[ProdIndex].SupSizes = SizeList;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
            return;

        fail_exit:
            MessageBox.Show(this, FailMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;   
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listBox1.SelectedItem != null)
                {
                    // если мы редактируем существующую продукцию,
                    // то мы должны позаботиться об удалении ссылок
                    if (itemId != -1)
                    {
                        ProductionSize psi = (ProductionSize)listBox1.SelectedItem;
                        if (psi.Index != -1)
                        {
                            // получаем задания, котороые содержат удаляемый материал
                            Stack<int> tasks_indexes = new Stack<int>();
                            for (int i = 0, i_end = conf.Tasks.Count; i < i_end; i++)
                            {
                                if (conf.Tasks[i].ProductionId == itemId)
                                {
                                    if (conf.Tasks[i].SizeIndex == psi.Index)
                                        tasks_indexes.Push(i);
                                }
                            }

                            if (tasks_indexes.Count != 0)
                            {
                                string Msg = "Продукция \"" + conf.Productions.GetTextById(itemId) +
                                "\" с этим размером используется в " + tasks_indexes.Count.ToString() +
                                (tasks_indexes.Count > 1 ? " заданиях. Эти задания будут удалены." :
                                " задании. Это задание будет удалено.") + "\r\nПродолжить?";

                                if (MessageBox.Show(this, Msg, this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) ==
                                    System.Windows.Forms.DialogResult.Yes)
                                {
                                    while (tasks_indexes.Count != 0)
                                    {
                                        conf.Tasks.Delete(tasks_indexes.Pop());
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }

                    listBox1.Items.Remove(listBox1.SelectedItem);
                }
            }
        }
    }
}
