using System;

namespace ScheduleCore
{
    /// <summary>
    /// Класс реализует список для хранения типов материала.
    /// В качестве хранимых параметров используется только имя материала.
    /// Так же для быстрой идентификации каждому материалу присваивается уникальный номер - целое число большее 0.
    /// </summary>
    public class MaterialList : BaseScheduleList
    {
        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        public MaterialList()
            : base ()
        {
 
        }

        /// <summary>
        /// Метод добавляет материал в список. Если материал с таким же именем уже существует в коллекции, то порождается исключение.
        /// </summary>
        /// <param name="Item">Материал.</param>
        public void Add(MaterialItem Item)
        {
            foreach (MaterialItem item in items)
            {
                if (item.Text == Item.Text)
                    throw new Exception("\"" + Item.Text + "\": Имя материала должно быть уникальным!");
                if (item.Id == Item.Id)
                    throw new Exception("\"" + Item.Text + "\": Идентификатор материала должнен быть уникальным!");
            }
            items.Add((MaterialItem)Item.Clone());
        }

        /// <summary>
        /// Стандартная перегрузка индексатора.
        /// </summary>
        /// <param name="index">Индекс по которому необходимо получить значение.</param>
        /// <returns>Элемент, находящийся по заданному индексу.</returns>
        public MaterialItem this[int index]
        {
            get { return (MaterialItem)items[index]; }
        }

        public new object Clone()
        {
            MaterialList NewList = new MaterialList();
            foreach (MaterialItem item in items)
            {
                NewList.Add((MaterialItem)item.Clone());
            }
            return NewList;
        }
    }
}
