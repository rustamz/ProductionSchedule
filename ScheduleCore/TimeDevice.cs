using System;
using System.Collections.Generic;

namespace ScheduleCore
{
    /// <summary>
    /// Основные типы устройств.
    /// </summary>
    public enum BaseDeviceType
    {
        /// <summary>
        /// Пила.
        /// </summary>
        Saw,

        /// <summary>
        /// Шлифовальный станок.
        /// </summary>
        Grinder
    }

    /// <summary>
    /// Класс реализующий хранение пары - id материала и время.
    /// </summary>
    public class MaterialPair : ICloneable
    {
        private int id;
        private double time;

        public int ID { get { return id; } set { id = value; } }
        public double Time { get { return time; } set { time = value; } }

        public MaterialPair(int Id, double Time)
        {
            id = Id;
            time = Time;
        }

        public Object Clone()
        {
            return new MaterialPair(id, time);
        }
    }

    public class MaterialPairList : List<MaterialPair>
    {
        public MaterialPairList()
            : base()
        {

        }
    }

    /// <summary>
    /// Класс реализующий хранение на устройстве выполненного задания.
    /// </summary>
    public class CompleteItem
    {
        public int TaskId
        {
            get;
            set;
        }

        public double Begin
        {
            get;
            set;
        }

        public double End
        {
            get;
            set;
        }

        public CompleteItem(int TaskId, double Begin, double End)
        {
            this.TaskId = TaskId;
            this.Begin = Begin;
            this.End = End;
        }
    }

    public class CompleteTaskList : List<CompleteItem>
    {
        public CompleteTaskList()
            : base()
        {
        }
    }


    
    /// <summary>
    /// Класс реализующий приборы, работающие в реальном времени.
    /// </summary>
    public class TimeDevice : BaseScheduleItem
    {

        #region Поля

        /// <summary>
        /// Имя ответственного человека.
        /// </summary>
        protected string responsible;

        /// <summary>
        /// Время, оставщееся до конца некоторой операции в минутах.
        /// </summary>
        protected double time;

        /// <summary>
        /// Период времени, когда необходимо производить техническое обслуживание.
        /// </summary>
        protected double serviceTimePeriod = -1;

        /// <summary>
        /// Период времени за которое соверщается техническое обслуживание.
        /// </summary>
        protected double serviceTime = 0;

        /// <summary>
        /// Содержит время, прошедшее с момента последнего технического обслуживания.
        /// </summary>
        protected double lastServiceTime = 0;

        /// <summary>
        /// Текущая конфигурация устройства.
        /// </summary>
        protected int currentMaterialId = -1;

        /// <summary>
        /// Текущая выполняемая операция
        /// </summary>
        protected int currentTaskId = -1;

        /// <summary>
        /// Настройка пилы в начальный момент времени.
        /// </summary>
        protected int defaultMaterialId = -1;

        /// <summary>
        /// Список, поддерживаемых устройством материалов
        /// </summary>
        private MaterialPairList supMaterialList = new MaterialPairList();

        /// <summary>
        /// Список выполненных заданий.
        /// </summary>
        private CompleteTaskList complTaskList = new CompleteTaskList();

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор для инициализации параметров по умолчанию 
        /// </summary>
        /// <param name="Id">Уникальный код устройства.</param>
        /// <param name="Text">Имя устройства.</param>
        public TimeDevice(int Id, string Text)
            : base(Id, Text)
        {
            time = 0;
        }

        /// <summary>
        /// Конструктор для инициализации параметров по умолчанию  
        /// </summary>
        /// <param name="Text">Имя устройства.</param>
        public TimeDevice(string Text)
            : base(Text)
        {
            time = 0;
        }

        /// <summary>
        /// Конструктор для инициализации параметров по умолчанию  
        /// </summary>
        /// <param name="DeviceName">Имя устройства.</param>
        /// <param name="DeviceResponsible">Имя ответственного человека.</param>
        public TimeDevice(int Id, string Text, string Responsible)
            : base(Id, Text)
        {
            responsible = Responsible;
            time = 0;
        }

        /// <summary>
        /// Конструктор для инициализации параметров по умолчанию 
        /// </summary>
        /// <param name="Id">Уникальный код устройства.</param>
        /// <param name="Text">Имя устройства.</param>
        /// <param name="Responsible">Имя ответственного человека.</param>
        /// <param name="Time">Время, оставщееся до конца некоторой операции в минутах.</param>
        /// <param name="LastUseTime">Время, когда последний раз использовался станок.</param>
        public TimeDevice(int Id, string Text, string Responsible, double Time, double LastUseTime)
            : base(Id, Text)
        {
            responsible = Responsible;
            time = Time;
        }

        /// <summary>
        /// Конструктор для инициализации параметров по умолчанию 
        /// </summary>
        /// <param name="Id">Уникальный код устройства.</param>
        /// <param name="Text">Имя устройства.</param>
        /// <param name="Responsible">Имя ответственного человека.</param>
        /// <param name="Time">Время, оставщееся до конца некоторой операции в минутах.</param>
        /// <param name="LastUseTime">Время, когда последний раз использовался станок.</param>
        /// <param name="SupMat">Список поддерживаемых материалов</param>
        public TimeDevice(int Id, string Text, string Responsible, double Time, MaterialPairList SupMat, CompleteTaskList CompTasks)
            : base (Id, Text)
        {
            responsible = Responsible;
            time = Time;

            foreach (MaterialPair item in SupMat)
                supMaterialList.Add(item);

            foreach (CompleteItem item in CompTasks)
                complTaskList.Add(item);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Возвращает или задаёт имя ответственного чза прибо человека.
        /// </summary>
        public string Responsible 
        { 
            get { return responsible; }
            set { responsible = value; }
        }

        /// <summary>
        /// Возвращает или задаёт время, оставщееся до конца некоторой операции в минутах.
        /// </summary>
        public double Time 
        {
            get { return time; }
            set { time = value; }
        }

        /// <summary>
        /// Возвращает время последнего использования.
        /// </summary>
        public double LastUsed
        { 
            get 
            {
                return complTaskList.Count == 0 ? 0 : complTaskList[complTaskList.Count - 1].End;
            }
        }

        /// <summary>
        /// Возвращает или задаёт время через которое необходимо производить техническое обслуживание.
        /// </summary>
        public double ServiceTimePeriod
        {
            get { return serviceTimePeriod; }
            set { serviceTimePeriod = value; }
        }

        /// <summary>
        /// Возвращает или задаёт время за которое соверщается техническое обслуживание.
        /// </summary>
        public double ServiceTime
        {
            get { return serviceTime; }
            set { serviceTime = value; }
        }

        /// <summary>
        /// Возвращает или задаёт время через которое необходимо производить техническое обслуживание.
        /// </summary>
        public double LastServiceTime
        {
            get { return lastServiceTime; }
            set { lastServiceTime = value; }
        }

        /// <summary>
        /// Возвращает мдентификатор материала,
        /// на который настроен прибор в данный момент.
        /// -1 - прибор не настроен.
        /// </summary>
        public int CurrentMaterialId
        {
            get { return currentMaterialId; }
            set { currentMaterialId = value; }
        }

        /// <summary>
        /// Текущее выполянемое задание.
        /// </summary>
        public int CurrentTaskId 
        {
            get { return currentTaskId; } 
        }

        /// <summary>
        /// Возвращает список поддерживаемых материалов
        /// </summary>
        public MaterialPairList SupportedMaterials 
        {
            get { return supMaterialList; }
            set { supMaterialList = value; }
        }

        /// <summary>
        /// Возвращает или задаёт настройку пилы в начальный момент времени.
        /// -1 - устройство не настроено.
        /// </summary>
        public int DefaultMaterialId
        {
            get { return defaultMaterialId; }
            set 
            {
                if (value == -1)
                {
                    defaultMaterialId = value;
                    return;
                }
                bool DefaultMaterialSet = false;
                foreach (MaterialPair item in supMaterialList)
                {
                    if (item.ID == value)
                    {
                        DefaultMaterialSet = true;
                        defaultMaterialId = value;
                        break;
                    }
                }
                if (!DefaultMaterialSet)
                {
                    throw new Exception("Нельзя задать данный материал в качестве начальной настройки устройства, если устройство не поддерживает этот материал!");
                }
            }
        }

        /// <summary>
        /// Содержит список выполненных заданий.
        /// </summary>
        public CompleteTaskList CompleteTask { get { return complTaskList; } }

        #endregion

        #region Методы
        /// <summary>
        /// Данный метод совершает временной шаг
        /// </summary>
        /// <param name="TimePeriod">Временной шаг в минутах.</param>
        /// <returns>Идентификатор освободившегося задания или -1;</returns>
        public int MakeStep(double TimePeriod)
        {
            lastServiceTime += TimePeriod;

            time = time > TimePeriod ?
                time - TimePeriod : 0;

            int Result = -1;
            if (time == 0)
            {
                Result = currentTaskId;
                currentTaskId = -1;
            }
            return Result;
        }

        /// <summary>
        /// Данный метод определяет, занято ли в данный момент устройство или нет.
        /// </summary>
        /// <returns>Истина, если занято. Ложь, если свободно.</returns>
        public bool IsBusy()
        {
            return time != 0;
        }

        /// <summary>
        /// Добавляет поддержку материала для прибора.
        /// </summary>
        /// <param name="item">Материал.</param>
        public void AddMaterial(MaterialPair item)
        {
            supMaterialList.Add((MaterialPair)item.Clone());
        }

        /// <summary>
        /// Проверяет, поддерживает ли данное устройство материал.
        /// </summary>
        /// <param name="MaterialId">Идентификатор материала.</param>
        /// <returns>Истина, если материал поддерживается.</returns>
        public bool IsSupportMaterial(int MaterialId)
        {
            foreach (MaterialPair item in supMaterialList)
            {
                if (item.ID == MaterialId)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddTask(int TaskId, int MaterialId, double CurrentTime, double OperationTime)
        {
            // вычисляем время на которое будет занято устройство
            Time = OperationTime;
            
            double dTime = -1; // время настройки
            bool NeedToBeConfig = false;
            if (currentMaterialId != MaterialId)
            {
                NeedToBeConfig = true;
                // ищем материал
                foreach (MaterialPair item in supMaterialList)
                {
                    if (item.ID == MaterialId)
                    {
                        dTime = item.Time;
                        break;
                    }
                }

                if (dTime == -1)
                    throw new Exception("Прибор не поддерживает данный материал!");
                
                currentMaterialId = MaterialId;
            }

            currentTaskId = TaskId;
            if (NeedToBeConfig)
            {
                complTaskList.Add(new CompleteItem(-1, CurrentTime, CurrentTime + dTime));
                complTaskList.Add(new CompleteItem(TaskId, CurrentTime + dTime, CurrentTime + dTime + Time));
                Time = OperationTime + dTime;

            }
            else
            {
                complTaskList.Add(new CompleteItem(TaskId, CurrentTime, CurrentTime + Time));
                Time = OperationTime;
            }
        }

        /// <summary>
        /// Сбрасывает выполненные задания из памяти устройства.
        /// </summary>
        public void ClearCompleteTasks()
        {
            complTaskList.Clear();
            time = 0;
            lastServiceTime = 0;
            currentMaterialId = defaultMaterialId;
            currentTaskId = -1;
        }

        /// <summary>
        /// Возвращает время конфигурации станка под данный материал,
        /// либо -1.
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public double GetConfigTime(int MaterialId)
        {
            double Result = -1;
            foreach (MaterialPair item in supMaterialList)
            {
                if (item.ID == MaterialId)
                {
                    Result = item.Time;
                    break;
                }
            }
            return Result;
        }

        /// <summary>
        /// Возвращает идентификатор наиболее перспективного задания 
        /// с учётом текущих настроек устройства.
        /// </summary>
        /// <param name="Tasks"></param>
        /// <param name="Products"></param>
        /// <param name="Device">Тип устройства</param>
        /// <param name="BigFirst">Отбирать сначало длительные задания</param>
        /// <returns></returns>
        public int MostImpTask(TaskList Tasks, MaterialList Materials, ProductionList Productions, BaseDeviceType Device, bool BigFirst = true)
        {
            int Result = -1;
            int GoalMaterial = currentMaterialId;

            // если устройство уже сконфигурировано
            bool TaskWithCurrentMaterialExist = GoalMaterial != -1 ? Tasks.TaskWithMaterialsExists(GoalMaterial) : true;

            // если устройство не сконфигурировано,
            // под определённый материал или задания под текущий материал
            // не существуют, то выбираем самое длинное задание
            // состоящее из данного материала
            if (GoalMaterial == -1 || !TaskWithCurrentMaterialExist)
            {
                GoalMaterial = GetBiggerMaterial(Tasks, Materials, Productions, Device);
                if (GoalMaterial == -1)
                    return Result;
            }

            return BigFirst ? Tasks.GetBiggerTask(GoalMaterial, Materials, Productions, Device)
                : Tasks.GetSmallestTask(GoalMaterial, Materials, Productions, Device);
        }

        public int MostImpTask2(TaskList Tasks, MaterialList Materials, ProductionList Productions, BaseDeviceType Device)
        {
            // ищем задание, которое истечет быстрее всего
            DateTime MinTime = new DateTime();
            int ResultIndex = -1;

            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].UseDeadLine)
                {
                    if (IsSupportMaterial(Tasks[i].MaterialId))
                    {
                        if (ResultIndex == -1)
                        {
                            MinTime = Tasks[i].DeadLine;
                            ResultIndex = i;
                        }
                        else
                        {
                            if (Tasks[i].DeadLine < MinTime)
                            {
                                MinTime = Tasks[i].DeadLine;
                                ResultIndex = i;
                            }
                        }
                    }
                }
            }

            // если задание найдено
            if (ResultIndex != -1)
            {
 
            }
            else // если задание не найдено
            {
                return MostImpTask(Tasks, Materials, Productions, Device);
            }

            return Tasks[ResultIndex].Id;
        }

        /// <summary>
        /// Возвращает идентификатор самого старого задания в списке
        /// </summary>
        /// <param name="Tasks"></param>
        /// <returns></returns>
        public int GetOldestTask(TaskList Tasks)
        {
            int ResultIndex = -1;
            for (int i = 0; i < Tasks.Count; i++)
            {
                // если устройство поддерживает данный материал
                if (IsSupportMaterial(Tasks[i].MaterialId))
                {
                    if (ResultIndex != -1)
                    {
                        if (Tasks[ResultIndex].TimeOfLastUsing < Tasks[ResultIndex].TimeOfLastUsing)
                        {
                            ResultIndex = i;
                        }
                    }
                    else
                    {
                        ResultIndex = i;
                    }
                }
            }
            return ResultIndex != -1 ? Tasks[ResultIndex].Id : -1;
        }

        public int MostImpTaskWithTimeLimit(TaskList Tasks, MaterialList Materials, ProductionList Productions, BaseDeviceType Device, DeviceList OtherDeviceList, double TimeLimit, bool BigFirst = true)
        {
            int GoalMaterial = currentMaterialId;

            // сперва ищем среди заданий текущего материала
            MaterialPairList mpl = new MaterialPairList();
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].MaterialId == GoalMaterial)
                {
                    mpl.Add(new MaterialPair(Tasks[i].Id, Tasks[i].SawingTime(Materials, Productions) +
                                          Tasks[i].PolishingTime(Materials, Productions) + OtherDeviceList.GetMidConfigTime(Tasks[i].MaterialId)));
                }
            }

            // сортируем по убыванию времени
            for (int i = 0, i_end = mpl.Count - 1; i < i_end; i++)
            {
                for (int j = i + 1, j_end = mpl.Count; j < j_end; j++)
                {
                    if (mpl[i].Time < mpl[j].Time)
                    {
                        MaterialPair tmp = mpl[i];
                        mpl[i] = mpl[j];
                        mpl[j] = tmp;
                    }
                }
            }

            for (int i = 0; i < mpl.Count; i++)
            {
                if (mpl[i].Time < TimeLimit)
                    return mpl[i].ID;
            }

            // если дошли до этого места, то походящего задания из тех что под данный материал не нашлось
            // поэтому шерстим все остальные
            List<int> MaterialExcept = new List<int>();
            MaterialExcept.Add(GoalMaterial);

            // пока есть не просмотренные материалы
            while (MaterialExcept.Count != Materials.Count)
            {
                GoalMaterial = GetBiggerMaterialWithExcept(Tasks, Materials, Productions, Device, MaterialExcept);
                if (!this.IsSupportMaterial(GoalMaterial))
                {
                    MaterialExcept.Add(GoalMaterial);
                    continue;
                }
                mpl.Clear();
                for (int i = 0; i < Tasks.Count; i++)
                {
                    if (Tasks[i].MaterialId == GoalMaterial)
                    {
                        mpl.Add(new MaterialPair(Tasks[i].Id, Tasks[i].SawingTime(Materials, Productions) +
                            Tasks[i].PolishingTime(Materials, Productions) + GetConfigTime(Tasks[i].MaterialId) +
                            OtherDeviceList.GetMidConfigTime(Tasks[i].MaterialId)));
                    }
                }

                // сортируем по убыванию времени
                for (int i = 0, i_end = mpl.Count - 1; i < i_end; i++)
                {
                    for (int j = i + 1, j_end = mpl.Count; j < j_end; j++)
                    {
                        if (mpl[i].Time < mpl[j].Time)
                        {
                            MaterialPair tmp = mpl[i];
                            mpl[i] = mpl[j];
                            mpl[j] = tmp;
                        }
                    }
                }

                for (int i = 0; i < mpl.Count; i++)
                {
                    if (mpl[i].Time < TimeLimit)
                        return mpl[i].ID;
                }

                MaterialExcept.Add(GoalMaterial);
            }
            return -1;
        }

        /// <summary>
        /// Возвращает идентификатор наиболее перспективного задания 
        /// с учётом текущих настроек устройства и времени.
        /// </summary>
        /// <param name="Tasks"></param>
        /// <param name="Products"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public int MostImpTaskByTime(TaskList Tasks, MaterialList Materials, ProductionList Productions , int CurrentTime, BaseDeviceType Device)
        {
            int Result = -1;
            int GoalMaterial = currentMaterialId;

            // если устройство уже сконфигурировано
            bool TaskWithCurrentMaterialExist = GoalMaterial != -1 ? 
                Tasks.TaskWithMaterialsExists(GoalMaterial) : true;

            // если устройство не сконфигурировано,
            // под определённый материал или задания под текущий материал
            // не существуют, то выбираем самое длинное задание
            // состоящее из данного материала
            if (GoalMaterial == -1 || !TaskWithCurrentMaterialExist)
            {
                GoalMaterial = GetBiggerMaterial(Tasks, Materials, Productions, Device);
                if (GoalMaterial == -1)
                    return Result;
            }

            return Tasks.GetBiggerTask(GoalMaterial, Materials, Productions, Device);
        }

        /// <summary>
        /// Возвращает идентификатор наиболее перспективного задания 
        /// c независмым от текущих настроек результатом.
        /// </summary>
        /// <param name="Tasks"></param>
        /// <param name="Products"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public int ImpTask(TaskList Tasks, MaterialList Materials, ProductionList Productions, BaseDeviceType Device)
        {
            int GoalMaterial = GetBiggerMaterial(Tasks, Materials, Productions, Device);
            if (GoalMaterial == -1)
                return -1;

            return Tasks.GetBiggerTask(GoalMaterial, Materials, Productions, Device);
        }

        /// <summary>
        /// Создаёт и возвращает копию объекта.
        /// </summary>
        /// <returns>Копия объекта.</returns>
        public new Object Clone()
        {
            TimeDevice clone = new TimeDevice(Id, Text, responsible, time, supMaterialList, complTaskList);
            clone.serviceTimePeriod = serviceTimePeriod;
            clone.serviceTime = serviceTime;
            clone.lastServiceTime = lastServiceTime;
            clone.defaultMaterialId = defaultMaterialId;
            return clone;
        }

        /// <summary>
        /// Возвращает идентификатор материала из множества материалов, выполянющихся по заданиям, в сумме дольше других.
        /// </summary>
        /// <param name="Tasks"></param>
        /// <param name="Products"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public int GetBiggerMaterial(TaskList Tasks, MaterialList Materials, ProductionList Productions, BaseDeviceType Device)
        {
            // определяем какого из поддерживаемых материалов
            // больше всего среди заданий
            List<MaterialPair> material = new List<MaterialPair>();

            // перебираем задания и наполняем список времен
            for (int i = 0; i < Tasks.Count; i++)
            {
                int CurrentMaterialID = Tasks[i].MaterialId;

                // если устройство не поддерживает материал,
                // то продолжаем перебор
                if (!IsSupportMaterial(CurrentMaterialID))
                    continue;


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
                double AddTime = (Device == BaseDeviceType.Saw ? Tasks[i].SawingTime(Materials, Productions) : Tasks[i].PolishingTime(Materials, Productions));
                if (FindedMaterialIndex > -1)
                    material[FindedMaterialIndex].Time += AddTime;
                else // если материала нет в списке
                    material.Add(new MaterialPair(CurrentMaterialID, AddTime));
            }

            if (material.Count == 0)
            {
                return -1;
            }

            // дополняем список времен, временем на конфигурацию
            for (int i = 0, i_end = material.Count; i < i_end; i++)
            {
                material[i].Time += GetConfigTime(material[i].ID);
            }

            // выбирем материал с самым долгим временем
            int ResultIndex = 0;
            for (int i = 1; i < material.Count; i++)
            {
                if (material[i].Time > material[ResultIndex].Time)
                {
                    ResultIndex = i;
                }
            }

            return material[ResultIndex].ID;
        }

        /// <summary>
        /// Возвращает идентификатор материала из множества материалов, выполянющихся по заданиям, в сумме дольше других.
        /// </summary>
        /// <param name="Tasks"></param>
        /// <param name="Products"></param>
        /// <param name="Device"></param>
        /// <returns></returns>
        public int GetBiggerMaterialWithExcept(TaskList Tasks, MaterialList Materials, ProductionList Productions, BaseDeviceType Device, List<int> MaterialExcept)
        {
            // определяем какого из поддерживаемых материалов
            // больше всего среди заданий
            List<MaterialPair> material = new List<MaterialPair>();

            // перебираем задания и наполняем список времен
            for (int i = 0; i < Tasks.Count; i++)
            {
                int CurrentMaterialID = Tasks[i].MaterialId;

                // если устройство не поддерживает материал,
                // то продолжаем перебор
                if (!IsSupportMaterial(CurrentMaterialID))
                    continue;


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
                double AddTime = (Device == BaseDeviceType.Saw ? Tasks[i].SawingTime(Materials, Productions) : Tasks[i].PolishingTime(Materials, Productions));
                if (FindedMaterialIndex > -1)
                    material[FindedMaterialIndex].Time += AddTime;
                else // если материала нет в списке
                    material.Add(new MaterialPair(CurrentMaterialID, AddTime));
            }

            if (material.Count == 0)
            {
                return -1;
            }

            // дополняем список времен, временем на конфигурацию
            for (int i = 0, i_end = material.Count; i < i_end; i++)
            {
                material[i].Time += GetConfigTime(material[i].ID);
            }

            // выбирем материал с самым долгим временем
            int ResultIndex = 0;
            for (int i = 1; i < material.Count; i++)
            {
                if (material[i].Time > material[ResultIndex].Time && !MaterialExcept.Contains(material[i].ID))
                {
                    ResultIndex = i;
                }
            }

            return material[ResultIndex].ID;
        }

        /// <summary>
        /// Проверяет не пришло ли время технического обслуживания
        /// </summary>
        /// <returns></returns>
        public bool TimeToService()
        {
            return serviceTimePeriod != -1 ?
                (lastServiceTime >= serviceTimePeriod ? true : false) :
                false;
        }

        // Осуществляем сервисное обслуживание
        public void Service(double CurrentTime)
        {
            // если устройство не занято
            if (!IsBusy())
            {
                Time = serviceTime;
                lastServiceTime = 0;
                currentTaskId = -2;
                complTaskList.Add(new CompleteItem(-2, CurrentTime, CurrentTime + serviceTime));
            }
        }

        public int IndexByTaskId(int TaskId)
        {
            for (int i = 0; i < complTaskList.Count; i++)
            {
                if (complTaskList[i].TaskId == TaskId)
                    return i;
            }
            return -1;
        }

        #endregion

    }
}
