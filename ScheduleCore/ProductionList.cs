using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public class ProductionList : BaseScheduleList
    {
        /// <summary>
        /// Стандартная перегрузка индексатора.
        /// </summary>
        /// <param name="index">Индекс по которому необходимо получить значение.</param>
        /// <returns>Элемент, находящийся по заданному индексу.</returns>
        public ProductionItem this[int index]
        {
            get { return (ProductionItem)items[index]; }
        }


        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        public ProductionList()
        {
 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Item"></param>
        public void Add(ProductionItem Item)
        {
            foreach (ProductionItem item in items)
            {
                if (item.Text == Item.Text)
                    throw new Exception(Item.Text + ": Продукция с данным именем уже существует!");
                if (item.Id == Item.Id)
                    throw new Exception(Item.Text + ": Продукция с данным идентификатором уже существует!");
            }
            items.Add((ProductionItem)Item.Clone());
        }

        public new object Clone()
        {
            ProductionList NewList = new ProductionList();
            foreach (ProductionItem item in items)
            {
                NewList.Add((ProductionItem)item.Clone());
            }
            return NewList;
        }
    }
}
