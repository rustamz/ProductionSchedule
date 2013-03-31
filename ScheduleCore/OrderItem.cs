using System;

namespace ScheduleCore
{
    /// <summary>
    /// Представляет единицу для хранения заказа
    /// </summary>
    public class OrderItem : BaseScheduleItem
    {
        /// <summary>
        /// Дата оформления заказа.
        /// </summary>
        public DateTime Date
        {
            get;
            set;
        }

        /// <summary>
        /// Время к которому необходимо выполнить заказ
        /// </summary>
        public DateTime? DeadLine
        {
            get;
            set;
        }

        /// <summary>
        /// Идентификатор заказчика.
        /// </summary>
        public int CustomerId
        {
            get;
            set;
        }

        /// <summary>
        /// Время штрафа для заказа, в файле не схораняется
        /// </summary>
        public double FineTime
        {
            get;
            set;
        }

        /// <summary>
        /// Время начала исполнения заказа, в файле не схораняется 
        /// </summary>
        public double StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Время конца исполнения заказа, в файле не схораняется 
        /// </summary>
        public double EndTime
        {
            get;
            set;
        }

        public new string Text
        {
            get { return "Заказ № " + id.ToString(); }
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Text"></param>
        /// <param name="CustomerId"></param>
        /// <param name="Date"></param>
        public OrderItem(int Id, int CustomerId, DateTime Date, DateTime? DeadLine, double FineTime = 0, double StartTime = 0, double EndTime = 0)
            : base(Id)
        {
            this.CustomerId = CustomerId;
            this.Date = Date;
            this.DeadLine = DeadLine;
            this.FineTime = FineTime;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public new object Clone()
        {
            return new OrderItem(Id, CustomerId, Date, DeadLine, FineTime, StartTime, EndTime);
        }
    }
}
