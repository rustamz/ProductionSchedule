using System;
using System.Collections.Generic;

namespace ScheduleCore
{
    /// <summary>
    /// Класс для хранения допустимых размеров продукции.
    /// </summary>
    public class ProductionSize : ICloneable
    {
        /// <summary>
        /// Возвращает или задаёт индекс размера, которые служит идентификатором
        /// </summary>
        public int Index
        {
            get;
            set;
        }
        
        /// <summary>
        /// Возвращает или задаёт длину единицы продукции в миллиметрах.
        /// </summary>
        public double Length
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает или задаёт ширину единицы продукции в миллиметрах.
        /// </summary>
        public double Width
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает или задаёт высоту единицы продукции в миллиметрах.
        /// </summary>
        public double Height
        {
            get;
            set;
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        public ProductionSize()
        {
            Length = 0;
            Width = 0;
            Height = 0;
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Length">Длина продукции.</param>
        /// <param name="Width">Ширина продукции.</param>
        /// <param name="Height">Высота продукции.</param>
        public ProductionSize(int Index, double Length, double Width, double Height)
        {
            this.Index = Index;
            this.Length = Length;
            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Создает копию объекта.
        /// </summary>
        /// <returns>Копия объекта.</returns>
        public object Clone()
        {
            return new ProductionSize(Index, Length, Width, Height);
        }

        public override string ToString()
        {
            return Length.ToString() + " × " + Width.ToString() + " × " + Height.ToString();
        }
    }

    public class ProductionSizeList : List<ProductionSize>
    {
        public ProductionSizeList()
            : base()
        {
 
        }

        /// <summary>
        /// Добавляет элемент в коллекцию. Если коллекция уже содержит
        /// элемент с аналогичными характеристиками, то элемент не добавляется.
        /// </summary>
        /// <param name="item"></param>
        public new void Add(ProductionSize item)
        {
            // проверка уникальности индекса
            foreach (ProductionSize Item in this)
            {
                if (Item.Index == item.Index)
                    return;
            }

            bool IsUnique = true;
            foreach (ProductionSize Item in this)
            {
                if (Item.Length == item.Length)
                    if (Item.Width == item.Width)
                        if (Item.Height == item.Height)
                        {
                            IsUnique = false;
                            break;
                        }
            }

            if (IsUnique)
            {
                base.Add(item);
            }
        }

        /// <summary>
        /// Возвращает индекс элемента массива продукции по размерному индексу.
        /// </summary>
        /// <param name="SizeIndex"></param>
        /// <returns></returns>
        public int IndexOf(int SizeIndex)
        {
            int Result = -1;
            for (int i = 0, i_end = this.Count; i < i_end; i++)
                if (this[i].Index == SizeIndex)
                    return i;
            return Result;
        }

        public int GetFreeIndex()
        {
            int MinimalIndex = 0;
            while (true)
            {
                bool IdExist = false;
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Index == MinimalIndex)
                    {
                        IdExist = true;
                        break;
                    }
                }

                if (IdExist)
                    MinimalIndex++;
                else break;
            }

            return MinimalIndex;
        }
    }

    /// <summary>
    /// Класс предоставляющий методы для работы с единицами продукции
    /// </summary>
    public class ProductionItem : BaseScheduleItem
    {
        private ProductionSizeList supSizes = new ProductionSizeList();

        /// <summary>
        /// Возвращает или задает допусимые размеры для продукции
        /// </summary>
        public ProductionSizeList SupSizes
        {
            get { return supSizes; }
            set { supSizes = value; }
        }

        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        /// <param name="Id">Идентификатор продукции.</param>
        /// <param name="Text">Имя продукции.</param>
        public ProductionItem(int Id, string Text)
            : base(Id, Text)
        {

        }

        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        /// <param name="Id">Идентификатор продукции.</param>
        /// <param name="Text">Имя продукции.</param>
        public ProductionItem(int Id, string Text, ProductionSizeList psl)
            : base(Id, Text)
        {
            supSizes.Clear();
            foreach (ProductionSize item in psl)
            {
                supSizes.Add((ProductionSize)item.Clone());
            }
        }

        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        /// <param name="Text">Имя продукции.</param>
        public ProductionItem(string Text)
            : base(Text)
        {

        }

        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        /// <param name="Text">Имя продукции.</param>
        public ProductionItem(string Text, ProductionSizeList psl)
            : base(Text)
        {
            supSizes.Clear();
            foreach (ProductionSize item in psl)
            {
                supSizes.Add((ProductionSize)item.Clone());
            }
        }

        /// <summary>
        /// Создаёт и возвращает копию объекта.
        /// </summary>
        /// <returns>Копия объекта.</returns>
        public new object Clone()
        {
            return new ProductionItem(Id, Text, supSizes);
        }

        public override string ToString()
        {
            return text;
        }
    }
}
