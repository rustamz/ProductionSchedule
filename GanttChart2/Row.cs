using System;
using System.Collections.Generic;

namespace GantChart2
{
    public class RowItem
    {
        /// <summary>
        /// Тест элемента
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Индекс задания.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Индекс задания.
        /// </summary>
        public int OrderIndex
        {
            get;
            set;
        }

        public System.Drawing.Rectangle DrawRegion
        {
            get;
            set;
        }

        /// <summary>
        /// Начало периода для графика
        /// </summary>
        public DateTime BeginDate
        {
            get;
            set;
        }

        /// <summary>
        /// Конец периода для графика
        /// </summary>
        public DateTime EndDate
        {
            get;
            set;
        }

        /// <summary>
        /// Начало периода для пользователя
        /// </summary>
        public DateTime RealBeginDate
        {
            get;
            set;
        }

        /// <summary>
        /// Конец периода для пользователя
        /// </summary>
        public DateTime RealEndDate
        {
            get;
            set;
        }

        /// <summary>
        /// Окрашивает регион в кричащий цвет для привлечения внимания.
        /// </summary>
        public bool IsAlert
        {
            get;
            set; 
        }

        /// <summary>
        /// Следующие задание.
        /// </summary>
        public RowItem Next
        {
            get;
            set;
        }

        /// <summary>
        /// Предыдущее задание.
        /// </summary>
        public RowItem Prev
        {
            get;
            set;
        }

        public RowItem(string Text, int Index, int OrderIndex, DateTime BeginDate, DateTime EndDate)
        {
            this.Text = Text;
            this.BeginDate = BeginDate;
            this.EndDate = EndDate;
            this.Next = null;
            this.Prev = null;
            this.Index = Index;
            this.OrderIndex = OrderIndex;
        }

        public RowItem(string Text, int Index, int OrderIndex, DateTime BeginDate, DateTime EndDate, bool IsAlert)
        {
            this.Text = Text;
            this.BeginDate = BeginDate;
            this.EndDate = EndDate;
            this.Next = null;
            this.Prev = null;
            this.Index = Index;
            this.OrderIndex = OrderIndex;
            this.IsAlert = IsAlert;
        }

        public RowItem(string Text, int Index, int OrderIndex, DateTime BeginDate, DateTime EndDate, RowItem Next, RowItem Prev)
        {
            this.Text = Text;
            this.BeginDate = BeginDate;
            this.EndDate = EndDate;
            this.Next = Next;
            this.Prev = Prev;
            this.Index = Index;
            
        }
    }

    /// <summary>
    /// Класс строки
    /// </summary>
    public class Row : List<RowItem>
    {
        /// <summary>
        /// Название строки
        /// </summary>
        public string Text
        {
            get;
            set;
        }
        
        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        /// <param name="Text">Название строки.</param>
        public Row(string Text)
            : base()
        {
            this.Text = Text;
        }
    }

    /// <summary>
    /// Класс реализующий коллекцию элементов-строк для диаграммы Ганта.
    /// </summary>
    public class Rows : List<Row>
    {
        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        public Rows()
            : base()
        {
            //
        }

        /// <summary>
        /// устанавливает связи между заданиями с одинаковыми индексами.
        /// </summary>
        public void Gluing()
        {
            // лианеризируем коллекцию
            List<RowItem> AllItems = new List<RowItem>();
            for (int i = 0; i < Count; i++)
                AllItems.AddRange(this[i]);

            for (int i = 0, i_end = AllItems.Count - 1; i < i_end; i++)
            {
                for (int j = i + 1, j_end = AllItems.Count; j < j_end; j++)
                {
                    if (AllItems[i].Index == AllItems[j].Index && AllItems[i].Index >= 0) // если у элементов одинаковые индексы, то объединяем их
                    {
                        if (AllItems[i].BeginDate < AllItems[j].EndDate)
                        {
                            AllItems[i].Next = AllItems[j];
                            AllItems[j].Prev = AllItems[i];
                        }
                        else
                        {
                            AllItems[j].Next = AllItems[i];
                            AllItems[i].Prev = AllItems[j];
                        }
                    }
                }
            }
        }
    }
}
