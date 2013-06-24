using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Production_schedule
{
    public partial class MainWindow : Form
    {

        #region Костыли для поддержки сворачиваемых групп
        
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct LVGROUP
        {
            public int cbSize;
            public int mask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszHeader;
            public int cchHeader;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszFooter;
            public int cchFooter;
            public int iGroupId;
            public int stateMask;
            public int state;
            public int uAlign;
        }

        private enum GroupStates
        {
            Normal = 0x00,
            Collapsible = 0x08,
            Collapsed = 0x01
        }

        private void MakeGroupCollapsible(int State)
        {
            for (int i = 0; i <= listView1.Groups.Count ; i++ )
            {
                LVGROUP group = new LVGROUP();
                group.cbSize = Marshal.SizeOf(group);
                group.state = State;
                group.mask = 4;
                group.iGroupId = i;
                IntPtr ip = IntPtr.Zero;
                try
                {
                    ip = Marshal.AllocHGlobal(Marshal.SizeOf(group));
                    Marshal.StructureToPtr(group, ip, false);
                    SendMessage(listView1.Handle, 0x1000 + 147, (IntPtr)i, ip);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (ip != null) 
                        Marshal.FreeHGlobal(ip);
                }

            }
        }

        #endregion


        private ScheduleCore.ScheduleManager sm = new ScheduleCore.ScheduleManager();

        private string ProjectName = "";
        private readonly string ProgramConfigPath = "guiconf";
        private string CommonGraphInfo = "Суммарная длительность:\r\nСуммарный штраф:";

        public MainWindow()
        {
            InitializeComponent();

            gantChart1.SelectItem += new GantChart2.GantChart.ItemChanged(gantChart1_ItemSelect);
            gantChart1.SelectLostItem += new GantChart2.GantChart.ItemChanged(gantChart1_ItemSelectLost);
         
            if (System.IO.File.Exists(ProgramConfigPath))
            {
                ProgramParam param;
                ProgramParamSerializer.Deserialize(out param, ProgramConfigPath);

                gantChart1.DrawItemText = param.DrawItemText;

                gantChart1.TaskColorFill = param.TaskColorFill;
                gantChart1.TaskColorPerimeter = param.TaskColorPerimeter;
                gantChart1.TaskColorFillAlert = param.TaskColorFillAlert;
                gantChart1.ConfColor1 = param.ConfColor1;
                gantChart1.ConfColor2 = param.ConfColor2;
                gantChart1.ServColor1 = param.ServColor1;
                gantChart1.ServColor2 = param.ServColor2;
                gantChart1.TaskColorFillOrder = param.TaskColorFillOrder;

                gantChart1.TaskColorFillShadow = param.TaskColorFillShadow;
                gantChart1.ConfColor1Shadow = param.ConfColor1Shadow;
                gantChart1.ConfColor2Shadow = param.ConfColor2Shadow;
                gantChart1.ServColor1Shadow = param.ServColor1Shadow;
                gantChart1.ServColor2Shadow = param.ServColor2Shadow;
                gantChart1.AllPerimeterColorShadow = param.AllPerimeterColorShadow;
                gantChart1.TaskColorFillOrderShadow = param.TaskColorFillOrderShadow;
                this.Left = param.LastWindowPosX;
                this.Top = param.LastWindowPosY;
                this.Width = param.LastWindowWidth;
                this.Height = param.LastWindowHeight;
                this.WindowState = param.FormSate;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            string[] Args = Environment.GetCommandLineArgs();
            string OpenFile;
            if (Args.Length > 1)
            {
                ProjectName = Args[1];
                Text = Text + " - " + Path.GetFileName(ProjectName);
                OpenFile = ProjectName;
            }
            else
            {
                OpenFile = "Config.xml";
            }

            try
            {
                sm.Data.LoadFromFile(OpenFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }


            LoadTasks();
        }


        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InsertTask dlg = new InsertTask(sm);
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                LoadTasks();
            }
        }

        private ListViewItem GetItemByCharacteristics(ListView.ListViewItemCollection Items, int TaskProductionId, int TaskMaterialId, int TaskSizeIndex)
        {
            foreach (ListViewItem Item in Items)
            {
                ScheduleCore.TaskItem OrdItem = (ScheduleCore.TaskItem)sm.Data.Tasks.GetItemById((int)Item.Tag);
                if (OrdItem != null)
                {
                    if (OrdItem.ProductionId == TaskProductionId
                        && OrdItem.MaterialId == TaskMaterialId 
                        && OrdItem.SizeIndex == TaskSizeIndex)
                    {
                        return Item;
                    }
                }
            }
            return null;
        }

        private void LoadTasks()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            ScheduleCore.TaskList temp_task = (ScheduleCore.TaskList)sm.Data.Tasks.Clone();

            for (int j = 0; j < sm.Data.Orders.Count; j++)
            {
                // получаем задания, которые относятся к заказу
                ScheduleCore.TaskList OrderedTask = new ScheduleCore.TaskList();
                Stack<int> deleteIndexes = new Stack<int>();
                
                // перебираем задания относящиеся к заказу
                for (int i = 0; i < temp_task.Count; i++)
                {
                    if (temp_task[i].OrderId == sm.Data.Orders[j].Id)
                    {
                        OrderedTask.Add(temp_task[i]);
                        deleteIndexes.Push(i);
                    }
                }

                // удаляем добавленные задания, чтоб ускорить поиск остальных
                // заданий под заказы
                while (deleteIndexes.Count != 0)
                    temp_task.Delete(deleteIndexes.Pop());

                if (OrderedTask.Count != 0)
                {
                    ListViewGroup lvg = new ListViewGroup();
                    string GroupHeader = "";
                    GroupHeader = sm.Data.Orders[j].Text + " (" +
                        sm.Data.Customers.GetTextById(sm.Data.Orders[j].CustomerId) +
                        ", " + OrderedTask.Count.ToString() +
                        (OrderedTask.Count == 2 || OrderedTask.Count == 3 || OrderedTask.Count == 4 ?
                        " задания" : " заданий") + 
                       (sm.Data.Orders[j].DeadLine != null ? 
                        ", Срок исполнения: " + ScheduleCore.Configuration.DateToString(sm.Data.Orders[j].DeadLine) 
                        : "") + ")";
                    lvg.Header = GroupHeader;
                    

                    int groupIndex = listView1.Groups.Add(lvg);

                    for (int i = 0; i < OrderedTask.Count; i++)
                    {
                        // получаем информацию из задания
                        int TaskProductionId = OrderedTask[i].ProductionId;
                        int TaskMaterialId = OrderedTask[i].MaterialId;
                        int TaskSizeIndex = OrderedTask[i].SizeIndex;

                        // получаем задание из группы с такими же характеристиками

                        ListViewItem CloneTask = null;

                        if (GroupCloneTasks.Checked)
                            CloneTask = GetItemByCharacteristics(listView1.Groups[groupIndex].Items, TaskProductionId, TaskMaterialId, TaskSizeIndex);
                        
                        // если такого задание нет, то создаём новое
                        if (CloneTask == null)
                        {
                            int SizePos = -1;
                            int ProdIndex = sm.Data.Productions.GetIndexById(OrderedTask[i].ProductionId);
                            for (int k = 0; k < sm.Data.Productions[ProdIndex].SupSizes.Count; k++)
                            {
                                if (sm.Data.Productions[ProdIndex].SupSizes[k].Index == OrderedTask[i].SizeIndex)
                                {
                                    SizePos = k;
                                    break;
                                }
                            }
                            ListViewItem lvi = new ListViewItem((i + 1).ToString());
                            lvi.Tag = OrderedTask[i].Id;
                            lvi.SubItems.Add(OrderedTask[i].Text);
                            lvi.SubItems.Add(sm.Data.Productions.GetTextById(OrderedTask[i].ProductionId));
                            lvi.SubItems.Add(sm.Data.Materials.GetTextById(OrderedTask[i].MaterialId));
                            
                            if (SizePos != -1)
                            {
                                lvi.SubItems.Add(sm.Data.Productions[ProdIndex].SupSizes[SizePos].Length.ToString() + "×" +
                                                 sm.Data.Productions[ProdIndex].SupSizes[SizePos].Width.ToString() + "×" +
                                                 sm.Data.Productions[ProdIndex].SupSizes[SizePos].Height.ToString());
                            }
                            else
                            {
                                lvi.SubItems.Add("нет данных");
                            }

                            lvi.SubItems.Add("1");

                            listView1.Items.Add(lvi).Group = listView1.Groups[groupIndex];
                        }
                        else // если есть такое задание, то увеличиваем у него количество
                        {
                            int CountInTask = Convert.ToInt32(CloneTask.SubItems[CloneTask.SubItems.Count - 1].Text);
                            CloneTask.SubItems[CloneTask.SubItems.Count - 1].Text = (CountInTask + 1).ToString();

                            string TaskText = CloneTask.SubItems[1].Text;
                            string Divider = " ... ";
                            if (CountInTask == 1)
                            {
                                CloneTask.SubItems[1].Text = CloneTask.SubItems[1].Text + Divider + OrderedTask[i].Text;
                            }
                            else
                            {
                                int PointsPos = TaskText.IndexOf(Divider);
                                if (PointsPos > -1)
                                {
                                    TaskText = TaskText.Remove(PointsPos, TaskText.Length - PointsPos);
                                    CloneTask.SubItems[1].Text = TaskText + Divider + OrderedTask[i].Text;
                                }
                            }
                        }
                    }
                }
            }

            MakeGroupCollapsible((int)(GroupStates.Collapsible) | (int)(GroupStates.Collapsed));

            listView1.EndUpdate();
            

            toolStripStatusLabel1.Text = "Всего заданий: " + sm.Data.Tasks.Count;
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = "psp";
            dlg.Filter = "Проект (*.psp)|*.psp";
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    sm.Data.WriteToFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                ProjectName = dlg.FileName;
                Text = Text + " - " + Path.GetFileName(ProjectName);
            }
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutWindow()).ShowDialog(this);
        }

        private void просмотрСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming soon...");
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "psp";
            dlg.Filter = "Проект (*.psp)|*.psp";
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    sm.Data.LoadFromFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ProjectName = dlg.FileName;
                Text = Text + " - " + Path.GetFileName(ProjectName);
                LoadTasks();
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectName.Length != 0)
            {
                try
                {
                    sm.Data.WriteToFile(ProjectName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
            {
                сохранитьКакToolStripMenuItem_Click(null,null);
            }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                    sm.Data.Tasks.DeleteById((int)item.Tag);
                LoadTasks();
            }
        }

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

        private void gantChart1_ItemSelect(Object sender, GantChart2.GantChangedEvent arg)
        {
            int TaskIndex = arg.SelectedItem.Index != -1 ? sm.Data.Tasks.GetIndexById(arg.SelectedItem.Index) : -1;

            if (TaskIndex >= 0)
            {
                int OrderIndex = sm.Data.Orders.GetIndexById(sm.Data.Tasks[TaskIndex].OrderId);
                string TaskInfo = "";
                TaskInfo += "Задание: " + sm.Data.Tasks[TaskIndex].Text +
                (OrderIndex > -1 ? "\r\nЗаказ: " + sm.Data.Orders[OrderIndex].Text : "") +
                (sm.Data.Orders[OrderIndex].DeadLine != null ? 
                    "\r\nКрайний срок: " + ScheduleCore.Configuration.DateToString(sm.Data.Orders[OrderIndex].DeadLine) : "") + 
                "\r\nПродукция: " + sm.Data.Productions.GetTextById(sm.Data.Tasks[TaskIndex].ProductionId) +
                "\r\nМатериал: " + sm.Data.Materials.GetTextById(sm.Data.Tasks[TaskIndex].MaterialId);

                int SawIndex = sm.Data.Saws.IndexByTaskId(arg.SelectedItem.Index);
                if (SawIndex > -1)
                {
                    int SawedTaskIndex = sm.Data.Saws[SawIndex].IndexByTaskId(arg.SelectedItem.Index);
                    TaskInfo += "\r\nРаспилен на: " + sm.Data.Saws[SawIndex].Text +
                    " ( " + ScheduleCore.Configuration.DateToString(sm.Data.BaseTime.AddMinutes(sm.Data.Saws[SawIndex].CompleteTask[SawedTaskIndex].Begin)) + " - " + 
                    ScheduleCore.Configuration.DateToString(sm.Data.BaseTime.AddMinutes(sm.Data.Saws[SawIndex].CompleteTask[SawedTaskIndex].End)) + " )";
                }

                int GrinderIndex = sm.Data.Grinders.IndexByTaskId(arg.SelectedItem.Index);
                if (GrinderIndex > -1)
                {
                    int PolishedTaskIndex = sm.Data.Grinders[GrinderIndex].IndexByTaskId(arg.SelectedItem.Index);
                    TaskInfo += "\r\nОтшлифован на: " + sm.Data.Grinders[GrinderIndex].Text +
                    " ( " + ScheduleCore.Configuration.DateToString(sm.Data.BaseTime.AddMinutes(sm.Data.Grinders[GrinderIndex].CompleteTask[PolishedTaskIndex].Begin)) + " - " +
                    ScheduleCore.Configuration.DateToString(sm.Data.BaseTime.AddMinutes(sm.Data.Grinders[GrinderIndex].CompleteTask[PolishedTaskIndex].End)) + " )";
                }
                   

                textBox2.Text = TaskInfo;
            }
            else if (arg.SelectedItem.Index == -1)
            {
                textBox2.Text = "Настройка" +
                "\r\nВремя старта: " + ScheduleCore.Configuration.DateToString(arg.SelectedItem.RealBeginDate) +
                "\r\nВремя завершения: " + ScheduleCore.Configuration.DateToString(arg.SelectedItem.RealEndDate);
            }
            else 
            {
                textBox2.Text = "Техническое обслуживание" +
                "\r\nВремя старта: " + ScheduleCore.Configuration.DateToString(arg.SelectedItem.RealBeginDate) +
                "\r\nВремя завершения: " + ScheduleCore.Configuration.DateToString(arg.SelectedItem.RealEndDate); 
            }
            
        }

        private void gantChart1_ItemSelectLost(Object sender, GantChart2.GantChangedEvent arg)
        {
            textBox2.Text = "";
        }

        private void WriteSchedule()
        {
            if (gantChart1.Items != null)
                gantChart1.Items.Clear();
            else 
                gantChart1.Items = new GantChart2.Rows();

            // загружаем задания
            gantChart1.ComboTaskItems.Clear();
            for (int i = 0; i < sm.Data.Tasks.Count; i++)
                gantChart1.ComboTaskItems.Add(new GantChart2.ComboBoxItem(sm.Data.Tasks[i].Id, sm.Data.Tasks[i].Text));

            // загружаем заказы
            gantChart1.ComboOrderItems.Clear();
            for (int i = 0; i < sm.Data.Orders.Count; i++)
                gantChart1.ComboOrderItems.Add(new GantChart2.ComboBoxItem(sm.Data.Orders[i].Id, sm.Data.Orders[i].Text));

            for (int i = 0; i < sm.Data.Saws.Count; i++)
            {
                GantChart2.Row row_item = new GantChart2.Row(sm.Data.Saws[i].Text);
                for (int j = 0; j < sm.Data.Saws[i].CompleteTask.Count; j++)
                {
                    int TaskID = sm.Data.Saws[i].CompleteTask[j].TaskId;
                    int TaskIndex = TaskID != -1 ? sm.Data.Tasks.GetIndexById(TaskID) : -1;

                    DateTime ItemEnd = sm.Data.BaseTime.AddMinutes(sm.Data.Saws[i].CompleteTask[j].End);
                    GantChart2.RowItem item = new GantChart2.RowItem(
                        (TaskID > -1 ? sm.Data.Tasks[TaskIndex].Text : (TaskID == -1 ? "Настройка" : "Сервис")),
                        TaskID,
                        TaskIndex >= 0 ? sm.Data.Tasks[TaskIndex].OrderId : -1,
                        sm.Data.BaseTime.AddMinutes(sm.Data.Saws[i].CompleteTask[j].Begin),
                        sm.Data.BaseTime.AddMinutes(sm.Data.Saws[i].CompleteTask[j].End),
                        TaskIndex != -1 ? (sm.Data.Tasks[TaskIndex].UseDeadLine ?
                        (ItemEnd > sm.Data.Tasks[TaskIndex].DeadLine) : false) : false);
                    row_item.Add(item);

                }
                gantChart1.Items.Add(row_item);
            }

            for (int i = 0; i < sm.Data.Grinders.Count; i++)
            {
                GantChart2.Row row_item = new GantChart2.Row(sm.Data.Grinders[i].Text);
                for (int j = 0; j < sm.Data.Grinders[i].CompleteTask.Count; j++)
                {
                    int TaskID = sm.Data.Grinders[i].CompleteTask[j].TaskId;
                    int TaskIndex = sm.Data.Tasks.GetIndexById(TaskID);

                    DateTime ItemEnd = sm.Data.BaseTime.AddMinutes(sm.Data.Grinders[i].CompleteTask[j].End);
                    GantChart2.RowItem item = new GantChart2.RowItem(
                        (TaskID > -1 ? sm.Data.Tasks[TaskIndex].Text : (TaskID == -1 ? "Настройка" : "Сервис")),
                        TaskID,
                        TaskIndex >= 0 ? sm.Data.Tasks[TaskIndex].OrderId : -1,
                        sm.Data.BaseTime.AddMinutes(sm.Data.Grinders[i].CompleteTask[j].Begin),
                        sm.Data.BaseTime.AddMinutes(sm.Data.Grinders[i].CompleteTask[j].End),
                        TaskIndex != -1 ? (sm.Data.Tasks[TaskIndex].UseDeadLine ?
                        (ItemEnd > sm.Data.Tasks[TaskIndex].DeadLine) 
                        : false) : false);
                    row_item.Add(item);
                }
                gantChart1.Items.Add(row_item);
            }
            gantChart1.BuildAndDrawGraphic();

            ViewCommonGraphInfo();

            gantChart1_ItemSelectLost(null, null);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            BuildClassicSchedule();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            BuildDirectiveSchedule();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (items.Count > 0)
            {
                InsertTask dlg = new InsertTask(sm, (int)items[0].Tag);
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    LoadTasks();
                }
            }
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            bool UpdateAfterEdit = false;
            foreach (ListViewItem item in items)
            {
                InsertTask dlg = new InsertTask(sm, (int)item.Tag);
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateAfterEdit = true;
                }
            }

            if (UpdateAfterEdit) LoadTasks();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramConfiguration dlg = new ProgramConfiguration();
            dlg.DrawItemText = gantChart1.DrawItemText;

            dlg.TaskColorFill = gantChart1.TaskColorFill;
            dlg.TaskColorFillOrder = gantChart1.TaskColorFillOrder;
            dlg.TaskColorPerimeter = gantChart1.TaskColorPerimeter;
            dlg.TaskColorFillAlert = gantChart1.TaskColorFillAlert;
            dlg.ConfColor1 = gantChart1.ConfColor1;
            dlg.ConfColor2 = gantChart1.ConfColor2;
            dlg.ServColor1 = gantChart1.ServColor1;
            dlg.ServColor2 = gantChart1.ServColor2;

            dlg.TaskColorFillShadow = gantChart1.TaskColorFillShadow;
            dlg.TaskColorFillOrderShadow = gantChart1.TaskColorFillOrderShadow;
            dlg.ConfColor1Shadow = gantChart1.ConfColor1Shadow;
            dlg.ConfColor2Shadow = gantChart1.ConfColor2Shadow;
            dlg.ServColor1Shadow = gantChart1.ServColor1Shadow;
            dlg.ServColor2Shadow = gantChart1.ServColor2Shadow;
            dlg.AllPerimeterColorShadow = gantChart1.AllPerimeterColorShadow;

            dlg.GraphAutoSize = gantChart1.AutoSizeDrawField;
            dlg.GraphWidth = gantChart1.DrawFieldWidth;
            dlg.GraphHeight = gantChart1.DrawFieldHeight;

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // Применяем настройки
                gantChart1.DrawItemText = dlg.DrawItemText;

                gantChart1.TaskColorFill = dlg.TaskColorFill;
                gantChart1.TaskColorFillOrder = dlg.TaskColorFillOrder;
                gantChart1.TaskColorPerimeter = dlg.TaskColorPerimeter;
                gantChart1.TaskColorFillAlert = dlg.TaskColorFillAlert;
                gantChart1.ConfColor1 = dlg.ConfColor1;
                gantChart1.ConfColor2 = dlg.ConfColor2;
                gantChart1.ServColor1 = dlg.ServColor1;
                gantChart1.ServColor2 = dlg.ServColor2;

                gantChart1.TaskColorFillShadow = dlg.TaskColorFillShadow;
                gantChart1.TaskColorFillOrderShadow = dlg.TaskColorFillOrderShadow;
                gantChart1.ConfColor1Shadow = dlg.ConfColor1Shadow;
                gantChart1.ConfColor2Shadow = dlg.ConfColor2Shadow;
                gantChart1.ServColor1Shadow = dlg.ServColor1Shadow;
                gantChart1.ServColor2Shadow = dlg.ServColor2Shadow;
                gantChart1.AllPerimeterColorShadow = dlg.AllPerimeterColorShadow;

                gantChart1.AutoSizeDrawField = dlg.GraphAutoSize;
                gantChart1.DrawFieldWidth = dlg.GraphWidth;
                gantChart1.DrawFieldHeight = dlg.GraphHeight;

                gantChart1.BuildAndDrawGraphic();
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProgramParam param = new ProgramParam();

            // восстановление состояния графика
            param.DrawItemText = gantChart1.DrawItemText;

            param.TaskColorFill = gantChart1.TaskColorFill;
            param.TaskColorFillOrder = gantChart1.TaskColorFillOrder;
            param.TaskColorPerimeter = gantChart1.TaskColorPerimeter;
            param.TaskColorFillAlert = gantChart1.TaskColorFillAlert;
            param.ConfColor1 = gantChart1.ConfColor1;
            param.ConfColor2 = gantChart1.ConfColor2;
            param.ServColor1 = gantChart1.ServColor1;
            param.ServColor2 = gantChart1.ServColor2;

            param.TaskColorFillShadow = gantChart1.TaskColorFillShadow;
            param.TaskColorFillOrderShadow = gantChart1.TaskColorFillOrderShadow;
            param.ConfColor1Shadow = gantChart1.ConfColor1Shadow;
            param.ConfColor2Shadow = gantChart1.ConfColor2Shadow;
            param.ServColor1Shadow = gantChart1.ServColor1Shadow;
            param.ServColor2Shadow = gantChart1.ServColor2Shadow;
            param.AllPerimeterColorShadow = gantChart1.AllPerimeterColorShadow;

            // восстановление состояния окна
            if (this.WindowState == FormWindowState.Normal)
            {
                param.LastWindowWidth = this.Width;
                param.LastWindowHeight = this.Height;
                param.LastWindowPosX = this.Left;
                param.LastWindowPosY = this.Top;
            }
            else
            {
                param.LastWindowWidth = -1;
                param.LastWindowHeight = -1;
                param.LastWindowPosX = -1;
                param.LastWindowPosY = -1;
            }
            param.FormSate = this.WindowState;
            ProgramParamSerializer.Serialize(param, ProgramConfigPath);
        }

        private void экпортДанныхВExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Файлы Excel |*.xlsx",
                OverwritePrompt = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Microsoft.Office.Interop.Excel._Application app =
                        new Microsoft.Office.Interop.Excel.Application();

                    app.SheetsInNewWorkbook = 1;
                    Microsoft.Office.Interop.Excel.Workbook book =
                        app.Workbooks.Add(Type.Missing);

                    Microsoft.Office.Interop.Excel.Worksheet sheet = book.ActiveSheet;
                    //sheet.Cells[1, 1] = "Hello!";

                    int DeviceIndex = 2;
                    for (int i = 0; i < sm.Data.Saws.Count; i++)
                    {
                        sheet.Cells[2, DeviceIndex] = sm.Data.Saws[i].Text;
                        sm.Data.Saws[i].Tag = DeviceIndex;
                        sheet.Cells[2, DeviceIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightSkyBlue);
                        sheet.Cells[2, DeviceIndex].Font.Bold = true;
                        sheet.Cells[2, DeviceIndex].Font.Size = 11;
                        sheet.Cells[2, DeviceIndex].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                        DeviceIndex++;
                    }
                    for (int i = 0; i < sm.Data.Grinders.Count; i++)
                    {
                        sheet.Cells[2, DeviceIndex] = sm.Data.Grinders[i].Text;
                        sm.Data.Grinders[i].Tag = DeviceIndex;
                        sheet.Cells[2, DeviceIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightSkyBlue);
                        sheet.Cells[2, DeviceIndex].Font.Bold = true;
                        sheet.Cells[2, DeviceIndex].Font.Size = 11;
                        sheet.Cells[2, DeviceIndex].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                        DeviceIndex++;
                    }

                    string Cell1 = sheet.Cells[1, 2].Address;
                    string Cell2 = sheet.Cells[1, (DeviceIndex - 1)].Address;
                    dynamic merg = sheet.get_Range(Cell1, Cell2);
                    merg.Merge(Type.Missing);
                    sheet.Cells[1, 2] = "Станки";
                    merg.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightSkyBlue);
                    sheet.Cells[1, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                    sheet.Cells[1, 2].Font.Bold = true;
                    sheet.Cells[1, 2].Font.Size = 14;
                    // пишем задания
                    {
                        int RowIndex = 3;
                        ScheduleCore.TaskList temp_task = (ScheduleCore.TaskList)sm.Data.Tasks.Clone();

                        for (int j = 0; j < sm.Data.Orders.Count; j++)
                        {
                            // получаем задания, которые относятся к заказу
                            ScheduleCore.TaskList OrderedTask = new ScheduleCore.TaskList();
                            Stack<int> deleteIndexes = new Stack<int>();

                            // перебираем задания относящиеся к заказу
                            for (int i = 0; i < temp_task.Count; i++)
                            {
                                if (temp_task[i].OrderId == sm.Data.Orders[j].Id)
                                {
                                    OrderedTask.Add(temp_task[i]);
                                    deleteIndexes.Push(i);
                                }
                            }

                            // удаляем добавленные задания, чтоб ускорить поиск остальных
                            // заданий под заказы
                            while (deleteIndexes.Count != 0)
                                temp_task.Delete(deleteIndexes.Pop());

                            if (OrderedTask.Count != 0)
                            {
                                string GroupHeader = "";
                                GroupHeader = sm.Data.Orders[j].Text + " (" +
                                    sm.Data.Customers.GetTextById(sm.Data.Orders[j].CustomerId) +
                                    ", " + OrderedTask.Count.ToString() +
                                    (OrderedTask.Count == 2 || OrderedTask.Count == 3 || OrderedTask.Count == 4 ?
                                    " задания" : " заданий") +
                                   (sm.Data.Orders[j].DeadLine != null ?
                                    ", Срок исполнения: " + ScheduleCore.Configuration.DateToString(sm.Data.Orders[j].DeadLine)
                                    : "") + ")";

                                sheet.Cells[RowIndex, 1] = GroupHeader;

                                Cell1 = sheet.Cells[RowIndex, 1].Address;
                                Cell2 = sheet.Cells[RowIndex, (DeviceIndex - 1)].Address;
                                merg = sheet.get_Range(Cell1, Cell2);
                                merg.Merge(Type.Missing);
                                merg.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);

                                sheet.Cells[RowIndex, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;

                                RowIndex++;
                                for (int i = 0; i < OrderedTask.Count; i++)
                                {
                                    sheet.Cells[RowIndex, 1] = OrderedTask[i].Text;
                                    sheet.Cells[RowIndex, 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);

                                    int TaskId = OrderedTask[i].Id;
                                    ScheduleCore.TimeDevice DeviceSaw = sm.Data.Saws[sm.Data.Saws.IndexByTaskId(TaskId)];

                                    if (DeviceSaw != null)
                                    {
                                        ScheduleCore.CompleteItem ComplItem = DeviceSaw.CompleteTask.GetItemByTaskId(TaskId);
                                        if (ComplItem != null)
                                        {
                                            int ColumnIndex = (int)DeviceSaw.Tag;
                                            sheet.Cells[RowIndex, ColumnIndex] = sm.Data.BaseTime.AddMinutes(ComplItem.Begin).ToString() + " - " + sm.Data.BaseTime.AddMinutes(ComplItem.End).ToString();

                                            if (sm.Data.Orders[j].DeadLine < sm.Data.BaseTime.AddMinutes(ComplItem.End))
                                            {
                                                sheet.Cells[RowIndex, ColumnIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightPink);
                                            }
                                        }
                                    }

                                    ScheduleCore.TimeDevice DeviceGrinder = sm.Data.Grinders[sm.Data.Grinders.IndexByTaskId(TaskId)];
                                    if (DeviceGrinder != null)
                                    {
                                        ScheduleCore.CompleteItem ComplItem = DeviceGrinder.CompleteTask.GetItemByTaskId(TaskId);
                                        if (ComplItem != null)
                                        {
                                            int ColumnIndex = (int)DeviceGrinder.Tag;
                                            sheet.Cells[RowIndex, ColumnIndex] = sm.Data.BaseTime.AddMinutes(ComplItem.Begin).ToString() + " - " + sm.Data.BaseTime.AddMinutes(ComplItem.End).ToString();

                                            if (sm.Data.Orders[j].DeadLine < sm.Data.BaseTime.AddMinutes(ComplItem.End))
                                            {
                                                sheet.Cells[RowIndex, ColumnIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightPink);
                                            }
                                        }
                                    }

                                    RowIndex++;
                                }
                            }
                        }
                    }

                    for (int i = 2; i < DeviceIndex; i++)
                    {
                        sheet.Columns[i].ColumnWidth = 40;
                    }

                    book.SaveAs(dialog.FileName);
                    app.Quit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // если сохранение произошло нормально, то предлагаем открыть документ
                if (MessageBox.Show(this, "Экспорт успешно завершен. Открыть документ?",
                    this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(dialog.FileName);
                    Process p = new Process();
                    p.StartInfo = psi;
                    p.Start();
                }
            }

           
            
        }

        private void статусбарToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = ((ToolStripMenuItem)sender).Checked;
        }

        private void BuildClassicSchedule()
        {
            try
            {
                sm.StartSchedule();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            WriteSchedule();
        }

        private void BuildDirectiveSchedule()
        {
            try
            {
                sm.StartSchedule2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            WriteSchedule();
        }

        private void построитьОбычноеРасписаниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildClassicSchedule();
        }

        private void построитьРасписаниеСИспользованиемДирективныхСроковToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildDirectiveSchedule();
        }

        private void управлениеПроектомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((new ConfigWindow(sm.Data)).ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                LoadTasks();
            }
        }

        private void свернутьВсеЗаданияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            MakeGroupCollapsible((int)(GroupStates.Collapsible) | (int)(GroupStates.Collapsed));
            listView1.EndUpdate();
        }

        private void развенутьВсеЗаданияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            MakeGroupCollapsible((int)(GroupStates.Collapsible) | (int)(GroupStates.Normal));
            listView1.EndUpdate();
        }

        private void ViewCommonGraphInfo()
        {
            double FullTime = 0; // полное время отработки на всех станках, включая настройку и сервис
            double FullGoodTime = 0; // время обработки, включает только полезное время
            double FullConfTime = 0; // полное время затраченное на конфигурацию оборудования
            double FullServiceTime = 0; // полное время затраченное на сервис

            int TasksComplete = 0;
            int TaskWithFineTime = 0;

            for (int i = 0; i < sm.Data.Orders.Count; i++)
            {
                sm.Data.Orders[i].FineTime = 0;
                sm.Data.Orders[i].StartTime = -1;
                sm.Data.Orders[i].EndTime = -1;
            }


            for (int i = 0; i < sm.Data.Saws.Count; i++)
            {
                for (int j = 0; j < sm.Data.Saws[i].CompleteTask.Count; j++)
                {
                    // длительность текущего задания
                    double TimeForTask = (sm.Data.Saws[i].CompleteTask[j].End - sm.Data.Saws[i].CompleteTask[j].Begin);

                    FullTime += TimeForTask;

                    if (sm.Data.Saws[i].CompleteTask[j].TaskId >= 0)
                    {
                        TasksComplete++;
                        FullGoodTime += TimeForTask;

                        int TaskIndex = sm.Data.Tasks.GetIndexById(sm.Data.Saws[i].CompleteTask[j].TaskId);
                        int OrderIndex = sm.Data.Orders.GetIndexById(sm.Data.Tasks[TaskIndex].OrderId);

                        {
                            if (sm.Data.Orders[OrderIndex].StartTime != -1)
                            {
                                if (sm.Data.Saws[i].CompleteTask[j].Begin < sm.Data.Orders[OrderIndex].StartTime)
                                {
                                    sm.Data.Orders[OrderIndex].StartTime = sm.Data.Saws[i].CompleteTask[j].Begin;
                                }
                            }
                            else
                            {
                                sm.Data.Orders[OrderIndex].StartTime = sm.Data.Saws[i].CompleteTask[j].Begin;
                            }

                            if (sm.Data.Orders[OrderIndex].EndTime != -1)
                            {
                                if (sm.Data.Saws[i].CompleteTask[j].End > sm.Data.Orders[OrderIndex].EndTime)
                                {
                                    sm.Data.Orders[OrderIndex].EndTime = sm.Data.Saws[i].CompleteTask[j].End;
                                }
                            }
                            else
                            {
                                sm.Data.Orders[OrderIndex].EndTime = sm.Data.Saws[i].CompleteTask[j].End;
                            }
                        }

                        if (sm.Data.Orders[OrderIndex].DeadLine != null)
                        {
                            if (sm.Data.BaseTime.AddMinutes(sm.Data.Saws[i].CompleteTask[j].End) >
                                sm.Data.Orders[OrderIndex].DeadLine)
                            {
                                // увеличиваем чсетчик просроченных заданий
                                TaskWithFineTime++;

                                // время просрочки для задания в минутах
                                double ExpairTime = (sm.Data.BaseTime.AddMinutes(sm.Data.Saws[i].CompleteTask[j].End) -
                                sm.Data.Orders[OrderIndex].DeadLine.Value).TotalMinutes;

                                // устанавливаем максимальную просрочку для заказа в целом
                                if (sm.Data.Orders[OrderIndex].FineTime < ExpairTime)
                                    sm.Data.Orders[OrderIndex].FineTime = ExpairTime;
                            }
                        }

                    }
                    else if (sm.Data.Saws[i].CompleteTask[j].TaskId == -1)
                    {
                        FullConfTime += TimeForTask;
                    }
                    else if (sm.Data.Saws[i].CompleteTask[j].TaskId == -2)
                    {
                        FullServiceTime += TimeForTask;
                    }
                }
            }

            for (int i = 0; i < sm.Data.Grinders.Count; i++)
            {
                for (int j = 0; j < sm.Data.Grinders[i].CompleteTask.Count; j++)
                {
                    // длительность текущего задания
                    double TimeForTask = (sm.Data.Grinders[i].CompleteTask[j].End - sm.Data.Grinders[i].CompleteTask[j].Begin);

                    FullTime += TimeForTask;

                    if (sm.Data.Grinders[i].CompleteTask[j].TaskId >= 0)
                    {
                        TasksComplete++;
                        FullGoodTime += TimeForTask;

                        int TaskIndex = sm.Data.Tasks.GetIndexById(sm.Data.Grinders[i].CompleteTask[j].TaskId);
                        int OrderIndex = sm.Data.Orders.GetIndexById(sm.Data.Tasks[TaskIndex].OrderId);

                        {
                            if (sm.Data.Orders[OrderIndex].StartTime != -1)
                            {
                                if (sm.Data.Grinders[i].CompleteTask[j].Begin < sm.Data.Orders[OrderIndex].StartTime)
                                {
                                    sm.Data.Orders[OrderIndex].StartTime = sm.Data.Grinders[i].CompleteTask[j].Begin;
                                }
                            }
                            else
                            {
                                sm.Data.Orders[OrderIndex].StartTime = sm.Data.Grinders[i].CompleteTask[j].Begin;
                            }

                            if (sm.Data.Orders[OrderIndex].EndTime != -1)
                            {
                                if (sm.Data.Grinders[i].CompleteTask[j].End > sm.Data.Orders[OrderIndex].EndTime)
                                {
                                    sm.Data.Orders[OrderIndex].EndTime = sm.Data.Grinders[i].CompleteTask[j].End;
                                }
                            }
                            else
                            {
                                sm.Data.Orders[OrderIndex].EndTime = sm.Data.Grinders[i].CompleteTask[j].End;
                            }
                        }

                        if (sm.Data.Orders[OrderIndex].DeadLine != null)
                        {
                            if (sm.Data.BaseTime.AddMinutes(sm.Data.Grinders[i].CompleteTask[j].End) >
                                sm.Data.Orders[OrderIndex].DeadLine)
                            {
                                // увеличиваем счетчик просроченных заданий
                                TaskWithFineTime++;

                                // время просрочки для задания в минутах
                                double ExpairTime = (sm.Data.BaseTime.AddMinutes(sm.Data.Grinders[i].CompleteTask[j].End) -
                                sm.Data.Orders[OrderIndex].DeadLine.Value).TotalMinutes;

                                // устанавливаем максимальную просрочку для заказа в целом
                                if (sm.Data.Orders[OrderIndex].FineTime < ExpairTime)
                                    sm.Data.Orders[OrderIndex].FineTime = ExpairTime;
                            }
                        }

                    }
                    else if (sm.Data.Grinders[i].CompleteTask[j].TaskId == -1)
                    {
                        FullConfTime += TimeForTask;
                    }
                    else if (sm.Data.Grinders[i].CompleteTask[j].TaskId == -2)
                    {
                        FullServiceTime += TimeForTask;
                    }
                }
            }

            CommonGraphInfo = "Всего выполненных заданий на всех станках: " + TasksComplete.ToString();
            CommonGraphInfo += "\r\nИз них просрочено: " + TaskWithFineTime.ToString();
            CommonGraphInfo += "\r\n-----------------------------------------------------";
            CommonGraphInfo += "\r\nСуммарное полное время: " + MinuteToHour(FullTime) + " ( " + (FullTime != 0 ? FullTime / FullTime * 100 : 0).ToString("0.0") + " % )";
            CommonGraphInfo += "\r\nСуммарное время обработки: " + MinuteToHour(FullGoodTime) + " ( " + (FullTime != 0 ? FullGoodTime / FullTime * 100 : 0).ToString("0.0") + " % )";
            CommonGraphInfo += "\r\nСуммарное время настройки: " + MinuteToHour(FullConfTime) + " ( " + (FullTime != 0 ? FullConfTime / FullTime * 100 : 0).ToString("0.0") + " % )";
            CommonGraphInfo += "\r\nСуммарное время обслуживания: " + MinuteToHour(FullServiceTime) + " ( " + (FullTime != 0 ? FullServiceTime / FullTime * 100 : 0).ToString("0.0") + " % )";
            CommonGraphInfo += "\r\n-----------------------------------------------------";
            CommonGraphInfo += "\r\nВремя исполнения по заказам: ";
            for (int i = 0; i < sm.Data.Orders.Count; i++)
            {
                CommonGraphInfo += "\r\n\t" + sm.Data.Orders[i].Text + ": ( " + 
                    ScheduleCore.Configuration.DateToString(sm.Data.BaseTime.AddMinutes(sm.Data.Orders[i].StartTime)) + 
                    " - " + ScheduleCore.Configuration.DateToString(sm.Data.BaseTime.AddMinutes(sm.Data.Orders[i].EndTime)) + " )";
            }
            CommonGraphInfo += "\r\n-----------------------------------------------------";
            CommonGraphInfo += "\r\nШтрафы по заказам: ";
            for (int i = 0; i < sm.Data.Orders.Count; i++)
            {
                CommonGraphInfo += "\r\n\t" + sm.Data.Orders[i].Text + ": " + MinuteToHour(sm.Data.Orders[i].FineTime);
            }

            int FinnestOrderIndex = -1;
            double FinnestOrderTime = 0;
            for (int i = 0; i < sm.Data.Orders.Count; i++)
            {
                if (sm.Data.Orders[i].FineTime > FinnestOrderTime)
                {
                    FinnestOrderIndex = i;
                    FinnestOrderTime = sm.Data.Orders[i].FineTime;
                }
            }
            if (FinnestOrderIndex > 0)
                CommonGraphInfo += "\r\nНаиболее просроченный заказ: " + sm.Data.Orders[FinnestOrderIndex].Text;

            double FinOrderSum = 0;
            for (int i = 0; i < sm.Data.Orders.Count; i++)
            {
                FinOrderSum += sm.Data.Orders[i].FineTime;
            }

            CommonGraphInfo += "\r\nСуммарный штраф по заказам: " + MinuteToHour(FinOrderSum);

            textBox1.Text = CommonGraphInfo; 
        }
        /// <summary>
        /// Преобразует минуты в строковое представление
        /// в формате hh ч. mm мин.
        /// </summary>
        /// <param name="MinuteCount">Минуты для конвертации.</param>
        /// <returns>Строковое представление минут.</returns>
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

        private void GroupCloneTasks_Click(object sender, EventArgs e)
        {
            LoadTasks();
        }

    }
}
