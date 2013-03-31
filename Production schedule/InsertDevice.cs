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
    public partial class InsertDevice : Form
    {
        private Configuration conf;
        private BaseDeviceType devType;
        private MaterialPairList mpl = new MaterialPairList();
        private int DefaultMaterialId = -1;
        private int itemId = -1;

        public InsertDevice(Configuration Conf, BaseDeviceType DevType)
        {
            InitializeComponent();

            conf = Conf;
            devType = DevType;

            if (devType == BaseDeviceType.Saw)
            {
                this.Text = "Добавить новую пилу";
            }

            this.Text = devType == BaseDeviceType.Saw ?
                "Добавить новую пилу" : "Добавить шлифовальный станок"; 

            LoadMaterials();
        }

        public InsertDevice(Configuration Conf, int ItemId, BaseDeviceType DevType)
        {
            InitializeComponent();

            conf = Conf;
            devType = DevType;
            itemId = ItemId;

            this.Text = devType == BaseDeviceType.Saw ?
                "Изменить конфигурацию пилы" : "Изменить конфигурацию шлифовального станка";

            DeviceList GoalList = devType == BaseDeviceType.Saw ?
                conf.Saws : conf.Grinders;

            int DeviceIndex = GoalList.GetIndexById(itemId);

            textBox1.Text = GoalList[DeviceIndex].Text;
            textBox2.Text = GoalList[DeviceIndex].Responsible;
            textBox4.Text = GoalList[DeviceIndex].ServiceTimePeriod.ToString();
            textBox5.Text = GoalList[DeviceIndex].ServiceTime.ToString();

            DefaultMaterialId = GoalList[DeviceIndex].DefaultMaterialId;

            for (int i = 0; i < GoalList[DeviceIndex].SupportedMaterials.Count; i++)
            {
                mpl.Add((MaterialPair)GoalList[DeviceIndex].SupportedMaterials[i].Clone());
            }           

            LoadMaterials();
            LoadSupMaterials();
        }

        private void LoadMaterials()
        {
            for (int i = 0; i < conf.Materials.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem(conf.Materials[i].Id, conf.Materials[i].Text);
                comboBox1.Items.Add(item);
            }
        }

        private void LoadSupMaterials()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            for (int i = 0; i < mpl.Count; i++ )
            {
                ListViewItem lvi = new ListViewItem(conf.Materials.GetTextById(mpl[i].ID));
                if (mpl[i].ID == DefaultMaterialId)
                {
                    lvi.ImageIndex = 0;
                }
                lvi.SubItems.Add(mpl[i].Time.ToString() + " мин.");
                lvi.Tag = i;
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string FailMessage = "";
            string Name = textBox1.Text;

            if (Name.Length == 0)
            {
                FailMessage = "Необходимо указать название станка!";
                goto fail_exit; 
            }

            if (itemId == -1)
            {
                if (devType == BaseDeviceType.Saw ? conf.Saws.GetIndexByText(Name) != -1 :
                    conf.Grinders.GetIndexByText(Name) != -1)
                {
                    FailMessage = "Станок с таким названием уже существует!";
                    goto fail_exit;
                }
            }

            if (textBox4.Text.Length == 0)
            {
                FailMessage = "Необходимо задать переодичность технического обслуживания!";
                goto fail_exit;
            }

            int ServiceTimePeriod = -1;
            try
            {
                ServiceTimePeriod = Convert.ToInt32(textBox4.Text);
            }
            catch (Exception ex)
            {
                FailMessage = "Период обслуживания: " + ex.Message;
                goto fail_exit;
            }

            if (textBox4.Text.Length == 0)
            {
                FailMessage = "Необходимо задать время технического обслуживания!";
                goto fail_exit;
            }


            int ServiceTime = -1;
            try
            {
                ServiceTime = Convert.ToInt32(textBox5.Text);
            }
            catch (Exception ex)
            {
                FailMessage = "Время технического обслуживания: " + ex.Message;
                goto fail_exit;
            }

            if (itemId == -1)
            {
                DeviceList GoalList = devType == BaseDeviceType.Saw ?
                    conf.Saws : conf.Grinders;
                TimeDevice NewDevice = new TimeDevice(GoalList.GetFreeId(), Name, textBox2.Text);
                NewDevice.SupportedMaterials = mpl;
                NewDevice.ServiceTimePeriod = ServiceTimePeriod;
                NewDevice.ServiceTime = ServiceTime;
                NewDevice.DefaultMaterialId = DefaultMaterialId;
                GoalList.Add(NewDevice);
            }
            else
            {
                DeviceList GoalList = devType == BaseDeviceType.Saw ?
                    conf.Saws : conf.Grinders;

                int DeviceIndex = GoalList.GetIndexById(itemId);
                GoalList[DeviceIndex].Text = Name;
                GoalList[DeviceIndex].Responsible = textBox2.Text;
                GoalList[DeviceIndex].SupportedMaterials = mpl;
                GoalList[DeviceIndex].ServiceTimePeriod = ServiceTimePeriod;
                GoalList[DeviceIndex].ServiceTime = ServiceTime;
                GoalList[DeviceIndex].DefaultMaterialId = DefaultMaterialId;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            return;

        fail_exit:
            MessageBox.Show(this, FailMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string FailMessage = "";


            if (comboBox1.SelectedItem == null)
            {
                FailMessage = "Необходимо выбрать материал!";
                goto fail_exit;  
            }

            int MaterialId = ((ComboBoxItem)comboBox1.SelectedItem).Id;
            {
                bool MaterialExist = false;
                for (int i = 0; i < mpl.Count; i++)
                {
                    if (mpl[i].ID == MaterialId)
                    {
                        MaterialExist = true;
                        break;
                    }
                }

                if (MaterialExist)
                {
                    FailMessage = "Данный материал уже есть в списке!";
                    goto fail_exit;   
                }
            }

            if (textBox3.Text.Length == 0)
            {
                FailMessage = "Необходимо указать время настройки для станка!";
                goto fail_exit;  
            }

            int ConfTime = -1;
            try 
            {
                ConfTime = Convert.ToInt32(textBox3.Text);
            }
            catch(Exception ex)
            {
                FailMessage = "Время конфигурации: " + ex.Message;
                goto fail_exit; 
            }

            mpl.Add(new MaterialPair(MaterialId, ConfTime));
            LoadSupMaterials();
            return;

        fail_exit:
            MessageBox.Show(this, FailMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSupMaterials();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                удалитьToolStripMenuItem.Enabled = true;
                поУмолчаниюToolStripMenuItem.Enabled = true;
            }
            else
            {
                удалитьToolStripMenuItem.Enabled = false;
                поУмолчаниюToolStripMenuItem.Enabled = false;
            }
        }

        private void DeleteSupMaterials()
        {
            if (listView1.SelectedItems.Count != 0)
            {
                Stack<int> IndexesToDelete = new Stack<int>();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    IndexesToDelete.Push((int)item.Tag);
                }

                if (IndexesToDelete.Count != 0)
                {
                    while (IndexesToDelete.Count != 0)
                    {
                        int mplIndex = IndexesToDelete.Pop();
                        if (mpl[mplIndex].ID == DefaultMaterialId)
                            DefaultMaterialId = -1;
                        mpl.RemoveAt(mplIndex);
                    }

                    LoadSupMaterials();
                }
            } 
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteSupMaterials();
        }

        private void поУмолчаниюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                ListViewItem LastSelectedItem = null;
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.ImageIndex == 0)
                    {
                        LastSelectedItem = item;
                        item.ImageIndex = -1;
                        break;
                    }
                }
                if (LastSelectedItem != null)
                {
                    if (LastSelectedItem == listView1.SelectedItems[0])
                    {
                        DefaultMaterialId = -1;
                        listView1.Refresh();
                        return;
                    }
                }

                int mplIndex = (int)listView1.SelectedItems[0].Tag;
                DefaultMaterialId = mpl[mplIndex].ID;
                listView1.SelectedItems[0].ImageIndex = 0;
                listView1.Refresh();
            } 
        }
    }
}
