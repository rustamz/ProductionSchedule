using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GantChart2
{
    internal partial class GraphField : UserControl
    {
        public delegate void ItemChanged(Object sender, GantChangedEvent arg);
        public event ItemChanged SelectItem;
        public event ItemChanged SelectLostItem;
        
        public GraphField()
        {
            InitializeComponent();

            DateFont = new Font(this.Font.FontFamily, this.Font.Size - 1, FontStyle.Italic);
            SignFont = new Font(this.Font.FontFamily, this.Font.Size - 1, FontStyle.Italic);

            axesScalePen.DashStyle = DashStyle.Dash;


            CreateSelectObjects(selTaskColorFill,
                                selTaskColorPerimeter,
                                selTaskColorFillAlert,
                                selLightedOrder,
                                selConfColor1,
                                selConfColor2,
                                selServColor1,
                                selServColor2);

            CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
        }

        #region Глобавльные переменные класса
        private RowItem RegionItem = null; // Элемент над которым находится указатель в данный момент
        private int LightedOrderId = -1; // идентификатор подсвечиваемого заказа
        private SizeF strSize; // Геометрический размер наибольшей строки в названии строки диаграммы гантта
        private int AxesXZeroPoint;
        private int AxesYZeroPoint;
        private float GraphHeight;
        #endregion

        #region Настройки графика

        private Color selTaskColorFill = Color.SkyBlue;
        private Color selTaskColorPerimeter = Color.Blue;
        private Color selTaskColorFillAlert = Color.Red;
        private Color selLightedOrder = Color.Yellow;
        private Color selConfColor1 = Color.DarkGray;
        private Color selConfColor2 = Color.LightGray;
        private Color selServColor1 = Color.DarkGray;
        private Color selServColor2 = Color.LightGreen;

        private Color shadowTaskColorFill = Color.LightGray;
        private Color shadowLightedOrder = Color.Yellow;
        private Color shadowConfColor1 = Color.DarkGray;
        private Color shadowConfColor2 = Color.LightGray;
        private Color shadowServColor1 = Color.DarkGray;
        private Color shadowServColor2 = Color.LightGreen;
        private Color shadowAllPerimeterColor = Color.DarkGray;

        private float rowIndentHor = 5; // горизонтальный отступ для элементов диаграммы гантта
        private float rowIndentVer = 2; // вертикальный отступ для элементов диаграммы гантта
        private float rowHeight =   24; // высота элемента диаграммы гантта

        // кистья и перья для отрсовки выделенного элемента диаграммы гантта
        private Brush selectBrush = null;
        private Brush selectOrderBrush = null;
        private Brush selectAlertBrush = null;
        private Brush selectBrushConf = null;
        private Brush selectBrushServ = null;
        private Pen selectPen = null;
        private Brush selectBrushText = null; 

        // кистья и перья для отрисовки затемненных элементов диаграммы гантта
        Brush shadowBrush = null;
        Brush shadowBrushConf = null;
        Brush shadowBrushOrder = null;
        Brush shadowBrushServ = null;
        Pen shadowPen = null;

        // Перья для отрисовки осей и дополнительных линий
        private Pen axesVerticalPen = new Pen(new SolidBrush(Color.Gray), 2.0f);
        private Pen axesHorizontalPen = new Pen(new SolidBrush(Color.LightGray), 1.0f);
        private Pen axesScalePen = new Pen(new SolidBrush(Color.Gray), 1.0f);


        private Font DateFont = null; // шрифт для распечатки даты
        private Font SignFont = null; // шрифт для подписи элементов

        private bool drawItemText = false;
        
        #endregion


        /// <summary>
        /// Коллекция элементов для представления
        /// </summary>
        private Rows items;
        
        /// <summary>
        /// Первая дата в графике.
        /// </summary>
        private DateTime minTime = DateTime.Now;

        /// <summary>
        /// Последняя дата в графике.
        /// </summary>
        private DateTime maxTime = DateTime.Now.AddDays(1);

        #region Свойства

        /// <summary>
        /// Возвращает или задаёт коллекцию элементов для представления.
        /// </summary>
        public Rows Items { get { return items; } set { items = value; } }

        /// <summary>
        /// Отрисовывать текст на элементах диаграммы или нет.
        /// </summary>
        public bool DrawItemText { get { return drawItemText; } set { drawItemText = value; } }

        public Color TaskColorFill 
        { 
            get { return selTaskColorFill; }
            set 
            {
                selTaskColorFill = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);               
            }
        }


        public Color TaskColorPerimeter
        {
            get { return selTaskColorPerimeter; }
            set
            {
                selTaskColorPerimeter = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);
            }
        }

        public Color TaskColorFillAlert
        {
            get { return selTaskColorFillAlert; }
            set
            {
                selTaskColorFillAlert = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);
            }
        }

        public Color TaskColorFillOrder
        {
            get { return selLightedOrder; }
            set
            {
                selLightedOrder = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);
            }
        }

        public Color ConfColor1
        {
            get { return selConfColor1; }
            set
            {
                selConfColor1 = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);
            }
        }

        public Color ConfColor2
        {
            get { return selConfColor2; }
            set
            {
                selConfColor2 = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);
            }
        }

        public Color ServColor1
        {
            get { return selServColor1; }
            set
            {
                selServColor1 = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);
            }
        }

        public Color ServColor2
        {
            get { return selServColor2; }
            set
            {
                selServColor2 = value;
                CreateSelectObjects(selTaskColorFill,
                                    selTaskColorPerimeter,
                                    selTaskColorFillAlert,
                                    selLightedOrder,
                                    selConfColor1,
                                    selConfColor2,
                                    selServColor1,
                                    selServColor2);
            }
        }
 
        public Color TaskColorFillShadow
        {
            get { return shadowTaskColorFill; }
            set 
            {
                shadowTaskColorFill = value;
                CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
            }
        }

        public Color ConfColor1Shadow
        {
            get { return shadowConfColor1; }
            set
            {
                shadowConfColor1 = value;
                CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
            }
        }

        public Color TaskColorFillOrderShadow
        {
            get { return shadowLightedOrder; }
            set
            {
                shadowLightedOrder = value;
                CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
            }
        }

        public Color ConfColor2Shadow
        {
            get { return shadowConfColor2; }
            set
            {
                shadowConfColor2 = value;
                CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
            }
        }

        public Color ServColor1Shadow
        {
            get { return shadowServColor1; }
            set
            {
                shadowServColor1 = value;
                CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
            }
        }

        public Color ServColor2Shadow
        {
            get { return shadowServColor2; }
            set
            {
                shadowServColor2 = value;
                CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
            }
        }

        public Color AllPerimeterColorShadow
        {
            get { return shadowAllPerimeterColor; }
            set
            {
                shadowAllPerimeterColor = value;
                CreateShadowObjects(shadowTaskColorFill,
                                shadowLightedOrder,
                                shadowConfColor1,
                                shadowConfColor2,
                                shadowServColor1,
                                shadowServColor2,
                                shadowAllPerimeterColor);
            }
        }

        #endregion


        #region Методы
        /// <summary>
        /// Отрисовка графика
        /// </summary>
        /// <param name="gr"></param>
        private void DrawGraph(Graphics gr)
        {
            // не рисуем ничего, если нет элементов
            if (items == null)
                return;
            if (items.Count == 0)
                return;

            float CurrentPoint = AxesYZeroPoint;
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Far;
            format.FormatFlags = StringFormatFlags.FitBlackBox;
            for (int i = 0; i < items.Count; i++)
            {
                gr.DrawString(items[i].Text, base.Font, new SolidBrush(Color.Black), new RectangleF(rowIndentHor, CurrentPoint, strSize.Width, rowHeight), format);
                CurrentPoint += rowHeight;
            }

            // отрисовка горизонтальных линий
            float CurrentLine = AxesYZeroPoint + rowHeight;
            for (int i = 0; i < items.Count - 1; i++)
            {
                gr.DrawLine(axesHorizontalPen, new PointF(AxesXZeroPoint, CurrentLine), new PointF(Width - rowIndentHor, CurrentLine));
                CurrentLine += (int)rowHeight;
            }

            StringFormat RegionFormat = new StringFormat();
            RegionFormat.LineAlignment = StringAlignment.Center;
            RegionFormat.Alignment = StringAlignment.Center;
            RegionFormat.FormatFlags = StringFormatFlags.NoWrap;


            // отрисовка прямоугольников
            if (RegionItem != null)
            {
                Row SelectedRegions = new Row("");

                // находим первый регион
                RowItem FirstItem = RegionItem;
                while (FirstItem.Prev != null)
                    FirstItem = FirstItem.Prev;
                // формируем список выделенных регионов
                while (FirstItem != null)
                {
                    SelectedRegions.Add(FirstItem);
                    FirstItem = FirstItem.Next;
                }


                // рисуем затемнённые регионы
                for (int i = 0; i < items.Count; i++)
                {
                    for (int j = 0; j < items[i].Count; j++)
                    {
                        if (!SelectedRegions.Contains(items[i][j]))
                        {
                            if (items[i][j].Index >= 0)
                            {
                                if (items[i][j].OrderIndex == LightedOrderId)
                                    gr.FillRectangle(shadowBrushOrder, items[i][j].DrawRegion);
                                else
                                    gr.FillRectangle(shadowBrush, items[i][j].DrawRegion);
                            }
                            else if (items[i][j].Index == -1)
                            {
                                gr.FillRectangle(shadowBrushConf, items[i][j].DrawRegion);
                            }
                            else
                            {
                                gr.FillRectangle(shadowBrushServ, items[i][j].DrawRegion);
                            }

                            gr.DrawRectangle(shadowPen, items[i][j].DrawRegion);
                        }
                    }
                }

                // рисуем выделенные регионы
                Pen ConnectionPen = new Pen(Color.Blue, 1);
                for (int i = 0; i < SelectedRegions.Count; i++)
                {
                    if (SelectedRegions[i].Index >= 0)
                    {
                        gr.FillRectangle(selectBrush, SelectedRegions[i].DrawRegion);

                    }
                    else if (SelectedRegions[i].Index == -1)
                    {
                        gr.FillRectangle(selectBrushConf, SelectedRegions[i].DrawRegion);
                    }
                    else
                    {
                        gr.FillRectangle(selectBrushServ, SelectedRegions[i].DrawRegion);
                    }

                    
                    gr.DrawRectangle(selectPen, SelectedRegions[i].DrawRegion);
                    if (drawItemText)
                        gr.DrawString(SelectedRegions[i].Text, SignFont, selectBrushText, SelectedRegions[i].DrawRegion, RegionFormat);
                    if (i + 1 < SelectedRegions.Count) // если есть следующий блок, то рисуем стрелочки
                    {
                        int i2 = i + 1;
                        Point[] Lines = new Point[] 
                            {
                                new Point(SelectedRegions[i].DrawRegion.Right, SelectedRegions[i].DrawRegion.Bottom - SelectedRegions[i].DrawRegion.Height / 2),
                                new Point(SelectedRegions[i].DrawRegion.Right + (int)rowIndentHor, SelectedRegions[i].DrawRegion.Bottom - SelectedRegions[i].DrawRegion.Height / 2),
                                new Point(SelectedRegions[i].DrawRegion.Right + (int)rowIndentHor, 
                                    SelectedRegions[i2].DrawRegion.Y > SelectedRegions[i].DrawRegion.Y ?
                                    SelectedRegions[i2].DrawRegion.Y - (int)rowIndentVer : SelectedRegions[i2].DrawRegion.Bottom + (int)rowIndentVer),
                                new Point(SelectedRegions[i].DrawRegion.Right + (int)rowIndentHor, 
                                    SelectedRegions[i2].DrawRegion.Y > SelectedRegions[i].DrawRegion.Y ?
                                    SelectedRegions[i2].DrawRegion.Y - (int)rowIndentVer : SelectedRegions[i2].DrawRegion.Bottom + (int)rowIndentVer),
                                new Point(SelectedRegions[i2].DrawRegion.X - (int)rowIndentHor, 
                                    SelectedRegions[i2].DrawRegion.Y > SelectedRegions[i].DrawRegion.Y ?
                                    SelectedRegions[i2].DrawRegion.Y - (int)rowIndentVer : SelectedRegions[i2].DrawRegion.Bottom + (int)rowIndentVer),
                                new Point(SelectedRegions[i2].DrawRegion.X - (int)rowIndentHor, SelectedRegions[i2].DrawRegion.Bottom - SelectedRegions[i2].DrawRegion.Height / 2),
                                new Point(SelectedRegions[i2].DrawRegion.X, SelectedRegions[i2].DrawRegion.Bottom - SelectedRegions[i2].DrawRegion.Height / 2)
                            };

                        gr.DrawLines(ConnectionPen, Lines);
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    for (int j = 0; j < items[i].Count; j++)
                    {
                        if (items[i][j].Index >= 0) // регионы выполнения
                        {
                            if (items[i][j].IsAlert)
                            {
                                gr.FillRectangle(selectAlertBrush, items[i][j].DrawRegion);
                            }
                            else
                            {
                                if (items[i][j].OrderIndex == LightedOrderId)
                                    gr.FillRectangle(selectOrderBrush, items[i][j].DrawRegion);
                                else
                                    gr.FillRectangle(selectBrush, items[i][j].DrawRegion);
                            }
                        }
                        else // регионы настройки
                        {
                            if (items[i][j].Index == -1)
                            {
                                gr.FillRectangle(selectBrushConf, items[i][j].DrawRegion);
                            }
                            else
                            {
                                gr.FillRectangle(selectBrushServ, items[i][j].DrawRegion);
                            }
                        }
                        gr.DrawRectangle(selectPen, items[i][j].DrawRegion);
                        if (drawItemText)
                            gr.DrawString(items[i][j].Text, SignFont, new SolidBrush(Color.Black), items[i][j].DrawRegion, RegionFormat);
                    }
                }
            }

            // рисуем начало координат
            float ScaleYBegin = AxesYZeroPoint - rowIndentVer;
            float ScaleYEnd = AxesYZeroPoint + items.Count * rowHeight + rowIndentVer;
            int AxesIndent = 80;
            gr.DrawLine(axesVerticalPen, new PointF(AxesXZeroPoint, ScaleYBegin), new PointF(AxesXZeroPoint, ScaleYEnd));

            double MinCount = (maxTime - minTime).TotalMinutes;
            float GraphWidth = Width - AxesXZeroPoint - rowIndentHor;
            for (int i = (int)AxesXZeroPoint, i_end = Width - (int)rowIndentHor;
                i < i_end; i += AxesIndent)
            {
                PointF p1 = new PointF(i, ScaleYBegin);
                gr.DrawLine(axesScalePen, p1, new PointF(i, ScaleYEnd));
                DrawDate(gr, p1, minTime.AddMinutes(MinCount * ((i - AxesXZeroPoint) / GraphWidth)));
            }

        }

        /// <summary>
        /// Отрисовывает дату в переданный графический контекст. Дата рисуется в две строки. В первой размещается, собственно дата, а на второй время.
        /// </summary>
        /// <param name="gr">Графический контекст, куда будет осуществляться отрисовка.</param>
        /// <param name="Pos">Координаты отрисовки.</param>
        /// <param name="Date">Дата, которая будет выведена на графический контекст.</param>
        private void DrawDate(Graphics gr, PointF Pos, DateTime Date)
        {
            string Result = (Date.Day > 9 ? Date.Day.ToString() : "0" + Date.Day) + "." +
                (Date.Month > 9 ? Date.Month.ToString() : "0" + Date.Month) + "." + 
                Date.Year + "\r\n" +
                (Date.Hour > 9 ? Date.Hour.ToString() : "0" + Date.Hour) + ":" + 
                (Date.Minute > 9 ? Date.Minute.ToString() : "0" + Date.Minute);

            SizeF ResultSize = gr.MeasureString(Result, DateFont);

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.FitBlackBox;
            float XPos = Pos.X - ResultSize.Width / 2;
            if (XPos + ResultSize.Width > Width)
            {
                XPos -= XPos + ResultSize.Width - Width;
                format.Alignment = StringAlignment.Far;
            }
            gr.DrawString(Result, DateFont, new SolidBrush(Color.Gray),
                new RectangleF(XPos, Pos.Y - ResultSize.Height, ResultSize.Width, ResultSize.Height), format);
        }

        /// <summary>
        /// Рассчет параметров графика
        /// </summary>
        /// <param name="OriginalItems">Коллекция элементов для отображения</param>
        /// <param name="MinDate">Начальная дата для масштабирования</param>
        /// <param name="MaxDate">Конечная дата для масштабирования</param>
        public void CalcCutRect(Rows OriginalItems, DateTime MinDate, DateTime MaxDate)
        {
            RegionItem = null;
            // устанавливаем масштаб
            minTime = MinDate;
            maxTime = MaxDate;
            if (OriginalItems == null)
                return;
            // определяем высоту для данного шрифта
            {
                Graphics gr = CreateGraphics();
                strSize = gr.MeasureString(GetBiggerString(OriginalItems), base.Font);
                SizeF DateString = gr.MeasureString("sometext\r\nsometext", DateFont);
                GraphHeight = rowHeight * OriginalItems.Count + DateString.Height + 2 * rowIndentVer;
            }
            if (GraphHeight > Height) // если график не вмещается изменяем размер холста
            {
                this.Height = (int)Math.Round(GraphHeight);
            }
            else
            {
                // если график вмещается, то устанавливаем размер холста, как у родительского контейнера
                this.Height = base.Height;
            }
            float GraphBeginY = (Height - GraphHeight) / 2;

            // определяем длину расписания в минутах
            double ScheduleLen = (maxTime - minTime).TotalMinutes;

            // определяем масштаб
            double PixelByMinute = (Width - strSize.Width - 3 * rowIndentHor) / ScheduleLen;

            // определяем позиции верхнего левого угла для координат
            AxesXZeroPoint = (int)Math.Round(strSize.Width + 2 * rowIndentHor);
            AxesYZeroPoint = (int)Math.Round(GraphBeginY + rowHeight);

            if (items != null)
                items.Clear();
            else
                items = new Rows();
            for (int i = 0; i < OriginalItems.Count; i++)
            {
                Row new_row = new Row(OriginalItems[i].Text);

                for (int j = 0; j < OriginalItems[i].Count; j++)
                {
                    DateTime t1 = OriginalItems[i][j].BeginDate;
                    DateTime t2 = OriginalItems[i][j].EndDate;
                    
                    // если время обработки пересекает линию начала
                    if (OriginalItems[i][j].BeginDate < minTime)
                        t1 = minTime;

                    // если время обработки пересекает линию начала
                    if (OriginalItems[i][j].EndDate > maxTime)
                        t2 = maxTime;

                    double MinuteBegin = (t1 - minTime).TotalMinutes;
                    double MinuteEnd = (t2 - minTime).TotalMinutes;

                    if (MinuteEnd - MinuteBegin > 0)
                    {
                        RowItem rowitem = new RowItem(OriginalItems[i][j].Text, OriginalItems[i][j].Index, OriginalItems[i][j].OrderIndex, t1, t2);

                        rowitem.RealBeginDate = OriginalItems[i][j].BeginDate;
                        rowitem.RealEndDate = OriginalItems[i][j].EndDate;
                        rowitem.IsAlert = OriginalItems[i][j].IsAlert;

                        rowitem.DrawRegion = new Rectangle(
                            (int)(AxesXZeroPoint + MinuteBegin * PixelByMinute),
                            (int)(AxesYZeroPoint + rowHeight * i + rowIndentVer),
                            (int)((MinuteEnd - MinuteBegin) * PixelByMinute),
                            (int)(rowHeight - 2 * rowIndentVer));
                        new_row.Add(rowitem);
                    }
                }

                items.Add(new_row);
            }

            if (RegionItem != null)
                SetSelected(RegionItem.Index);

            items.Gluing();

        }


        /// <summary>
        /// Возвращает самую длинное название для строки диаграммы гантта
        /// </summary>
        /// <returns>Строка.</returns>
        private string GetBiggerString(Rows rows)
        {
            string Result = "";
            foreach (Row item in rows)
            {
                if (item.Text.Length > Result.Length)
                    Result = item.Text;
            }
            return Result;
        }

        public void SetSelected(int Index)
        {
            if (Index == -1)
            {
                RegionItem = null;
                Refresh();                
                return;
            }
            foreach (Row row_item in items)
            {
                foreach (RowItem item in row_item)
                {
                    if (item.Index == Index)
                    {
                        RegionItem = item;
                        Refresh();
                        
                        if (SelectItem != null)
                        {
                            SelectItem(this, new GantChangedEvent(RegionItem));
                        }
                        return;
                    }
                }
            }
        }

        public void SetLightedOrderId(int OrderId)
        {
            LightedOrderId = OrderId;
            Refresh();
        }


        public void CreateSelectObjects(Color TaskColorFill, // Color.SkyBlue
                                        Color TaskColorPerimeter, // Color.Blue
                                        Color TaskColorFillAlert, // Color.Red
                                        Color OrderlightedColor, // Color.Coral
                                        Color ConfColor1, // Color.DarkGray
                                        Color ConfColor2, // Color.LightGray
                                        Color ServColor1, // Color.DarkGray
                                        Color ServColor2) // Color.LightGreen
        {
            selectBrush = new SolidBrush(TaskColorFill);
            selectAlertBrush = new SolidBrush(TaskColorFillAlert);
            selectOrderBrush = new SolidBrush(OrderlightedColor);
            selectBrushConf = new HatchBrush(HatchStyle.ForwardDiagonal, ConfColor1, ConfColor2);
            selectBrushServ = new HatchBrush(HatchStyle.ForwardDiagonal, ServColor1, ServColor2);
            selectPen = new Pen(TaskColorPerimeter);
            selectBrushText = new SolidBrush(TaskColorPerimeter);  
        }

        public void CreateShadowObjects(Color TaskColorFill,
                                        Color LightedOrder,
                                        Color ConfColor1,
                                        Color ConfColor2,
                                        Color ServColor1,
                                        Color ServColor2,
                                        Color AllPen) // Color.LightGreen
        {
            
            shadowBrush = new SolidBrush(TaskColorFill);
            shadowBrushOrder = new SolidBrush(LightedOrder);
            shadowBrushConf = new HatchBrush(HatchStyle.ForwardDiagonal, ConfColor1, ConfColor2);
            shadowBrushServ = new HatchBrush(HatchStyle.ForwardDiagonal, ServColor1, ServColor2);
            shadowPen = new Pen(AllPen, 1);
        }

        #endregion

        private void GraphField_Paint(object sender, PaintEventArgs e)
        {
            DrawGraph(e.Graphics);
        }

        private void сохранитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (items.Count != 0)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = "bmp";
                dlg.Filter = "Точечный рисунок (*.bmp)|*.bmp";
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height);
                    System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
                    gr.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), 0, 0, Width, Height);
                    DrawGraph(gr);
                    bmp.Save(dlg.FileName);
                }
            }
        }

        private void GraphField_MouseDown(object sender, MouseEventArgs e)
        {
            if (items == null)
                return;

            if (RegionItem != null)
            {
                if (e.X > RegionItem.DrawRegion.X &&
                    e.Y > RegionItem.DrawRegion.Y &&
                    e.X < RegionItem.DrawRegion.Right &&
                    e.Y < RegionItem.DrawRegion.Bottom)
                {
                    // если указатель находится в том же регионе, то не нужно ничего пересчитывать
                    return;
                }
            }

            foreach (Row row_items in items)
            {
                if (row_items.Count != 0) // находим строку в которой содержится искомый элемент
                {
                    if (e.Y > row_items[0].DrawRegion.Y &&
                        e.Y < row_items[0].DrawRegion.Bottom)
                    {
                        foreach (RowItem item in row_items)
                        {
                            if (e.X > item.DrawRegion.X &&
                                e.X < item.DrawRegion.Right)
                            {
                                RegionItem = item;
                                Refresh();
  

                                if (SelectItem != null)
                                    SelectItem(this, new GantChangedEvent(RegionItem));
                                return;
                            }
                        }
                    }
                }
            }

            // если дошли до этого места, то, очевидно, регион не выбран
            if (RegionItem != null)
            {
                RegionItem = null;
                Refresh();

                if (SelectLostItem != null)
                    SelectLostItem(this, new GantChangedEvent());
            }
        }

    }
}
