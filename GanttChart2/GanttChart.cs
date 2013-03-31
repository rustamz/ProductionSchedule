using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GantChart2
{
    public partial class GantChart : UserControl
    {
        public GantChart()
        {
            InitializeComponent();
            items = new Rows();

            graphField1.SelectItem += new GraphField.ItemChanged(GraphFieldItemChanged);
            graphField1.SelectLostItem += new GraphField.ItemChanged(GraphFieldItemChangedLost);
        }

        #region Поля
        
        private Rows items;

        private bool autoSizeDrawField = true;

        #endregion

        #region События
        public delegate void ItemChanged(Object sender, GantChangedEvent arg);

        /// <summary>
        /// Событие возникает, когда выделяется событие.
        /// </summary>
        public event ItemChanged SelectItem;

        /// <summary>
        /// Событие возникает, когда выделяется событие.
        /// </summary>
        public event ItemChanged SelectLostItem;
        #endregion

        #region Свойства
        /// <summary>
        /// Возвращает или задаёт коллекцию элементов для представления.
        /// </summary>
        public Rows Items { get { return items; } set { items = value; } }

        /// <summary>
        /// Предоставляет доступ к элементам Combobox-а с заданиями
        /// </summary>
        public ComboBox.ObjectCollection ComboTaskItems
        {
            get { return comboBoxSelectItem.Items; }
        }

        /// <summary>
        /// Предоставляет доступ к элементам Combobox-а с заданиями
        /// </summary>
        public ComboBox.ObjectCollection ComboOrderItems
        {
            get { return comboBoxSelectOrder.Items; }
        }

        public bool AutoSizeDrawField
        {
            get
            {
                return autoSizeDrawField;
            }

            set
            {
                autoSizeDrawField = value;
                if (autoSizeDrawField)
                {
                    graphField1.Dock = DockStyle.Fill;
                }
                else
                {
                    graphField1.Dock = DockStyle.None;
                }
            }
        }

        public int DrawFieldWidth
        {
            get { return graphField1.Width; }
            set { if (!autoSizeDrawField) graphField1.Width = value; }
        }

        public int DrawFieldHeight
        {
            get { return graphField1.Height; }
            set { if (!autoSizeDrawField) graphField1.Height = value; }
        }


        /// <summary>
        /// Отрисовывать текст на элементах диаграммы или нет.
        /// </summary>
        public bool DrawItemText { get { return graphField1.DrawItemText; } set { graphField1.DrawItemText = value; } }

        public Color TaskColorFill
        {
            get { return graphField1.TaskColorFill; }
            set { graphField1.TaskColorFill = value; }
        }


        public Color TaskColorPerimeter
        {
            get { return graphField1.TaskColorPerimeter; }
            set { graphField1.TaskColorPerimeter = value; }
        }

        public Color TaskColorFillAlert
        {
            get { return graphField1.TaskColorFillAlert; }
            set { graphField1.TaskColorFillAlert = value; }
        }

        public Color TaskColorFillOrder
        {
            get { return graphField1.TaskColorFillOrder; }
            set { graphField1.TaskColorFillOrder = value; }
        }

        public Color ConfColor1
        {
            get { return graphField1.ConfColor1; }
            set { graphField1.ConfColor1 = value; }
        }

        public Color ConfColor2
        {
            get { return graphField1.ConfColor2; }
            set { graphField1.ConfColor2 = value; }
        }

        public Color ServColor1
        {
            get { return graphField1.ServColor1; }
            set { graphField1.ServColor1 = value; }
        }

        public Color ServColor2
        {
            get { return graphField1.ServColor2; }
            set { graphField1.ServColor2 = value; }
        }


        public Color TaskColorFillShadow
        {
            get { return graphField1.TaskColorFillShadow; }
            set { graphField1.TaskColorFillShadow = value; }
        }

        public Color TaskColorFillOrderShadow
        {
            get { return graphField1.TaskColorFillOrderShadow; }
            set { graphField1.TaskColorFillOrderShadow = value; }
        }

        public Color ConfColor1Shadow
        {
            get { return graphField1.ConfColor1Shadow; }
            set { graphField1.ConfColor1Shadow = value; }
        }

        public Color ConfColor2Shadow
        {
            get { return graphField1.ConfColor2Shadow; }
            set { graphField1.ConfColor2Shadow = value; }
        }

        public Color ServColor1Shadow
        {
            get { return graphField1.ServColor1Shadow; }
            set { graphField1.ServColor1Shadow = value; }
        }

        public Color ServColor2Shadow
        {
            get { return graphField1.ServColor2Shadow; }
            set { graphField1.ServColor2Shadow = value; }
        }

        public Color AllPerimeterColorShadow
        {
            get { return graphField1.AllPerimeterColorShadow; }
            set { graphField1.AllPerimeterColorShadow = value; }
        }

        #endregion
        
        /// <summary>
        /// Вызывает пересчёт параметров и перерисовку графика.
        /// </summary>
        public void BuildAndDrawGraphic()
        {
            DateTime minTime = new DateTime();
            DateTime maxTime = new DateTime();

            if (checkAutoScale.Checked && items != null)
            {
                // определяем наименьшую и наибольшую дату в списке
                bool minTimeSet = false;
                bool maxTimeSet = false;

                foreach (Row row_item in items)
                {
                    foreach (RowItem item in row_item)
                    {
                        if (minTimeSet)
                        {
                            if (minTime > item.BeginDate)
                                minTime = item.BeginDate;
                        }
                        else
                        {
                            minTimeSet = true;
                            minTime = item.BeginDate;
                        }

                        if (maxTimeSet)
                        {
                            if (maxTime < item.EndDate)
                                maxTime = item.EndDate;
                        }
                        else
                        {
                            maxTimeSet = true;
                            maxTime = item.EndDate;
                        }
                    }
                }
                if (!minTimeSet || !maxTimeSet)
                    return;

                TimePickerBegin.Value = minTime;
                TimePickerEnd.Value = maxTime;
            }
            else
            {
                minTime = TimePickerBegin.Value;
                maxTime = TimePickerEnd.Value;
            }

            graphField1.CalcCutRect(items, minTime, maxTime);
            Refresh();
        }

        private void TimePickerBegin_ValueChanged(object sender, EventArgs e)
        {
            BuildAndDrawGraphic();
        }

        private void TimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            BuildAndDrawGraphic();
        }

        private void checkAutoScale_CheckedChanged(object sender, EventArgs e)
        {
            TimePickerBegin.Enabled = TimePickerEnd.Enabled = !checkAutoScale.Checked;
            BuildAndDrawGraphic();
        }

        private void graphField1_Resize(object sender, EventArgs e)
        {
            BuildAndDrawGraphic();
        }

        private void comboBoxSelectItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSelectItem.SelectedItem != null)
            {
                int TaskId = ((ComboBoxItem)comboBoxSelectItem.SelectedItem).Index;
                graphField1.SetSelected(TaskId);
            }
            else
            {
                graphField1.SetSelected(-1);
            }
        }

        private void GraphFieldItemChanged(Object sender, GantChangedEvent arg)
        {
            if (arg.SelectedItem.Index > 0)
            {
                foreach (object item in comboBoxSelectItem.Items)
                    if (((ComboBoxItem)item).Index == arg.SelectedItem.Index)
                    {
                        comboBoxSelectItem.SelectedItem = item;
                        break;
                    }
            }
            else
            {
                comboBoxSelectItem.Text = ""; 
            }
            if (SelectItem != null)
                SelectItem(this, arg);
        }

        private void GraphFieldItemChangedLost(Object sender, GantChangedEvent arg)
        {
            comboBoxSelectItem.SelectedItem = null;
            comboBoxSelectItem.Text = "";
            if (SelectLostItem != null)
                SelectLostItem(this, arg);
        }

        private void comboBoxSelectItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                string TextToFind = comboBoxSelectItem.Text;
                if (TextToFind.Length == 0)
                    graphField1.SetSelected(-1);
                else
                {
                    foreach (object item in comboBoxSelectItem.Items)
                        if (((ComboBoxItem)item).Text == TextToFind)
                        {
                            comboBoxSelectItem.SelectedItem = item;
                            graphField1.SetSelected(((ComboBoxItem)comboBoxSelectItem.SelectedItem).Index);
                            break;
                        }
                }
            }
        }

        private void comboBoxSelectOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSelectOrder.SelectedItem != null)
            {
                int OrderId = ((ComboBoxItem)comboBoxSelectOrder.SelectedItem).Index;
                graphField1.SetLightedOrderId(OrderId);
            }
            else
            {
                graphField1.SetLightedOrderId(-1);
            }
        }

        private void comboBoxSelectOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                string TextToFind = comboBoxSelectOrder.Text;
                if (TextToFind.Length == 0)
                    graphField1.SetLightedOrderId(-1);
                else
                {
                    foreach (object item in comboBoxSelectOrder.Items)
                        if (((ComboBoxItem)item).Text == TextToFind)
                        {
                            comboBoxSelectOrder.SelectedItem = item;
                            graphField1.SetLightedOrderId(((ComboBoxItem)comboBoxSelectOrder.SelectedItem).Index);
                            break;
                        }
                }
            }
        }
 
    }
}
