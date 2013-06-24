using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ScheduleCore;

namespace Production_schedule
{
    public partial class ConfigWindow : Form
    {
        Configuration original_conf;
        Configuration conf;

        public ConfigWindow(Configuration Conf)
        {
            InitializeComponent();
            original_conf = Conf;
            conf = (Configuration)original_conf.Clone();

            LoadMaterials();
            LoadSaws();
            LoadGrinders();
            LoadProtucts();
            LoadCustomers();
            LoadOrders();
            dateTimePicker1.Value = conf.BaseTime;
        }


        # region Обновить списки
        
        /// <summary>
        /// Загружает доступные материалы.
        /// </summary>
        private void LoadMaterials()
        {
            listView1.BeginUpdate();

            listView1.Items.Clear();
            for (int i = 0; i < conf.Materials.Count; i++)
            {
                ListViewItem lvi = new ListViewItem((i + 1).ToString());
                lvi.Tag = conf.Materials[i].Id;
                lvi.SubItems.Add(conf.Materials[i].Text);
                listView1.Items.Add(lvi);
            }

            listView1.EndUpdate();

            listView1_SelectedIndexChanged(listView1, new EventArgs());
        }

        /// <summary>
        /// Загружает доступные пилы.
        /// </summary>
        private void LoadSaws()
        {
            listView2.BeginUpdate();

            listView2.Items.Clear();
            for (int i = 0; i < conf.Saws.Count; i++)
            {
                ListViewItem lvi = new ListViewItem((i + 1).ToString());
                lvi.Tag = conf.Saws[i].Id;
                lvi.SubItems.Add(conf.Saws[i].Text);
                listView2.Items.Add(lvi);
            }

            listView2.EndUpdate();

            listView2_SelectedIndexChanged(listView2, new EventArgs());
        }

        /// <summary>
        /// Загружает доступные шлифовальные станки.
        /// </summary>
        private void LoadGrinders()
        {
            listView3.BeginUpdate();

            listView3.Items.Clear();
            for (int i = 0; i < conf.Grinders.Count; i++)
            {
                ListViewItem lvi = new ListViewItem((i + 1).ToString());
                lvi.Tag = conf.Grinders[i].Id;
                lvi.SubItems.Add(conf.Grinders[i].Text);
                listView3.Items.Add(lvi);
            }

            listView3.EndUpdate();

            listView3_SelectedIndexChanged(listView3, new EventArgs());
        }

        /// <summary>
        /// Загружает доступную продукцию.
        /// </summary>
        private void LoadProtucts()
        {
            listView4.BeginUpdate();

            listView4.Items.Clear();
            for (int i = 0; i < conf.Productions.Count; i++)
            {
                ListViewItem lvi = new ListViewItem((i + 1).ToString());
                lvi.Tag = conf.Productions[i].Id;
                lvi.SubItems.Add(conf.Productions[i].Text);

                listView4.Items.Add(lvi);
            }

            listView4.EndUpdate();

            listView4_SelectedIndexChanged(listView4, new EventArgs());
        }

        /// <summary>
        /// Загружает доступных заказчиков.
        /// </summary>
        private void LoadCustomers()
        {
            listView5.BeginUpdate();
            
            listView5.Items.Clear();
            for (int i = 0; i < conf.Customers.Count; i++)
            {
                ListViewItem lvi = new ListViewItem((i + 1).ToString());
                lvi.Tag = conf.Customers[i].Id;
                lvi.SubItems.Add(conf.Customers[i].Text);
                lvi.SubItems.Add(conf.Customers[i].Phone);
                lvi.SubItems.Add(conf.Customers[i].Address);

                listView5.Items.Add(lvi);
            }

            listView5.EndUpdate();
        }

        /// <summary>
        /// Загружает доступные заказы.
        /// </summary>
        private void LoadOrders()
        {
            listView6.BeginUpdate();

            listView6.Items.Clear();
            for (int i = 0; i < conf.Orders.Count; i++)
            {
                ListViewItem lvi = new ListViewItem((i + 1).ToString());
                lvi.Tag = conf.Orders[i].Id;
                lvi.SubItems.Add(conf.Orders[i].Text);
                lvi.SubItems.Add(conf.Customers.GetTextById(conf.Orders[i].CustomerId));
                lvi.SubItems.Add( conf.Orders[i].Date.ToShortDateString());

                listView6.Items.Add(lvi);
            }

            listView6.EndUpdate();
        }

        #endregion

        #region Добавление Элементов

        /// <summary>
        /// Добавляет материал.
        /// </summary>
        public void AddMaterial()
        {
            if ((new InsertMaterial(conf)).ShowDialog(this) == 
                System.Windows.Forms.DialogResult.OK)
            {
                LoadMaterials();
            } 
        }

        /// <summary>
        /// Добавляет пилу.
        /// </summary>
        public void AddSaw()
        {
            if ((new InsertDevice(conf, BaseDeviceType.Saw)).ShowDialog(this) == 
                System.Windows.Forms.DialogResult.OK)
            {
                LoadSaws();
            }
        }

        /// <summary>
        /// Добавляет шлифовальный станок.
        /// </summary>
        public void AddGrinder()
        {
            if ((new InsertDevice(conf, BaseDeviceType.Grinder)).ShowDialog(this) == 
                System.Windows.Forms.DialogResult.OK)
            {
                LoadGrinders();
            }
        }

        /// <summary>
        /// Добавляет продукцию.
        /// </summary>
        public void AddProduction()
        {
            if ((new InsertProduction(conf)).ShowDialog(this) == 
                System.Windows.Forms.DialogResult.OK)
            {
                LoadProtucts();
            } 
        }

        /// <summary>
        /// Добавляет заказчика.
        /// </summary>
        public void AddCustomer()
        {
            if ((new InsertCustomer(conf)).ShowDialog(this) == 
                System.Windows.Forms.DialogResult.OK)
            {
                LoadCustomers();
            }
        }

        /// <summary>
        /// Добавляет заказ.
        /// </summary>
        public void AddOrders()
        {
            if ((new InsertOrder(conf)).ShowDialog(this) ==
                System.Windows.Forms.DialogResult.OK)
            {
                LoadOrders();
            }
        }

        #endregion

        #region Изменение элементов

        /// <summary>
        /// Изменяет выделенный в данный момент материал.
        /// </summary>
        public void EditMaterial()
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            bool UpdateAfterEdit = false;
            foreach (ListViewItem item in items)
            {
                InsertMaterial dlg = new InsertMaterial(conf, (int)item.Tag);
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateAfterEdit = true;
                }
            }

            if (UpdateAfterEdit) LoadMaterials();
        }

        /// <summary>
        /// Изменяет выделенные в данный момент пилы.
        /// </summary>
        public void EditSaw()
        {
            ListView.SelectedListViewItemCollection items = listView2.SelectedItems;
            bool UpdateAfterEdit = false;
            foreach (ListViewItem item in items)
            {
                InsertDevice dlg = new InsertDevice(conf, (int)item.Tag, BaseDeviceType.Saw);
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateAfterEdit = true;
                }
            }

            if (UpdateAfterEdit) LoadSaws();
        }

        /// <summary>
        /// Изменяет выделенные в данный момент шлифовальные станки.
        /// </summary>
        public void EditGrinder()
        {
            ListView.SelectedListViewItemCollection items = listView3.SelectedItems;
            bool UpdateAfterEdit = false;
            foreach (ListViewItem item in items)
            {
                InsertDevice dlg = new InsertDevice(conf, (int)item.Tag, BaseDeviceType.Grinder);
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateAfterEdit = true;
                }
            }

            if (UpdateAfterEdit) LoadGrinders();
        }

        /// <summary>
        /// Изменяет выделенную в данный момент продукцию.
        /// </summary>
        public void EditProduction()
        {
            ListView.SelectedListViewItemCollection items = listView4.SelectedItems;
            bool UpdateAfterEdit = false;
            foreach (ListViewItem item in items)
            {
                InsertProduction dlg = new InsertProduction(conf, (int)item.Tag);
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateAfterEdit = true;
                }
            }

            if (UpdateAfterEdit) LoadProtucts();
        }

        /// <summary>
        /// Изменяет выделенных заказчиков.
        /// </summary>
        public void EditCustomer()
        {
            ListView.SelectedListViewItemCollection items = listView5.SelectedItems;
            bool UpdateAfterEdit = false;
            foreach (ListViewItem item in items)
            {
                InsertCustomer dlg = new InsertCustomer(conf, (int)item.Tag);
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateAfterEdit = true;
                }
            }

            if (UpdateAfterEdit) LoadCustomers();
        }

        /// <summary>
        /// Изменяет выделенные в данный момент заказы.
        /// </summary>
        public void EditOrders()
        {
            ListView.SelectedListViewItemCollection items = listView6.SelectedItems;
            bool UpdateAfterEdit = false;
            foreach (ListViewItem item in items)
            {
                if ((new InsertOrder(conf, (int)item.Tag)).ShowDialog(this) == 
                    System.Windows.Forms.DialogResult.OK)
                {
                    UpdateAfterEdit = true;
                }
            }

            if (UpdateAfterEdit) LoadOrders();
        }

        #endregion

        #region Удаление элементов

        /// <summary>
        /// Удаляет выделенный в данный момент материал.
        /// </summary>
        public void DeleteMaterial()
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    int MaterialId = (int)item.Tag;

                    // получаем задания, котороые содержат удаляемый материал
                    Stack<int> tasks_indexes = new Stack<int>();
                    Stack<int> saws_indexes = new Stack<int>();
                    Stack<int> grinders_indexes = new Stack<int>();

                    for (int i = 0, i_end = conf.Tasks.Count; i < i_end; i++)
                    {
                        if (conf.Tasks[i].MaterialId == MaterialId)
                        {
                            tasks_indexes.Push(i);
                        }
                    }

                    for (int i = 0; i < conf.Saws.Count; i++)
                    {
                        if (conf.Saws[i].IsSupportMaterial(MaterialId))
                        {
                            saws_indexes.Push(i);
                        }
                    }

                    for (int i = 0; i < conf.Grinders.Count; i++)
                    {
                        if (conf.Saws[i].IsSupportMaterial(MaterialId))
                        {
                            grinders_indexes.Push(i);
                        }
                    }

                    if (tasks_indexes.Count != 0 || saws_indexes.Count != 0 || grinders_indexes.Count != 0)
                    {
                        string Msg = "Материал используется в существующих заданиях и/или в качестве настроек для станков.\r\nПродолжить удаление?";

                        if (MessageBox.Show(this, Msg, this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            // удаляем задания по материалу
                            while (tasks_indexes.Count != 0)
                            {
                                conf.Tasks.Delete(tasks_indexes.Pop());
                            }

                            // удаляем из станоков
                            while (saws_indexes.Count != 0)
                            {
                                int DeviceIndex = saws_indexes.Pop();
                                conf.Saws[DeviceIndex].RemoveMaterial(MaterialId);
                            }

                            while (grinders_indexes.Count != 0)
                            {
                                int DeviceIndex = grinders_indexes.Pop();
                                conf.Grinders[DeviceIndex].RemoveMaterial(MaterialId);
                            }

                            LoadSaws();
                            LoadGrinders();

                            conf.Materials.DeleteById(MaterialId);
                        }
                    }
                    else
                    {
                        conf.Materials.DeleteById(MaterialId);
                    }
                }
                LoadMaterials();
            }
        }

        /// <summary>
        /// Удаляет выделенные в данный момент пилы.
        /// </summary>
        public void DeleteSaw()
        {
            ListView.SelectedListViewItemCollection items = listView2.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    int SawId = (int)item.Tag;
                    conf.Saws.DeleteById(SawId);
                }
                LoadSaws();
            }
        }

        /// <summary>
        /// Удаляет выделенные в данный момент шлифовальные станки.
        /// </summary>
        public void DeleteGrinder()
        {
            ListView.SelectedListViewItemCollection items = listView3.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    int GrinderId = (int)item.Tag;
                    conf.Saws.DeleteById(GrinderId);
                }
                LoadGrinders();
            }
        }

        /// <summary>
        /// Удаляет выделенную в данный момент продукцию.
        /// </summary>
        public void DeleteProduction()
        {
            ListView.SelectedListViewItemCollection items = listView4.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    int ProdId = (int)item.Tag;

                    // получаем задания, котороые содержат удаляемый материал
                    Stack<int> tasks_indexes = new Stack<int>();
                    for (int i = 0, i_end = conf.Tasks.Count; i < i_end; i++)
                    {
                        if (conf.Tasks[i].ProductionId == ProdId)
                        {
                            tasks_indexes.Push(i);
                        }
                    }

                    if (tasks_indexes.Count != 0)
                    {
                        string Msg = "Продукция \"" + conf.Productions.GetTextById(ProdId) +
                        "\" используется в " + tasks_indexes.Count.ToString() +
                        (tasks_indexes.Count > 1 ? " заданиях. Эти задания будут удалены." :
                        " задании. Это задание будет удалено.") + "\r\nПродолжить?";


                        if (MessageBox.Show(this, Msg, this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            while (tasks_indexes.Count != 0)
                            {
                                conf.Tasks.Delete(tasks_indexes.Pop());
                            }
                            conf.Productions.DeleteById(ProdId);
                        }
                    }
                    else
                    {
                        conf.Productions.DeleteById(ProdId);
                    }
                }
                LoadProtucts();
            }
        }

        /// <summary>
        /// Удаляет выделенных заказчиков.
        /// </summary>
        public void DeleteCustomer()
        {
            ListView.SelectedListViewItemCollection items = listView5.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    int CustId = (int)item.Tag;

                    // получаем заказы, котороые содержат удаляемого заказчика
                    Stack<int> orders_indexes = new Stack<int>();
                    Stack<int> orders_id = new Stack<int>();
                    for (int i = 0, i_end = conf.Orders.Count; i < i_end; i++)
                    {
                        if (conf.Orders[i].CustomerId == CustId)
                        {
                            orders_indexes.Push(i);
                            orders_id.Push(conf.Orders[i].Id);
                        }
                    }

                    if (orders_indexes.Count != 0)
                    {
                        string Msg = "Заказчик \"" + conf.Customers.GetTextById(CustId) +
                        "\" имеет " + orders_indexes.Count.ToString() +
                        (orders_indexes.Count > 1 ? " заказов. Эти заказы будут удалены." :
                        " заказ. Это заказ будет удален.") + "\r\nПродолжить?";


                        if (MessageBox.Show(this, Msg, this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            // удаляем заказы
                            while (orders_indexes.Count != 0)
                            {
                                conf.Orders.Delete(orders_indexes.Pop());
                            }

                            // удаляем задания
                            while (orders_id.Count != 0)
                            {
                                int OrderId = orders_id.Pop();
                                Stack<int> tasks_indexes = new Stack<int>();
                                for (int i = 0, i_end = conf.Tasks.Count; i < i_end; i++)
                                {
                                    if (conf.Tasks[i].OrderId == OrderId)
                                    {
                                        tasks_indexes.Push(i);
                                    }
                                }

                                while (tasks_indexes.Count != 0)
                                {
                                    conf.Tasks.Delete(tasks_indexes.Pop());
                                }
                            }

                            conf.Customers.DeleteById(CustId);
                        }
                    }
                    else
                    {
                        conf.Customers.DeleteById(CustId);
                    }
                }
                LoadCustomers();
            }
        }

        /// <summary>
        /// Удаляет выделенные в данный момент заказы.
        /// </summary>
        public void DeleteOrders()
        {
            ListView.SelectedListViewItemCollection items = listView6.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    int OrderId = (int)item.Tag;

                    // получаем задания, которые состоят в заказе
                    Stack<int> tasks_indexes = new Stack<int>();
                    for (int i = 0, i_end = conf.Tasks.Count; i < i_end; i++)
                    {
                        if (conf.Tasks[i].OrderId == OrderId)
                        {
                            tasks_indexes.Push(i);
                        }
                    }

                    if (tasks_indexes.Count != 0)
                    {
                        string Msg = "Заказ \"" + conf.Orders.GetTextById(OrderId) +
                        "\" содержит " + tasks_indexes.Count.ToString() +
                        (tasks_indexes.Count > 1 ? " заданий. Эти задания будут удалены." :
                        " задание. Это задание будет удалено.") + "\r\nПродолжить?";

                        if (MessageBox.Show(this, Msg, this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            while (tasks_indexes.Count != 0)
                            {
                                conf.Tasks.Delete(tasks_indexes.Pop());
                            }
                            conf.Orders.DeleteById(OrderId);
                        }
                    }
                    else
                    {
                        conf.Orders.DeleteById(OrderId);
                    }
                }
                LoadOrders();
            }
        }

        #endregion

        #region События по вкладкам
        
        #region Вкладка "Материал"

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView1.SelectedItems[0];
                int ID = (int)lvi.Tag;
                textBox1.Clear();
                int Index = conf.Materials.GetIndexById(ID);

                if (Index < 0) return;
                string Result = "Время распиливания m2: " +
                    conf.Materials[Index].SawingTime + " мин.\r\nВремя шлифования m2: " +
                    conf.Materials[Index].PolishingTime + " мин.\r\nОписание:\r\n" + conf.Materials[Index].Description;

                textBox1.Text = Result;
            }
            else
            {
                textBox1.Text = "Выделите элемент для получения сведений";
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            EditMaterial();
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    {
                        EditMaterial();
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteMaterial();
                        break;
                    }
            }
        }

        #endregion

        #region Вкладка "Пилы"

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView2.SelectedItems[0];
                int ID = (int)lvi.Tag;
                textBox2.Clear();
                int Index = conf.Saws.GetIndexById(ID);

                if (Index < 0) return;

                string Result = "Ответственный:\r\n\t" + conf.Saws[Index].Responsible + "\r\nСписок поддерживаемых конфигураций:";
                for (int i = 0; i < conf.Saws[Index].SupportedMaterials.Count; i++)
                {
                    Result += "\r\n\t- " + (i + 1).ToString() + ". " +
                        conf.Materials.GetTextById(conf.Saws[Index].SupportedMaterials[i].ID) +
                        " ( настройка: " + MinuteToHour(conf.Saws[Index].SupportedMaterials[i].Time) + ")";

                }

                Result += "\r\nНачальная конфигурация:\r\n\t" + (conf.Saws[Index].DefaultMaterialId != -1 ? 
                    conf.Materials.GetTextById(conf.Saws[Index].DefaultMaterialId) : "не настроено");


                textBox2.Text = Result;
            }
            else
            {
                textBox2.Text = "Выделите элемент для получения сведений";
            }
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditSaw();
        }

        private void listView2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    {
                        EditSaw();
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteSaw();
                        break;
                    }
            }
        }

        #endregion

        #region Вкладка "Шлифовальные станки"

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView3.SelectedItems[0];
                int ID = (int)lvi.Tag;
                textBox3.Clear();
                int Index = conf.Grinders.GetIndexById(ID);

                if (Index < 0) return;

                string Result = "Ответственный:\r\n\t" + conf.Grinders[Index].Responsible + "\r\nСписок поддерживаемых конфигураций:";
                for (int i = 0; i < conf.Grinders[Index].SupportedMaterials.Count; i++)
                {
                    Result += "\r\n\t- " + (i + 1).ToString() + ". " +
                        conf.Materials.GetTextById(conf.Grinders[Index].SupportedMaterials[i].ID) +
                        " ( настройка: " + MinuteToHour(conf.Grinders[Index].SupportedMaterials[i].Time) + ")";

                }

                Result += "\r\nНачальная конфигурация:\r\n\t" + (conf.Grinders[Index].DefaultMaterialId != -1 ?
                    conf.Materials.GetTextById(conf.Grinders[Index].DefaultMaterialId) : "не настроено");

                textBox3.Text = Result;
            }
            else
            {
                textBox3.Text = "Выделите элемент для получения сведений";
            }
        }

        private void listView3_DoubleClick(object sender, EventArgs e)
        {
            EditGrinder();
        }

        private void listView3_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    {
                        EditGrinder();
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteGrinder();
                        break;
                    }
            }
        }
       
        #endregion

        #region Вкладка "Продукция"
        
        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView4.SelectedItems[0];
                int ID = (int)lvi.Tag;
                textBox4.Clear();
                int Index = conf.Productions.GetIndexById(ID);

                if (Index < 0) return;
                string Result = "Поддерживаемые размеры:\r\n";
                ProductionSizeList psl = conf.Productions[Index].SupSizes;
                foreach (ProductionSize item in psl)
                {
                    Result += "\t" + item.ToString() + "\r\n";
                }
                textBox4.Text = Result;
            }
            else
            {
                textBox4.Text = "Выделите элемент для получения сведений";
            }
        }

        private void listView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditProduction();
        }

        private void listView4_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    {
                        EditProduction();
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteProduction();
                        break;
                    }
            }
        }

        #endregion

        #region Вкладка "Заказчики"

        private void listView5_DoubleClick(object sender, EventArgs e)
        {
            EditCustomer();
        }

        private void listView5_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    {
                        EditCustomer();
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteCustomer();
                        break;
                    }
            }
        }

        #endregion

        #region Вкладка "Заказы"

        private void listView6_DoubleClick(object sender, EventArgs e)
        {
            EditOrders();
        }

        private void listView6_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    {
                        EditOrders();
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteOrders();
                        break;
                    }
            }
        }

        #endregion

        #endregion

        #region Контекстные меню

        #region Контекстное меню "Материалы"

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (listView1.SelectedItems.Count == 0)
            {
                изменитьToolStripMenuItem.Enabled = false;
                удалитьToolStripMenuItem.Enabled = false;
            }
            else
            {
                изменитьToolStripMenuItem.Enabled = true;
                удалитьToolStripMenuItem.Enabled = true; 
            }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMaterial();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditMaterial();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMaterial();
        }

        #endregion

        #region Контекстное меню "Пилы"

        private void contextMenuStrip3_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView2.SelectedItems.Count == 0)
            {
                удалитьToolStripMenuItem2.Enabled = false;
                изменитьToolStripMenuItem2.Enabled = false;
            }
            else
            {
                удалитьToolStripMenuItem2.Enabled = true;
                изменитьToolStripMenuItem2.Enabled = true;
            }
        }

        private void добавитьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AddSaw();
        }

        private void изменитьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            EditSaw();
        }

        private void удалитьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DeleteSaw();
        }

        #endregion

        #region Контекстное меню "Шлифовальные станки"

        private void contextMenuStrip4_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView3.SelectedItems.Count == 0)
            {
                удалитьToolStripMenuItem3.Enabled = false;
                изменитьToolStripMenuItem3.Enabled = false;
            }
            else
            {
                удалитьToolStripMenuItem3.Enabled = true;
                изменитьToolStripMenuItem3.Enabled = true;
            }
        }

        private void добавитьToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            AddGrinder();
        }

        private void изменитьToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            EditGrinder();
        }

        private void удалитьToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            DeleteGrinder();
        }

        #endregion

        #region Контекстное меню "Продукция"

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView4.SelectedItems.Count == 0)
            {
                удалитьToolStripMenuItem1.Enabled = false;
                изменитьToolStripMenuItem1.Enabled = false;
            }
            else
            {
                удалитьToolStripMenuItem1.Enabled = true;
                изменитьToolStripMenuItem1.Enabled = true;
            }
        }
        
        private void добавитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddProduction();
        }

        private void изменитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditProduction();
        }

        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteProduction();
        }

        #endregion

        #region Контекстное меню "Заказчики"

        private void contextMenuCustomer_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView5.SelectedItems.Count == 0)
            {
                удалитьToolStripMenuItem4.Enabled = false;
                изменитьToolStripMenuItem4.Enabled = false;
            }
            else
            {
                удалитьToolStripMenuItem4.Enabled = true;
                изменитьToolStripMenuItem4.Enabled = true;
            }
        }

        private void добавитьToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AddCustomer();
        }

        private void изменитьToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            EditCustomer();
        }

        private void удалитьToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            DeleteCustomer();
        }

        #endregion

        #region Контекстное меню "Заказы"
        
        private void contextMenuOrder_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView6.SelectedItems.Count == 0)
            {
                удалитьToolStripMenuItem5.Enabled = false;
                изменитьToolStripMenuItem5.Enabled = false;
            }
            else
            {
                удалитьToolStripMenuItem5.Enabled = true;
                изменитьToolStripMenuItem5.Enabled = true;
            }
        }

        private void добавитьToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            AddOrders();
        }

        private void изменитьToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            EditOrders();
        }

        private void удалитьToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            DeleteOrders();
        }
        #endregion

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conf.BaseTime = dateTimePicker1.Value;
            original_conf.Assign(conf);
            Close();
        }

        private string MinuteToHour(double MinuteCount)
        {
            string Result = "";
            int HourCount = (int)(MinuteCount / 60);

            if (HourCount > 0)
            {
                Result += HourCount.ToString() + " ч.";

                int dM = (int)(MinuteCount - HourCount * 60);

                if (dM > 0)
                    Result += ", " + dM.ToString() + " мин.";

                return Result;
            }

            return MinuteCount.ToString() + " мин.";
        }
    }
}
