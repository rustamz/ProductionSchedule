using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace ScheduleCore
{
    public class ScheduleManager
    {
        /// <summary>
        /// Содержит параметры расписания.
        /// </summary>
        private Configuration data = new Configuration();

        /// <summary>
        /// Возвращает или задаёт параметры расписания.
        /// </summary>
        public Configuration Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        public ScheduleManager()
        {
 
        }

        /// <summary>
        /// Возвращает индекс задания в коллекции с наибольшим временем распиливания.
        /// </summary>
        /// <param name="list">Коллекция заданий в которой производится поиск.</param>
        /// <returns>Индекс задания.</returns>
        protected int GetTaskIndexWithMostLongSawing(TaskList list)
        {
            if (list.Count == 0)
                return -1;

            int Result = 0;
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].SawingTime(data.Materials, data.Productions) >
                    list[Result].SawingTime(data.Materials, data.Productions))
                    Result = 0;
            }

            return Result;
        }

        /// <summary>
        /// Получает временной шаг как минимальное время, оставшееся до конца операции из всех станков.
        /// </summary>
        /// <param name="SawtList">Список пил.</param>
        /// <param name="GrinderList">Список шлифвальных станков.</param>
        /// <returns></returns>
        private double GetStep(DeviceList SawtList, DeviceList GrinderList)
        {
            double MinSawTime = data.Saws.GetMinTime();
            double MinGrindTime = data.Grinders.GetMinTime();
            double Step = MinSawTime < MinGrindTime ? MinSawTime : MinGrindTime;
            return Step == 0 ? MinSawTime + MinGrindTime : Step; 
        }

        /// <summary>
        /// Рассчитать расписание.
        /// </summary>
        public void StartSchedule()
        {
            // Сбрасываем предыдущее расписание
            data.Saws.ClearCompleteTasks();
            data.Grinders.ClearCompleteTasks();
            // устанавливаем текущее время
            double T = 0;
            // формируем список исходных заданий для распиливания
            TaskList DefaultSawingList = new TaskList();
            for (int i = 0; i < data.Tasks.Count; i++)
            {
                if (data.Tasks[i].SawingTime(data.Materials, data.Productions) != 0)
                {
                    DefaultSawingList.AddCopy((TaskItem)data.Tasks[i].Clone());
                }
            }

            // формируем список исходных заданий для шлифования из заданий не требующих распиливания
            TaskList DefaultPolishingList = new TaskList();
            for (int i = 0; i < data.Tasks.Count; i++)
            {
                if (data.Tasks[i].SawingTime(data.Materials, data.Productions) == 0 &&
                    data.Tasks[i].PolishingTime(data.Materials, data.Productions) != 0)
                {
                    DefaultPolishingList.AddCopy((TaskItem)data.Tasks[i].Clone());
                }
            }

            //
            // первая настройка пил
            //

            // сортируем пилы по возрастанию кол-ва поддерживаемых материалов
            for (int i = 0, i_end = data.Saws.Count - 1; i < i_end; i++)
            {
                for (int j = i + 1, j_end = data.Saws.Count; j < j_end; j++)
                {
                    if (data.Saws[i].SupportedMaterials.Count > data.Saws[j].SupportedMaterials.Count)
                    {
                        TimeDevice tmp = data.Saws[i];
                        data.Saws[i] = data.Saws[j];
                        data.Saws[j] = tmp;
                    }
                }
            }

            bool BigTaskFirst = data.Saws.Count < data.Grinders.Count;

            do
            {
                data.Saws.Service(T);
                // назначаем каждому устройству задание
                for (int i = 0, i_end = data.Saws.Count; i < i_end; i++)
                {
                    if (!data.Saws[i].IsBusy()) // если устройство не занято
                    {
                        int TaskId = data.Saws[i].MostImpTask(DefaultSawingList, data.Materials, data.Productions, BaseDeviceType.Saw, BigTaskFirst);

                        if (TaskId != -1)
                        {
                            int TaskIndex = DefaultSawingList.GetIndexById(TaskId);
                            int ProdIndex = data.Productions.GetIndexById(DefaultSawingList[TaskIndex].ProductionId);
                            data.Saws[i].AddTask(TaskId, DefaultSawingList[TaskIndex].MaterialId, T, DefaultSawingList[TaskIndex].SawingTime(data.Materials, data.Productions));
                            DefaultSawingList.DeleteById(TaskId);
                        }
                    }
                }

                data.Grinders.Service(T);
                // назначаем каждому станку
                for (int i = 0, i_end = data.Grinders.Count; i < i_end; i++)
                {
                    if (!data.Grinders[i].IsBusy()) // если устройство не занято
                    {
                        // получаем идентификатор задания с учётом того как настроено устройство
                        int FirstTaskId = data.Grinders[i].MostImpTask(DefaultPolishingList, data.Materials, data.Productions, BaseDeviceType.Grinder, BigTaskFirst);

                        // альтернативное задание
                        int SecondTaskId = data.Grinders[i].ImpTask(DefaultPolishingList, data.Materials, data.Productions, BaseDeviceType.Grinder);

                        if (FirstTaskId != SecondTaskId && SecondTaskId != -1)
                        {
                            int FirstTaskIndex = DefaultPolishingList.GetIndexById(FirstTaskId);
                            int FirstMaterialId = DefaultPolishingList[FirstTaskIndex].MaterialId;

                            // есть ли устройство - альтернатива текущему?
                            int AlternateDeviceId = data.Grinders.GetBestAlternative(FirstMaterialId, data.Grinders[i].Id, BaseDeviceType.Grinder);
                            if (AlternateDeviceId != -1) // альтернатива существует
                            {
                                int AlternateDeviceIndex = data.Grinders.GetIndexById(AlternateDeviceId);

                                int SecondTaskIndex = DefaultPolishingList.GetIndexById(SecondTaskId);
                                int SecondMaterialId = DefaultPolishingList[SecondTaskIndex].MaterialId;

                                int FirstProductIndex = data.Productions.GetIndexById(DefaultPolishingList[SecondTaskIndex].ProductionId);
                                
                                // время обработки первого задания + конфигурация первого станка под второй материал
                                double FirstTime = DefaultPolishingList[FirstTaskIndex].PolishingTime(data.Materials, data.Productions) + data.Grinders[i].GetConfigTime(SecondMaterialId);

                                // время до конца отработки альтернативного устройства + время обработки первого задания + конфигурация второго станка под второй материал
                                double SecondTime = data.Grinders[AlternateDeviceIndex].Time + DefaultPolishingList.TaskDurationByMaterial(FirstMaterialId, data.Materials, data.Productions, BaseDeviceType.Grinder);
                                if (data.Grinders[AlternateDeviceIndex].CurrentMaterialId != FirstMaterialId) 
                                    SecondTime += data.Grinders[AlternateDeviceIndex].GetConfigTime(FirstMaterialId);

                                if (SecondTime <= FirstTime)
                                {
                                    FirstTaskId = SecondTaskId;
                                }
                            }
                        }

                        if (FirstTaskId != -1)
                        {
                            int TaskIndex = DefaultPolishingList.GetIndexById(FirstTaskId);
                            int ProdIndex = data.Productions.GetIndexById(DefaultPolishingList[TaskIndex].ProductionId);
                            data.Grinders[i].AddTask(FirstTaskId, DefaultPolishingList[TaskIndex].MaterialId, T, DefaultPolishingList[TaskIndex].PolishingTime(data.Materials, data.Productions));
                            DefaultPolishingList.DeleteById(FirstTaskId);
                        }
                    }
                }  

                //Определяем шаг по времени
                double Step = GetStep(data.Saws, data.Grinders);


                // совершаем временной шаг для пил
                for (int i = 0; i < data.Saws.Count; i++)
                {
                    int CompleteTaskId = data.Saws[i].MakeStep(Step);

                    // если завершилось распиливание
                    if (CompleteTaskId >= 0)
                    {
                        // получаем задание, которое прошло распиливание
                        int TaskIndex = data.Tasks.GetIndexById(CompleteTaskId);

                        // если завершенное задание требует шлифования, отправляем его в очередь на шлифовку
                        if (data.Tasks[TaskIndex].PolishingTime(data.Materials, data.Productions) != 0)
                        {
                            TaskItem TaskCopy = (TaskItem)data.Tasks[TaskIndex].Clone();
                            TaskCopy.TimeOfLastUsing = T;
                            DefaultPolishingList.AddCopy(TaskCopy);
                        }
                    }
                }

                // совершаем временной шаг шлифовщиков
                for (int i = 0; i < data.Grinders.Count; i++)
                {
                    int CompleteTaskId = data.Grinders[i].MakeStep(Step);
                }

                // изменяем текущее время
                T += Step;
            } while (data.Saws.OneWorkDeviceExist() || data.Grinders.OneWorkDeviceExist() ||
                    DefaultSawingList.Count != 0 || DefaultPolishingList.Count != 0);
        }

        /// <summary>
        /// Возвращает наиболее просроченное задание
        /// </summary>
        /// <param name="DefaultList">Список заданий.</param>
        /// <param name="CurrentTime">Текущее время.</param>
        /// <returns>Идентификатор просроченного задания или -1, если такого задания нет.</returns>
        private int GetOverdueTask(TaskList DefaultList, double CurrentTime)
        {
            DateTime dtCurrentTime = data.BaseTime.AddMinutes(CurrentTime);
            int ResultIndex = -1;
            DateTime MostOverdueDate = new DateTime();
            for (int i = 0; i < DefaultList.Count; i++)
            {
                if (DefaultList[i].UseDeadLine)
                {
                    if (DefaultList[i].DeadLine < dtCurrentTime) // если задание просрочено
                    {
                        if (ResultIndex == -1)
                        {
                            MostOverdueDate = DefaultList[i].DeadLine;
                            ResultIndex = i;
                        }
                        else
                        {
                            if (MostOverdueDate > DefaultList[i].DeadLine)
                            {
                                ResultIndex = i;
                            }
                        }
                    }
                }
            }
            return ResultIndex != -1 ? DefaultList[ResultIndex].Id : -1;
        }

        /// <summary>
        /// Метод возвращает идентификатор материала которого больше всего
        /// по времени обработки на пиле.
        /// </summary>
        /// <param name="DefaultSawingList"></param>
        private int MostImportantMaterial(TaskList Tasks, MaterialList Materials, 
            ProductionList Productions, BaseDeviceType DeviceType)
        {
            // список в котором хранится материал и суммарное время всех заданий
            // под данный материал
            List<MaterialPair> material = new List<MaterialPair>();

            // перебираем задания и наполняем список времен
            for (int i = 0; i < Tasks.Count; i++)
            {
                int CurrentMaterialID = Tasks[i].MaterialId;
                int FindedMaterialIndex = -1;
                for (int j = 0; j < material.Count; j++)
                {
                    if (material[j].ID == CurrentMaterialID)
                    {
                        FindedMaterialIndex = j;
                        break;
                    }
                }

                // если материал уже есть в списке
                if (FindedMaterialIndex > -1)
                    material[FindedMaterialIndex].Time += DeviceType == BaseDeviceType.Saw ?
                        Tasks[i].SawingTime(Materials, Productions) : Tasks[i].PolishingTime(Materials, Productions);
                else // если материала нет в списке
                    material.Add(new MaterialPair(CurrentMaterialID,
                       DeviceType == BaseDeviceType.Saw ? Tasks[i].SawingTime(Materials, Productions) :
                       Tasks[i].PolishingTime(Materials, Productions)));

            }

            // перебираем задания и наполняем список времен
            int Result = -1;
            double CurrentMaxTime = 0;
            for (int i = 0; i < material.Count; i++)
            {
                if (material[i].Time > CurrentMaxTime)
                {
                    CurrentMaxTime = material[i].Time;
                    Result = i;
                }
            }

            return Result != -1 ? material[Result].ID : -1;
        }


        /// <summary>
        /// Возвращает идентификатор наиболее важного задания.
        /// Важность зависит от времени выполнения.
        /// Чем дольше задание выполняется, тем оно важнее.
        /// </summary>
        /// <param name="DefaultList"></param>
        /// <param name="MaterialID"></param>
        /// <returns></returns>
        private int MostImportantTask(TaskList Tasks, MaterialList Materials, 
            ProductionList Productions, int MaterialID, BaseDeviceType DeviceType)
        {
            int ResultId = -1;
            if (Tasks.Count == 0)
                return ResultId;

            // находим индекс первого задания данного типа
            int ResultIndex = -1;
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].MaterialId == MaterialID)
                {
                    ResultIndex = i;
                    break;
                }
            }

            // если заданий под нужный материал вовсе нет, то до свидания
            if (ResultIndex == -1)
                return ResultId;

            ResultId = Tasks[ResultIndex].Id;

            for (int i = ResultIndex + 1; i < Tasks.Count; i++)
            {
                if (Tasks[i].MaterialId != MaterialID)
                    continue;

                if ((DeviceType == BaseDeviceType.Saw ? Tasks[i].SawingTime(Materials, Productions) : Tasks[i].PolishingTime(Materials, Productions)) >
                    (DeviceType == BaseDeviceType.Saw ? Tasks[ResultIndex].SawingTime(Materials, Productions) : Tasks[ResultIndex].PolishingTime(Materials, Productions)))
                {
                    ResultId = Tasks[i].Id;
                    ResultIndex = i;
                }
            }
            return ResultId;
        }

        /// <summary>
        /// Рассчитать расписание.
        /// </summary>
        public void StartSchedule2()
        {
            // устанавливаем в каждое задание директивный срок
            SetDirectiveFromOrder(data.Orders, data.Tasks);
            // Сбрасываем предыдущее расписание
            data.Saws.ClearCompleteTasks();
            data.Grinders.ClearCompleteTasks();
            // устанавливаем текущее время
            double T = 0;
            // формируем список исходных заданий для распиливания
            TaskList DefaultSawingList = new TaskList();
            for (int i = 0; i < data.Tasks.Count; i++)
            {
                if (data.Tasks[i].SawingTime(data.Materials, data.Productions) != 0)
                {
                    DefaultSawingList.AddCopy((TaskItem)data.Tasks[i].Clone());
                }
            }

            // формируем список исходных заданий для шлифования из заданий не требующих распиливания
            TaskList DefaultPolishingList = new TaskList();
            for (int i = 0; i < data.Tasks.Count; i++)
            {
                if (data.Tasks[i].SawingTime(data.Materials, data.Productions) == 0 &&
                    data.Tasks[i].PolishingTime(data.Materials, data.Productions) != 0)
                {
                    DefaultPolishingList.AddCopy((TaskItem)data.Tasks[i].Clone());
                }
            }

            //
            // первая настройка пил
            //

            // сортируем пилы по возрастанию кол-ва поддерживаемых материалов
            for (int i = 0, i_end = data.Saws.Count - 1; i < i_end; i++)
            {
                for (int j = i + 1, j_end = data.Saws.Count; j < j_end; j++)
                {
                    if (data.Saws[i].SupportedMaterials.Count > data.Saws[j].SupportedMaterials.Count)
                    {
                        TimeDevice tmp = data.Saws[i];
                        data.Saws[i] = data.Saws[j];
                        data.Saws[j] = tmp;
                    }
                }
            }

            do
            {
                data.Saws.Service(T);
                // назначаем каждому устройству задание
                for (int i = 0, i_end = data.Saws.Count; i < i_end; i++)
                {
                    if (!data.Saws[i].IsBusy()) // если устройство не занято
                    {
                        int TaskId = data.Saws[i].MostImpTask2(DefaultSawingList, data.Materials, data.Productions, BaseDeviceType.Saw);

                        if (TaskId != -1)
                        {
                            // получаем время до истечения директивногог срока
                            
                            int TaskIndex = DefaultSawingList.GetIndexById(TaskId);

                            if (DefaultSawingList[TaskIndex].UseDeadLine)
                            {
                                // время через которое наступит deadline для задания
                                double TimeToExpare = (DefaultSawingList[TaskIndex].DeadLine - data.BaseTime.AddMinutes(T)).TotalMinutes;

                                if (TimeToExpare > 0) // если время ещё есть
                                {
                                    // рассмотрим, уложиться ли задание в директивный срок 
                                    // в худшем случае относительно настройки и в лучшем случае относительно 
                                    // свободности станков
                                    double RealWorkTime = DefaultSawingList[TaskIndex].SawingTime(data.Materials, data.Productions) +
                                                          DefaultSawingList[TaskIndex].PolishingTime(data.Materials, data.Productions) +
                                                          data.Saws.GetMidConfigTime(DefaultSawingList[TaskIndex].MaterialId) + 
                                                          data.Grinders.GetMidConfigTime(DefaultSawingList[TaskIndex].MaterialId);

                                    // теперь определяем есть ли время, до которого необходимо вкрай начать выполнять это задание
                                    double DeltaTime = TimeToExpare - RealWorkTime;

                                    /*
                                    // определим примерное суммарное время заданий, которое нужно выполнить до этого же срока или ранее
                                    // примерное, потому что не учитываем настройку
                                    double TaskDurationSum = DefaultSawingList.GetTaskDurationByDeadLine(DefaultSawingList[TaskIndex].DeadLine, data.Materials, data.Productions);

                                    // получим количество таких заданий.
                                    // и представим что хотя бы для 20% заданий потребуется настройка
                                    int TaskCountByDeadLine = DefaultSawingList.GetTaskCountByDeadLine(DefaultSawingList[TaskIndex].DeadLine);

                                    // учтем время примерной настройки
                                    TaskDurationSum += 0.2 * TaskCountByDeadLine * (data.Saws.GetMidConfigTime(data.Materials) + data.Grinders.GetMidConfigTime(data.Materials));

                                    // получим новую дельту
                                    DeltaTime -= TaskDurationSum;
                                    */
                                    if (DeltaTime > 0)
                                    {
                                        // а теперь попробуем найти задание
                                        int AlterTaskId = data.Saws[i].MostImpTaskWithTimeLimit(DefaultSawingList, data.Materials, data.Productions, BaseDeviceType.Saw, data.Grinders, DeltaTime);
                                        if (AlterTaskId != -1) // если задание нашлось, то выполняем его
                                        {
                                            TaskId = AlterTaskId;
                                            TaskIndex = DefaultSawingList.GetIndexById(TaskId);
                                        }
                                    }
                                }
                            }

                            int ProdIndex = data.Productions.GetIndexById(DefaultSawingList[TaskIndex].ProductionId);

                            data.Saws[i].AddTask(TaskId, DefaultSawingList[TaskIndex].MaterialId, T, DefaultSawingList[TaskIndex].SawingTime(data.Materials, data.Productions));

                            DefaultSawingList.DeleteById(TaskId);
                        }
                    }
                }

                data.Grinders.Service(T);
                // назначаем каждому станку
                for (int i = 0, i_end = data.Grinders.Count; i < i_end; i++)
                {
                    if (!data.Grinders[i].IsBusy()) // если устройство не занято
                    {
                        /*
                        
                        // получаем самое застарелое задание в очереди
                        int TaskId = data.Grinders[i].GetOldestTask(DefaultPolishingList);
                        if (TaskId != -1)
                        {
                            int TaskIndex = DefaultPolishingList.GetIndexById(TaskId);
                            int ProdIndex = data.Productions.GetIndexById(DefaultPolishingList[TaskIndex].ProductionId);
                            data.Grinders[i].AddTask(TaskId, DefaultPolishingList[TaskIndex].MaterialId, T, DefaultPolishingList[TaskIndex].SawingTime(data.Materials, data.Productions));
                            DefaultPolishingList.DeleteById(TaskId);
                        }
                        */
                        
                        int TaskId = data.Grinders[i].MostImpTask2(DefaultPolishingList, data.Materials, data.Productions, BaseDeviceType.Grinder);

                        if (TaskId != -1)
                        {
                            int TaskIndex = DefaultPolishingList.GetIndexById(TaskId);
                            int ProdIndex = data.Productions.GetIndexById(DefaultPolishingList[TaskIndex].ProductionId);
                            data.Grinders[i].AddTask(TaskId, DefaultPolishingList[TaskIndex].MaterialId, T, DefaultPolishingList[TaskIndex].SawingTime(data.Materials, data.Productions));
                            DefaultPolishingList.DeleteById(TaskId);
                        }
                        
                    }
                }

                //Определяем шаг по времени
                double Step = GetStep(data.Saws, data.Grinders);


                // совершаем временной шаг для пил
                for (int i = 0; i < data.Saws.Count; i++)
                {
                    int CompleteTaskId = data.Saws[i].MakeStep(Step);

                    // если завершилось распиливание
                    if (CompleteTaskId >= 0)
                    {
                        // получаем задание, которое прошло распиливание
                        int TaskIndex = data.Tasks.GetIndexById(CompleteTaskId);

                        // если завершенное задание требует шлифования, отправляем его в очередь на шлифовку
                        if (data.Tasks[TaskIndex].PolishingTime(data.Materials, data.Productions) != 0)
                        {
                            TaskItem TaskCopy = (TaskItem)data.Tasks[TaskIndex].Clone();
                            TaskCopy.TimeOfLastUsing = T;
                            DefaultPolishingList.AddCopy(TaskCopy);
                        }
                    }
                }

                // совершаем временной шаг шлифовщиков
                for (int i = 0; i < data.Grinders.Count; i++)
                {
                    int CompleteTaskId = data.Grinders[i].MakeStep(Step);
                }

                // изменяем текущее время
                T += Step;
            } while (data.Saws.OneWorkDeviceExist() || data.Grinders.OneWorkDeviceExist() ||
                    DefaultSawingList.Count != 0 || DefaultPolishingList.Count != 0);
        }

        /// <summary>
        /// Расставляем директивные сроки в зависмости от заказа
        /// </summary>
        /// <param name="Orders">Списко исходных заказов.</param>
        /// <param name="Tasks">Список исходных заданий.</param>
        private void SetDirectiveFromOrder(OrderList Orders, TaskList Tasks)
        {
            // Копируем директивные сроки из заказа в каждое задание
            for (int i = 0; i < Orders.Count; i++)
            {
                for (int j = 0; j < Tasks.Count; j++)
                {
                    if (Tasks[j].OrderId == Orders[i].Id)
                    {
                        if (Orders[i].DeadLine != null)
                        {
                            Tasks[j].UseDeadLine = true;
                            Tasks[j].DeadLine = Orders[i].DeadLine.Value;
                        }
                        else
                            Tasks[j].UseDeadLine = false;
                    }
                }
            }
        }
    }

}