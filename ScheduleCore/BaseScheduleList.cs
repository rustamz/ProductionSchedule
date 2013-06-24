using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public class BaseScheduleList : ICloneable
    {
        /// <summary>
        /// Список элементов
        /// </summary>
        protected List<IBaseScheduleItem> items = new List<IBaseScheduleItem>();

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        public BaseScheduleList()
        {
            //
        }

        /// <summary>
        /// Очищает список.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// Возвращает кол-во элементов в списке.
        /// </summary>
        /// <return>Количество элементов в списке.</return>
        public int Count
        {
            get { return items.Count; }
        }


        /// <summary>
        /// Возвращает индекс элемента по идентификатору.
        /// </summary>
        /// <param name="Id">Идентификатор элемента.</param>
        /// <returns>Индес элемента в коллекции, если элемент с данным идентификатором существует, иначе -1.</returns>
        public int GetIndexById(int Id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == Id)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Возвращает индекс элемента по связанному с ним тексту.
        /// </summary>
        /// <param name="Text">Связанный с элементом текст.</param>
        /// <returns>Индес первого элемента в коллекции с данным текстом, если элемент с данным текстом существует, иначе -1.</returns>
        public int GetIndexByText(string Text)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Text == Text)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Возвращает идентификатор элемента по связанному с ним тексту.
        /// </summary>
        /// <param name="Text">Связанный с элементом текст.</param>
        /// <returns>Идентификатор первого элемента в коллекции с данным текстом, если элемент с данным текстом существует, иначе -1.</returns>
        public int GetIdByText(string Text)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Text == Text)
                    return items[i].Id;
            }
            return -1;
        }

        /// <summary>
        /// Возвращает тест элемента по идентификатору.
        /// </summary>
        /// <param name="Id">Идентификатор элемента.</param>
        /// <returns>Текст элемента в коллекции с данным идентификатором, если элемент с данным идентификаторов существует, иначе пустая строка.</returns>
        public string GetTextById(int Id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == Id)
                    return items[i].Text;
            }
            return "";
        }
        
        /// <summary>
        /// Возвращает ссылку на элемент по идентификатору
        /// </summary>
        /// <param name="Id">Идентификатор элемента.</param>
        /// <returns>Ссылка на идентификатор, либо null.</returns>
        public IBaseScheduleItem GetItemById(int Id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == Id)
                    return items[i];
            }
            return null ; 
        }

        /// <summary>
        /// Удаляет элемент по идентификатору.
        /// </summary>
        /// <param name="Id"></param>
        public void DeleteById(int Id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == Id)
                {
                    items.RemoveAt(i);
                    break;
                }
            }            
        }

        /// <summary>
        /// Удаляет элемент.
        /// </summary>
        /// <param name="Id"></param>
        public void Delete(int Index)
        {
            items.RemoveAt(Index);
        }

        public object Clone()
        {
            BaseScheduleList clone = new BaseScheduleList();
            foreach (IBaseScheduleItem item in items)
            {
                clone.items.Add((IBaseScheduleItem)item.Clone());
            }
            return clone;
        }

        /// <summary>
        /// Получает минимальный свободный Id
        /// </summary>
        /// <returns></returns>
        public int GetFreeId()
        {
            int MinimalId = 0;
            while (true)
            {
                bool IdExist = false;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Id == MinimalId)
                    {
                        IdExist = true;
                        break;
                    }
                }

                if (IdExist)
                    MinimalId++;
                else break;
            }

            return MinimalId;
        }
    }
}
