namespace GantChart2
{
    partial class GantChart
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkAutoScale = new System.Windows.Forms.CheckBox();
            this.TimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.TimePickerBegin = new System.Windows.Forms.DateTimePicker();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSelectItem = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.comboBoxSelectOrder = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.graphField1 = new GantChart2.GraphField();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.graphField1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 26);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(577, 320);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.checkAutoScale);
            this.panel2.Controls.Add(this.TimePickerEnd);
            this.panel2.Controls.Add(this.TimePickerBegin);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 346);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(577, 26);
            this.panel2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(366, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "по";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(104, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Дата с:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkAutoScale
            // 
            this.checkAutoScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkAutoScale.Checked = true;
            this.checkAutoScale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutoScale.Location = new System.Drawing.Point(3, 3);
            this.checkAutoScale.Name = "checkAutoScale";
            this.checkAutoScale.Size = new System.Drawing.Size(118, 20);
            this.checkAutoScale.TabIndex = 2;
            this.checkAutoScale.Text = "Автомасштаб";
            this.checkAutoScale.UseVisualStyleBackColor = true;
            this.checkAutoScale.CheckedChanged += new System.EventHandler(this.checkAutoScale_CheckedChanged);
            // 
            // TimePickerEnd
            // 
            this.TimePickerEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TimePickerEnd.CustomFormat = "HH:mm / dd MMMM yyyy г.";
            this.TimePickerEnd.Enabled = false;
            this.TimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.TimePickerEnd.Location = new System.Drawing.Point(396, 3);
            this.TimePickerEnd.Name = "TimePickerEnd";
            this.TimePickerEnd.Size = new System.Drawing.Size(178, 20);
            this.TimePickerEnd.TabIndex = 1;
            this.TimePickerEnd.ValueChanged += new System.EventHandler(this.TimePickerEnd_ValueChanged);
            // 
            // TimePickerBegin
            // 
            this.TimePickerBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TimePickerBegin.CustomFormat = "HH:mm / dd MMMM yyyy г.";
            this.TimePickerBegin.Enabled = false;
            this.TimePickerBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.TimePickerBegin.Location = new System.Drawing.Point(182, 3);
            this.TimePickerBegin.Name = "TimePickerBegin";
            this.TimePickerBegin.Size = new System.Drawing.Size(178, 20);
            this.TimePickerBegin.TabIndex = 0;
            this.TimePickerBegin.ValueChanged += new System.EventHandler(this.TimePickerBegin_ValueChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.comboBoxSelectOrder);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.comboBoxSelectItem);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(577, 26);
            this.panel3.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(295, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Отслеживать задание:";
            // 
            // comboBoxSelectItem
            // 
            this.comboBoxSelectItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSelectItem.FormattingEnabled = true;
            this.comboBoxSelectItem.Location = new System.Drawing.Point(424, 3);
            this.comboBoxSelectItem.Name = "comboBoxSelectItem";
            this.comboBoxSelectItem.Size = new System.Drawing.Size(150, 21);
            this.comboBoxSelectItem.TabIndex = 0;
            this.comboBoxSelectItem.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectItem_SelectedIndexChanged);
            this.comboBoxSelectItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxSelectItem_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // comboBoxSelectOrder
            // 
            this.comboBoxSelectOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSelectOrder.FormattingEnabled = true;
            this.comboBoxSelectOrder.Location = new System.Drawing.Point(139, 3);
            this.comboBoxSelectOrder.Name = "comboBoxSelectOrder";
            this.comboBoxSelectOrder.Size = new System.Drawing.Size(150, 21);
            this.comboBoxSelectOrder.TabIndex = 2;
            this.comboBoxSelectOrder.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectOrder_SelectedIndexChanged);
            this.comboBoxSelectOrder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxSelectOrder_KeyDown);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Отслеживать заказ:";
            // 
            // graphField1
            // 
            this.graphField1.AllPerimeterColorShadow = System.Drawing.Color.DarkGray;
            this.graphField1.ConfColor1 = System.Drawing.Color.DarkGray;
            this.graphField1.ConfColor1Shadow = System.Drawing.Color.DarkGray;
            this.graphField1.ConfColor2 = System.Drawing.Color.LightGray;
            this.graphField1.ConfColor2Shadow = System.Drawing.Color.LightGray;
            this.graphField1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphField1.DrawItemText = false;
            this.graphField1.Items = null;
            this.graphField1.Location = new System.Drawing.Point(0, 0);
            this.graphField1.Name = "graphField1";
            this.graphField1.ServColor1 = System.Drawing.Color.DarkGray;
            this.graphField1.ServColor1Shadow = System.Drawing.Color.DarkGray;
            this.graphField1.ServColor2 = System.Drawing.Color.LightGreen;
            this.graphField1.ServColor2Shadow = System.Drawing.Color.LightGreen;
            this.graphField1.Size = new System.Drawing.Size(577, 320);
            this.graphField1.TabIndex = 0;
            this.graphField1.TaskColorFill = System.Drawing.Color.SkyBlue;
            this.graphField1.TaskColorFillAlert = System.Drawing.Color.Red;
            this.graphField1.TaskColorFillShadow = System.Drawing.Color.LightGray;
            this.graphField1.TaskColorPerimeter = System.Drawing.Color.Blue;
            this.graphField1.Resize += new System.EventHandler(this.graphField1_Resize);
            // 
            // GantChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "GantChart";
            this.Size = new System.Drawing.Size(577, 372);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private GraphField graphField1;
        private System.Windows.Forms.CheckBox checkAutoScale;
        private System.Windows.Forms.DateTimePicker TimePickerEnd;
        private System.Windows.Forms.DateTimePicker TimePickerBegin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comboBoxSelectItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxSelectOrder;
    }
}
