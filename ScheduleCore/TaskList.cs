using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public class TaskList : BaseScheduleList, ICloneable
    {
        /// <summary>
        /// Стандартная перегрузка индексатора.
        /// </summary>
        /// <param name="index">Индекс по которому необходимо получить значение.</param>
        /// <returns>Элемент, находящийся по заданному индексу.</returns>
        public TaskItem this[int index]
        {
            get { return (TaskItem)items[index]; }
            set { items[index] = value; }
        }


        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        public TaskList()
        {
            //
        }

        /// <summary>
        /// Сортирует по убываню директивного срока.
        /// Задания в которых директивный срок не задан оказываются в конце.
        /// </summary>
        public void SortByDeadLineDawn()
        {
            for (int i = 0, i_end = Count - 1; i < i_end; i++)
            {
                if (!this[i].UseDeadLine)
                    continue;
                for (int j = i + 1, j_end = Count; j < j_end; j++)
                {
                    if (!this[j].UseDeadLine)
                        continue;
                    if (this[i].DeadLine < this[j].DeadLine)
                    {
                        TaskItem tmp = this[i];
                        this[i] = this[j];
                        this[j] = tmp;
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет, существуют ли задания из заданного материала.
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public bool TaskWithMaterialsExists(int MaterialId)
        {
            for (int i = 0, i_end = this.Count; i < i_end; i++)
            {
                if (this[i].MaterialId == MaterialId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Добавляет элемент в коллекцию
        /// </summary>
        /// <param name="Item"></param>
        public void Add(TaskItem Item)
        {
            items.Add((TaskItem)Item.Clone());
        }

        /// <summary>
        /// Добавляет копию элемента в коллекцию с сохранением текущего идентификатора.
        /// </summary>
        /// <param name="Item"></param>
        public void AddCopy(TaskItem Item)
        {
            items.Add(new TaskItem(Item.Id, Item.Text, Item.MaterialId, Item.ProductionId, Item.SizeIndex, Item.OrderId, Item.DeadLine, Item.UseDeadLine));
        }

        /// <summary>
        /// Возвращает самое длинное задание с заданным материалом.
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="Products"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public int GetBiggerTask(int MaterialId, MaterialList Materials, ProductionList Products, BaseDeviceType Device)
        {
            int TaskIndex = -1;
            double TaskTime = 0;
            for (int i = 0, i_end = this.Count; i < i_end; i++)
            {
                if (this[i].MaterialId == MaterialId)
                {
                    if (TaskIndex == -1)
                    {
                        TaskIndex = i;
                        TaskTime = (Device == BaseDeviceType.Saw ?
                            this[i].SawingTime(Materials, Products)
                            : this[i].PolishingTime(Materials, Products));
                    }
                    else
                    {
                        double NewTime = (Device == BaseDeviceType.Saw ?
                            this[i].SawingTime(Materials, Products)
                            : this[i].PolishingTime(Materials, Products));
                        if (NewTime > TaskTime)
                        {
                            TaskIndex = i;
                            TaskTime = NewTime;
                        }
                    }
                }
            }

            return TaskIndex != -1 ? this[TaskIndex].Id : -1; 
        }

        /// <summary>
        /// Возвращает самое короткое задание с заданным материалом.
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="Products"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public int GetSmallestTask(int MaterialId, MaterialList Materials, ProductionList Products, BaseDeviceType Device)
        {
            int TaskIndex = -1;
            double TaskTime = 0;
            for (int i = 0, i_end = this.Count; i < i_end; i++)
            {
                if (this[i].MaterialId == MaterialId)
                {
                    if (TaskIndex == -1)
                    {
                        TaskIndex = i;
                        TaskTime = (Device == BaseDeviceType.Saw ?
                            this[i].SawingTime(Materials, Products)
                            : this[i].PolishingTime(Materials, Products));
                    }
                    else
                    {
                        double NewTime = (Device == BaseDeviceType.Saw ?
                            this[i].SawingTime(Materials, Products)
                            : this[i].PolishingTime(Materials, Products));
                        if (NewTime < TaskTime)
                        {
                            TaskIndex = i;
                            TaskTime = NewTime;
                        }
                    }
                }
            }

            return TaskIndex != -1 ? this[TaskIndex].Id : -1;
        }

        /// <summary>
        /// Возвращает суммарную длительность заданий,
        /// состоящих из одного материала
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="Products"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public double TaskDurationByMaterial(int MaterialId, MaterialList Materials, ProductionList Products, BaseDeviceType Device)
        {
            double Result = 0;
            for (int i = 0, i_end = this.Count; i < i_end; i++)
            {
                if (this[i].MaterialId == MaterialId)
                {
                    Result += Device == BaseDeviceType.Saw ?
                        this[i].SawingTime(Materials, Products) :
                        this[i].PolishingTime(Materials, Products);
                }
            }
            return Result;
        }

        /// <summary>
        /// Возвращает сумму времени выполнения всех заданий, которые нужно выполнить до определенного срока
        /// </summary>
        /// <param name="DeadLine">Срок, до которого должно исполниться задание.</param>
        /// <param name="Materials">Список материалов.</param>
        /// <param name="Productions">Список продукции.</param>
        /// <returns></returns>
        public double GetTaskDurationByDeadLine(DateTime DeadLine, MaterialList Materials, ProductionList Productions)
        {
            double Result = 0;
            foreach (TaskItem item in items)
            {
                if (item.UseDeadLine)
                {
                    if (item.DeadLine <= DeadLine)
                    {
                        Result += item.SawingTime(Materials, Productions) + item.PolishingTime(Materials, Productions);
                    }
                }
            }
            return Result;
        }

        /// <summary>
        /// Возвращает количество всех заданий, которые нужно выполнить до определенного срока
        /// </summary>
        /// <param name="DeadLine">Крайний срок выполнения</param>
        /// <returns></returns>
        public int GetTaskCountByDeadLine(DateTime DeadLine)
        {
            int Result = 0;
            foreach (TaskItem item in items)
            {
                if (item.UseDeadLine)
                {
                    if (item.DeadLine <= DeadLine)
                    {
                        Result++;
                    }
                }
            }
            return Result;
        }

        /// <summary>
        /// Стандартное клонирование объекта
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            TaskList NewList = new TaskList();
            foreach (TaskItem item in items)
            {
                NewList.Add((TaskItem)item.Clone());
            }
            return NewList;
        }

    }
}
