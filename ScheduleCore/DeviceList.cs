using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    /// <summary>
    /// Данный класс реализует контейнер для хранения устройств, выполяющихся в реальном времени.
    /// </summary>
    public class DeviceList : BaseScheduleList
    {

        #region Свойства

        public TimeDevice this[int index]
        {
            get { return (TimeDevice)items[index]; }
            set { items[index] = value; }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        public DeviceList()
        {
            //
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление элемента в коллекцию доступных устройств.
        /// При этом устройству назначается новый идентификатор.
        /// </summary>
        /// <param name="Item">Устройство.</param>
        public void Add(TimeDevice Item)
        {
            items.Add((TimeDevice)Item.Clone());
        }

        /// <summary>
        /// Получает идентификатор свободного прибора.
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public int GetIdOfFreeDevice(int MaterialId)
        {
            // получаем идентификаторы свободных в данный момент
            // и поддерживающих данный материал устройств.
            List <int> mass = new List<int>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!((TimeDevice)items[i]).IsBusy() &&
                    ((TimeDevice)items[i]).IsSupportMaterial(MaterialId))
                    mass.Add(i);
            }

            // теперь оставляем те устройства, которые настроены на данный
            // материал в данный момент
            List<int> bestDevices = new List<int>();
            for (int i = 0; i < mass.Count; i++)
            {
                if (((TimeDevice)items[mass[i]]).CurrentMaterialId == MaterialId)
                    bestDevices.Add(mass[i]);
            }

            // если найдены лучше подходящие устройства,
            // то переключаемся на улучшеный список
            if (bestDevices.Count != 0)
                mass = bestDevices;
            else
            {
                // если наиболее подходящих устройств нет, то
                // пытаемся найти хотя бы не настроенные
                for (int i = 0; i < mass.Count; i++)
                {
                    if (((TimeDevice)items[mass[i]]).CurrentMaterialId == -1)
                        bestDevices.Add(mass[i]);
                }

                // если нашлись ненастроенные устройства, то переключаемся на них
                if (bestDevices.Count != 0)
                    mass = bestDevices;
            }

            // если такие устройства найдены
            if (mass.Count != 0)
            {
                // если найдены свободные устройства, то выбираем среди них то,
                // которое простаевает дольше всех
                int MinLastUsedIndex = 0;
                double LastUsedMin = ((TimeDevice)items[mass[0]]).LastUsed;
                for (int i = 1; i < mass.Count; i++)
                    if (((TimeDevice)items[mass[i]]).LastUsed < LastUsedMin)
                    {
                        MinLastUsedIndex = i;
                        LastUsedMin = ((TimeDevice)items[mass[i]]).LastUsed;
                    }

                return items[mass[MinLastUsedIndex]].Id;
            }
            return -1;
        }

        /// <summary>
        /// Определяет, есть ли сободные устройства.
        /// </summary>
        /// <returns></returns>
        public bool FreeDeviceExist()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!((TimeDevice)items[i]).IsBusy())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Определяет, есть ли сободные ненастроенные устройства.
        /// </summary>
        /// <returns></returns>
        public bool FreeUnconfiguredDeviceExist()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!((TimeDevice)items[i]).IsBusy() && ((TimeDevice)items[i]).CurrentMaterialId == -1)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Определяет, есть ли сободные ненастроенные устройства
        /// способные принять хотя бы одно из заданий.
        /// </summary>
        /// <returns></returns>
        public bool FreeUnconfiguredDeviceExist(TaskList Tasks)
        {
            for (int i = 0; i < items.Count; i++)
            {
                // если пила не занята и не настроена
                if (!((TimeDevice)items[i]).IsBusy() && ((TimeDevice)items[i]).CurrentMaterialId == -1)
                {
                    for (int j = 0; j < Tasks.Count; j++)
                    {
                        if (((TimeDevice)items[i]).IsSupportMaterial(Tasks[j].MaterialId))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Получает идентификатор свободной ненастроенной пилы,
        /// поддерживающей данный материал.
        /// </summary>
        /// <returns></returns>
        public int GetUnconfiguredFreeDevice(int MaterialId)
        {
            for (int i = 0; i < items.Count; i++)
            {
                // если пила не занята и не настроена
                if (!((TimeDevice)items[i]).IsBusy() && ((TimeDevice)items[i]).CurrentMaterialId == -1)
                {
                    if (((TimeDevice)items[i]).IsSupportMaterial(MaterialId))
                    {
                        return items[i].Id;
                    }
                }
            }
            return -1;
        }


        /// <summary>
        /// Определяет есть ли среди приборов хотя бы
        /// одно работающее в данный момент устройство.
        /// </summary>
        /// <returns></returns>
        public bool OneWorkDeviceExist()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (((TimeDevice)items[i]).IsBusy())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Возвращает минимальное время оставщееся до конца операции.
        /// </summary>
        /// <returns>Время в минутах</returns>
        public double GetMinTime()
        {
            double Result = 0;
            bool ResultSet = false;
            
            for (int i = 0; i < items.Count; i++)
            {
                if (((TimeDevice)items[i]).CurrentTaskId != -1)
                {
                    if (!ResultSet)
                    {
                        Result = ((TimeDevice)items[i]).Time;
                        ResultSet = true;
                        continue;
                    }
                    if (((TimeDevice)items[i]).Time < Result)
                        Result = ((TimeDevice)items[i]).Time;
                }

            }
            return Result;
        }

        /// <summary>
        /// Очищает списки выполненных операций для всех устройтсв.
        /// </summary>
        public void ClearCompleteTasks()
        {
            for (int i = 0; i < items.Count; i++)
            {
                TimeDevice Item = (TimeDevice)items[i];
                Item.ClearCompleteTasks();
            }
        }

        /// <summary>
        /// Возвращает коллекцию свободных в данный момент пил поддерживающих
        /// заданный материал.
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public DeviceList GetFreeDeviceByMatType(int MaterialId)
        {
            DeviceList ResultList = new DeviceList();
            for (int i = 0; i < Count; i++)
            {
                if (this[i].IsSupportMaterial(MaterialId))
                {
                    if (!this[i].IsBusy())
                        ResultList.Add(this[i]);
                }   
            }

            return ResultList;
        }

        /*
        /// <summary>
        /// Возвращает идентификатор устройства, настроенного на данный материал.
        /// </summary>
        /// <returns>Идентификатор устройства или -1</returns>
        public int GetDeviceByMaterial(int MaterialId)
        {
            int ResultId = -1;
            for (int i = 0; i < Count; i++)
            {
                if (this[i].CurrentMaterialId == MaterialId)
                {
                    ResultId = this[i].Id;
                    break;
                }
            }
            return ResultId;
        }

        /// <summary>
        /// Возвращает идентификатор устройства, настроенного на данный материал.
        /// </summary>
        /// <returns>Идентификатор устройства или -1</returns>
        public DeviceList GetFreeDeviceByMatTypeAndAfterTime(int MaterialId, int Time)
        {
            DeviceList ResultList = new DeviceList();
            
            for (int i = 0; i < Count; i++)
            {
                if (this[i].IsSupportMaterial(MaterialId))
                {
                    if (this[i].Time < Time)
                    {
                        ResultList.Add(this[i]);
                    }
                }
            }

            return ResultList;
        }
        */

        /// <summary>
        /// Возвращает идентификатор устройства, которое свободно в данный момент,
        /// настроенного на данный материал или настроится на материал за минимальное время
        /// </summary>
        /// <returns>Идентификатор устройства или -1</returns>
        public int GetDeviceByMaterial(int MaterialId)
        {
            int ResultId = -1;
            for (int i = 0; i < Count; i++)
            {
                if (!this[i].IsBusy())
                {
                    if (this[i].CurrentMaterialId == MaterialId)
                    {
                        ResultId = this[i].Id;
                        break;
                    }
                }
            }

            // если нет устройства настроенного в данный момент
            if (ResultId == -1)
            {
                // получаем устройство которое сконфигурируется быстрее всего
                double MinTime = -1;
                for (int i = 0; i < Count; i++)
                {
                    for (int j = 0; j < this[i].SupportedMaterials.Count; j++)
                    {
                        if (this[i].SupportedMaterials[j].ID == MaterialId)
                        {
                            if (ResultId == -1)
                            {
                                ResultId = this[i].Id;
                                MinTime = this[i].SupportedMaterials[j].Time;
                            }
                            else
                            {
                                if (MinTime > this[i].SupportedMaterials[j].Time)
                                {
                                    ResultId = this[i].Id;
                                    MinTime = this[i].SupportedMaterials[j].Time;
                                }
                            }

                        }
                    }
                }


            }

            return ResultId;
        }

        /// <summary>
        /// Возвращает устройство - альтернативу DeviceId, то есть такое устройство
        /// которое способно обработать MaterialId и которое освободится в ближайшее время или уже свободное. 
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="DeviceId"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public int GetBestAlternative(int MaterialId, int DeviceId, BaseDeviceType Device)
        {
            int BestIndex = -1;
            for (int i = 0, i_end = this.Count; i < i_end; i++)
            {
                if (this[i].Id != DeviceId)
                {
                    if (this[i].IsSupportMaterial(MaterialId))
                    {
                        if (BestIndex == -1)
                        {
                            BestIndex = i;
                        }
                        else
                        {
                            if (this[i].Time < this[BestIndex].Time)
                            {
                                BestIndex = i;
                            }
                        }
                    }
                }
            }
            return this[BestIndex].Id;
        }

        /// <summary>
        /// Возвращает самое простаивающее устройство с учётом
        /// материала, который содержится в Tasks
        /// </summary>
        /// <param name="Tasks"></param>
        /// <returns></returns>
        public int GetNotBusyDeviceId(TaskList Tasks)
        {
            int BestIndex = -1;
            for (int i = 0, i_end = this.Count; i < i_end; i++)
            {
                if (!this[i].IsBusy())
                {
                    if (BestIndex == -1)
                    {
                        for (int j = 0; j < Tasks.Count; j++)
                        {
                            if (this[i].IsSupportMaterial(Tasks[j].MaterialId))
                            {
                                // если это устройство свободно и поддерживает хотя бы один материал из списка
                                BestIndex = i;
                                break;
                            }
                        }
                            
                    }
                    else
                    {
                        if (this[i].LastUsed < this[BestIndex].LastUsed)
                        {
                            for (int j = 0; j < Tasks.Count; j++)
                            {
                                if (this[i].IsSupportMaterial(Tasks[j].MaterialId))
                                {
                                    // если это устройство свободно и поддерживает хотя бы один материал из списка
                                    BestIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return BestIndex != -1 ? this[BestIndex].Id : -1;
        }

        /// <summary>
        /// Возвращает среднее арифметическое время настройки всех устройств на заданный материал
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public double GetMidConfigTime(int MaterialId)
        {
            int SupportedDeviceCount = 0;
            double FullTime = 0;
            for (int i = 0; i < Count; i++)
            {
                double ConfTime = this[i].GetConfigTime(MaterialId);
                if (ConfTime != -1)
                {
                    FullTime += ConfTime;
                    SupportedDeviceCount++;
                }
            }

            return SupportedDeviceCount != 0 ? FullTime / SupportedDeviceCount : 0;
        }

        /// <summary>
        /// Возвращает среднее арифметическое время настройки всех устройств на заданный список материалов
        /// </summary>
        /// <param name="Materials"></param>
        /// <returns></returns>
        public double GetMidConfigTime(MaterialList Materials)
        {
            double SumTime = 0;
            for (int i = 0; i < Materials.Count; i++)
            {
                SumTime += GetMidConfigTime(Materials[i].Id);
            }
            return Materials.Count != 0 ? SumTime / Materials.Count : 0;
        }

        /// <summary>
        /// Возвращает идентификатор устройства, настроенного на данный материал.
        /// </summary>
        /// <returns>Идентификатор устройства или -1</returns>
        public DeviceList GetFreeDeviceByMatTypeAndAfterTime(int MaterialId, double Time)
        {
            DeviceList ResultList = new DeviceList();

            for (int i = 0; i < Count; i++)
            {
                if (this[i].IsSupportMaterial(MaterialId))
                {
                    if (this[i].Time < Time)
                    {
                        ResultList.Add(this[i]);
                    }
                }
            }

            return ResultList;
        }

        public int IndexByTaskId(int TaskId)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (((TimeDevice)items[i]).IndexByTaskId(TaskId) > -1)
                    return i;
            }
            return -1;
        }

        public void Service(double CurrentTime)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].TimeToService())
                    this[i].Service(CurrentTime);
        }

        public new object Clone()
        {
            DeviceList NewList = new DeviceList();
            foreach (TimeDevice item in items)
            {
                NewList.Add((TimeDevice)item.Clone());
            }
            return NewList;
        }

        #endregion

    }
}
