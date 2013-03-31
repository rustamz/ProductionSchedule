using System;

namespace GantChart2
{
    public class GantChangedEvent : EventArgs
    {
        /// <summary>
        /// Выделенный в текущий момент элемент
        /// </summary>
        public RowItem SelectedItem
        {
            get;
            set;
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        public GantChangedEvent()
            : base()
        {
            //
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        /// <param name="SelectedItem">Выделенный элемент.</param>
        public GantChangedEvent(RowItem SelectedItem)
            : base()
        {
            this.SelectedItem = SelectedItem;
        }

    }
}
