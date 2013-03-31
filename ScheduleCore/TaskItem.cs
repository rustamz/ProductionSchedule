using System;

namespace ScheduleCore
{
    /// <summary>
    /// Класс предоставляет методы для работы с заданями в системе построения расписания
    /// </summary>
    public class TaskItem : BaseScheduleItem 
    {
        #region Скрытые поля

        /// <summary>
        /// Время в минутах к которому желательно закончить обработку.
        /// </summary>
        private DateTime deadLine;

        /// <summary>
        /// Идентификатор продукции.
        /// </summary>
        private int productionId;

        /// <summary>
        /// Индекс размера продукции.
        /// </summary>
        private int sizeIndex;

        /// <summary>
        /// Идентификатор материала.
        /// </summary>
        private int materialId;

        /// <summary>
        /// Идентификатор заказа.
        /// </summary>
        private int orderId;

        /// <summary>
        /// Флаг, указывающий на то, использовать ли крайний срок или нет.
        /// </summary>
        private bool useDeadLine;

        /// <summary>
        /// Врямя последего использования от старта составления расписания
        /// </summary>
        private double timeOfLastUsing;

        #endregion

        #region Свойства

        public new string Text
        {
            get { return "#" + id.ToString(); }
        }

        /// <summary>
        /// Возвращает или задаёт идентификатор продукции.
        /// </summary>
        public int ProductionId
        {
            get { return productionId; }
            set { productionId = value; }
        }

        /// <summary>
        /// Возвращает или задаёт индекс размера продукции.
        /// </summary>
        public int SizeIndex
        {
            get { return sizeIndex; }
            set { sizeIndex = value; }
        }

        /// <summary>
        /// Возвращает или задаёт индекс заказа.
        /// </summary>
        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        /// <summary>
        /// Возвращает или задаёт идентификатор материала продукции.
        /// </summary>
        public int MaterialId
        {
            get { return materialId; }
            set { materialId = value; }
        }

        /// <summary>
        /// Возвращает или задаёт время к которому желательно закончить обработку.
        /// </summary>
        public DateTime DeadLine 
        { 
            get { return deadLine; }
            set { deadLine = value; }
        }

        /// <summary>
        /// Возвращает или задаёт время к которому желательно закончить обработку.
        /// </summary>
        public bool UseDeadLine
        {
            get { return useDeadLine; }
            set { useDeadLine = value; }
        }

        /// <summary>
        /// Возвращает или задает время последнего использования для задания
        /// </summary>
        public double TimeOfLastUsing
        {
            get { return timeOfLastUsing; }
            set { timeOfLastUsing = value; }
        }

        #endregion

        /// <summary>
        /// Метод инициализирует объект типа "Task"
        /// </summary>
        public TaskItem(int Id, string Text, int MaterialId, int ProductionId, int SizeIndex, int OrderId)
            : base (Id, Text)
        { 
            materialId = MaterialId;
            productionId = ProductionId;
            sizeIndex = SizeIndex;
            orderId = OrderId;
            deadLine = DeadLine;
            useDeadLine = false;
        }

        /// <summary>
        /// Метод инициализирует объект типа "Task"
        /// </summary>
        public TaskItem(int Id, string Text, int MaterialId, int ProductionId, int SizeIndex, int OrderId, DateTime DeadLine)
            : base(Id, Text)
        {
            materialId = MaterialId;
            productionId = ProductionId;
            sizeIndex = SizeIndex;
            orderId = OrderId;
            deadLine = DeadLine;
            useDeadLine = true;
        }

        /// <summary>
        /// Метод инициализирует объект типа "Task"
        /// </summary>
        public TaskItem(string Text, int MaterialId, int ProductionId, int SizeIndex, int OrderId)
            : base(Text)
        {
            materialId = MaterialId;
            productionId = ProductionId;
            sizeIndex = SizeIndex;
            orderId = OrderId;
            deadLine = DeadLine;
            useDeadLine = false;
        }

        /// <summary>
        /// Метод инициализирует объект типа "Task"
        /// </summary>
        public TaskItem(string Text, int MaterialId, int ProductionId, int SizeIndex, int OrderId, DateTime DeadLine)
            : base(Text)
        {
            materialId = MaterialId;
            productionId = ProductionId;
            sizeIndex = SizeIndex;
            orderId = OrderId;
            deadLine = DeadLine;
            useDeadLine = true;
            timeOfLastUsing = 0;
        }

        /// <summary>
        /// Метод инициализирует объект типа "Task"
        /// </summary>
        public TaskItem(int Id, string Text, int MaterialId, int ProductionId, int SizeIndex, int OrderId, DateTime DeadLine, bool UseDeadLine)
            : base(Id, Text)
        {
            materialId = MaterialId;
            productionId = ProductionId;
            sizeIndex = SizeIndex;
            orderId = OrderId;
            deadLine = DeadLine;
            useDeadLine = UseDeadLine;
            timeOfLastUsing = 0;
        }

        /// <summary>
        /// Возвращает время распиливания для задания.
        /// </summary>
        /// <param name="Materials">Список исходных материалов.</param>
        /// <param name="Productions">Список исходной продукции.</param>
        /// <returns>Полное время распиливания для заказа.</returns>
        public double SawingTime(MaterialList Materials, ProductionList Productions)
        {
            int MaterialIndex = Materials.GetIndexById(materialId);
            if (MaterialIndex == -1)
                return MaterialIndex;

            int ProductionIndex = Productions.GetIndexById(productionId);
            if (ProductionIndex == -1)
                return ProductionIndex;

            int IndexOfSizeMas = Productions[ProductionIndex].SupSizes.IndexOf(sizeIndex);
            if (IndexOfSizeMas == -1)
                return IndexOfSizeMas;

            // определяем площадь элемента задания
            double S = Productions[ProductionIndex].SupSizes[IndexOfSizeMas].Length * Productions[ProductionIndex].SupSizes[IndexOfSizeMas].Width / 1e+6;

            return (S * Materials[MaterialIndex].SawingTime + 0.5); // 0,5 - для правильного округления до целых
        }

        /// <summary>
        /// Возвращает время шлифования для задания.
        /// </summary>
        /// <param name="Materials">Список исходных материалов.</param>
        /// <param name="Productions">Список исходной продукции.</param>
        /// <returns>Полное время шлифования для заказа.</returns>
        public double PolishingTime(MaterialList Materials, ProductionList Productions)
        {
            int MaterialIndex = Materials.GetIndexById(materialId);
            if (MaterialIndex == -1)
                return MaterialIndex;

            int ProductionIndex = Productions.GetIndexById(productionId);
            if (ProductionIndex == -1)
                return ProductionIndex;

            int IndexOfSizeMas = Productions[ProductionIndex].SupSizes.IndexOf(sizeIndex);
            if (IndexOfSizeMas == -1)
                return IndexOfSizeMas;

            // определяем площадь элемента задания
            double S = Productions[ProductionIndex].SupSizes[IndexOfSizeMas].Length * Productions[ProductionIndex].SupSizes[IndexOfSizeMas].Width / 1e+6;

            return (S * Materials[MaterialIndex].PolishingTime + 0.5); // 0,5 - для правильного округления до целых
        }

        public new object Clone()
        {
            return new TaskItem(id, text, materialId, productionId, sizeIndex, orderId, deadLine, useDeadLine) 
                { timeOfLastUsing = this.timeOfLastUsing};
        }
    }
}
