using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace ScheduleCore
{
    /// <summary>
    /// Данный красс реализует хранение и обработку
    /// данных расписания. Класс также предоставляет методы
    /// для записи и чтения данных класса на внешнее устройство. 
    /// </summary>
    public class Configuration : ICloneable
    {
        #region Скрытые поля
        /// <summary>
        /// Версия класса.
        /// </summary>
        protected const int version = 2;

        /// <summary>
        /// Базовое время для расписания.
        /// </summary>
        private DateTime baseTime;

        /// <summary>
        /// Список доступных материалов.
        /// </summary>
        private MaterialList materials = new MaterialList();

        /// <summary>
        /// Список доступных пил.
        /// </summary>
        private DeviceList saws = new DeviceList();

        /// <summary>
        /// Список доступных шлифовальных станков.
        /// </summary>
        private DeviceList grinders = new DeviceList();

        /// <summary>
        /// Список доступных единиц продукции.
        /// </summary>
        private ProductionList productions = new ProductionList();

        /// <summary>
        /// Список зарегистрированных заказчиков.
        /// </summary>
        private CustomerList customers = new CustomerList();

        /// <summary>
        /// Список оформленных заказов.
        /// </summary>
        private OrderList orders = new OrderList();

        /// <summary>
        /// Список доступных заданий.
        /// </summary>
        private TaskList tasks = new TaskList();
        #endregion

        #region Открытые свойства

        /// <summary>
        /// Возвращает или задает базовое время.
        /// </summary>
        public DateTime BaseTime
        {
            get { return baseTime; }
            set { baseTime = value; }
        }

        /// <summary>
        /// Возвращает или задает список доступных материалов.
        /// </summary>
        public MaterialList Materials
        {
            get { return materials; }
            set { materials = value; }
        }

        /// <summary>
        /// Возвращает или задает список доступных пил.
        /// </summary>
        public DeviceList Saws
        {
            get { return saws; }
            set { saws = value; }
        }

        /// <summary>
        /// Возвращает или задает список доступных шлифовальных станков.
        /// </summary>
        public DeviceList Grinders
        {
            get { return grinders; }
            set { grinders = value; } 
        }

        /// <summary>
        /// Возвращает или задает список доступных единиц продукции.
        /// </summary>
        public ProductionList Productions
        {
            get { return productions; }
            set { productions = value; }            
        }


        /// <summary>
        /// Возвращает или задает список зарегистрированных заказчиков.
        /// </summary>
        public CustomerList Customers
        {
            get { return customers; }
            set { customers = value; }
        }

        /// <summary>
        /// Возвращает или задает список оформленных заказов.
        /// </summary>
        public OrderList Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        /// <summary>
        /// Возвращает или задает список доступных заданий.
        /// </summary>
        public TaskList Tasks
        {
            get { return tasks; }
            set { tasks = value; }  
        }

        #endregion

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public Configuration()
        {
            // добавьте код инициализации
        }


        /// <summary>
        /// Считывает доступные материалы.
        /// </summary>
        /// <param name="XMLMaterialLists">Список узлов, соответствующих тегу "Materials", содержащих информацию о доступных материалах.</param>
        /// <exception cref="System.Exception"></exception>
        private void LoadMaterials(XmlNodeList XMLMaterialLists)
        {
            // очищаем список
            materials.Clear();

            // перебираем все теги в которых описаны материалы
            foreach (XmlNode mat in XMLMaterialLists)
            {
                XmlNodeList MaterialsDescription = mat.ChildNodes;
                foreach (XmlNode item in MaterialsDescription)
                {
                    // проверяем узел на возможность обработки
                    if (item.Name != "MaterialItem")
                        throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Materials. Единственный допустимый тег: \"MaterialItem\"");

                    // получаем идентификатор материала
                    XmlNode XMLItemAttribute = item.Attributes["id"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"MaterialItem\" не содержит обязательный атрибут \"id\"");

                    int ItemId = -1;
                    try
                    {
                        ItemId = Convert.ToInt32(XMLItemAttribute.Value);
                    }
                    catch (Exception ex) 
                    {
                        throw new Exception("\"id\": " + ex.Message);
                    }


                    // получаем имя материала
                    XMLItemAttribute = item.Attributes["Name"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"MaterialItem\" не содержит обязательный атрибут \"Name\"");

                    string MaterialName = XMLItemAttribute.Value;

                    // считываем время распиливания
                    XMLItemAttribute = item.Attributes["SawingTime"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"MaterialItem\" не содержит обязательный атрибут \"SawingTime\"");

                    int SawingTime = 0;
                    if (XMLItemAttribute.Value.Length > 0)
                    {
                        try
                        {
                            SawingTime = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Атрибут \"SawingTime\": " + ex.Message);
                        }
                    }

                    // считываем время шлифования
                    XMLItemAttribute = item.Attributes["PolishingTime"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"MaterialItem\" не содержит обязательный атрибут \"PolishingTime\"");

                    int PolishingTime = 0;
                    if (XMLItemAttribute.Value.Length > 0)
                    {
                        try
                        {
                            PolishingTime = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Атрибут \"PolishingTime\": " + ex.Message);
                        }
                    }

                    // добавляем материал в список
                    materials.Add(new MaterialItem(ItemId, MaterialName, DeleteWhiteSpace(item.InnerText), SawingTime, PolishingTime));
                }
            }
        }

        /// <summary>
        /// Считывает доступных заказчиков.
        /// </summary>
        /// <param name="XMLMaterialLists"></param>
        /// <exception cref="System.Exception"></exception>
        private void LoadCustomers(XmlNodeList XMLCustomerLists)
        {
            // очищаем список
            customers.Clear();

            // перебираем все теги в которых описаны материалы
            foreach (XmlNode cust in XMLCustomerLists)
            {
                XmlNodeList SubItems = cust.ChildNodes;
                foreach (XmlNode item in SubItems)
                {
                    // проверяем узел на возможность обработки
                    if (item.Name != "Customer")
                        throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Customers. Единственный допустимый тег: \"Customer\"");

                    // получаем идентификатор
                    XmlNode XMLItemAttribute = item.Attributes["id"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"Customer\" не содержит обязательный атрибут \"id\"");

                    int ItemId = -1;
                    try
                    {
                        ItemId = Convert.ToInt32(XMLItemAttribute.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("\"id\": " + ex.Message);
                    }

                    // получаем имя материала
                    XMLItemAttribute = item.Attributes["Name"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"Customer\" не содержит обязательный атрибут \"Name\"");

                    string CustomerName = XMLItemAttribute.Value;

                    // получаем телефон
                    XMLItemAttribute = item.Attributes["Phone"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"Customer\" не содержит обязательный атрибут \"Phone\"");

                    string CustomerPhone = XMLItemAttribute.Value;

                    // получаем адрес
                    XMLItemAttribute = item.Attributes["Address"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"Customer\" не содержит обязательный атрибут \"Address\"");

                    string CustomerAdsress = XMLItemAttribute.Value;

                    // добавляем материал в список
                    customers.Add(new CustomerItem(ItemId, CustomerName, CustomerPhone, CustomerAdsress));
                }
            }
        }

        /// <summary>
        /// Считывает доступные заказы
        /// </summary>
        /// <param name="XMLMaterialLists"></param>
        /// <exception cref="System.Exception"></exception>
        private void LoadOrders(XmlNodeList XMLOrderLists)
        {
            // очищаем список
            orders.Clear();

            // перебираем все теги в которых описаны материалы
            foreach (XmlNode ord in XMLOrderLists)
            {
                XmlNodeList SubItems = ord.ChildNodes;
                foreach (XmlNode item in SubItems)
                {
                    // проверяем узел на возможность обработки
                    if (item.Name != "Order")
                        throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Orders. Единственный допустимый тег: \"Order\"");

                    // получаем идентификатор
                    XmlNode XMLItemAttribute = item.Attributes["Index"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"Order\" не содержит обязательный атрибут \"Index\"");

                    int ItemIndex = -1;
                    try
                    {
                        ItemIndex = Convert.ToInt32(XMLItemAttribute.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("\"Index\": " + ex.Message);
                    }

                    // получаем заказчика
                    XMLItemAttribute = item.Attributes["CustomerId"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"Order\" не содержит обязательный атрибут \"CustomerId\"");

                    int CustomerId = -1;
                    try
                    {
                        CustomerId = Convert.ToInt32(XMLItemAttribute.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("\"Order\": " + ex.Message);
                    }

                    // получаем телефон
                    XMLItemAttribute = item.Attributes["Date"];
                    if (XMLItemAttribute == null)
                        throw new Exception("Узел \"Order\" не содержит обязательный атрибут \"Date\"");

                    DateTime Date = StringToDateTime(XMLItemAttribute.Value);

                    // считываем директивный срок
                    DateTime? DeadLine = null;
                    XMLItemAttribute = item.Attributes["DeadLine"];
                    if (XMLItemAttribute != null)
                    {
                        try
                        {
                            DeadLine = StringToDateTime(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Ошибка преобразования даты в узле \"Order\": " + ex.Message);
                        }
                    }


                    // добавляем материал в список
                    orders.Add(new OrderItem(ItemIndex, CustomerId, Date, DeadLine));
                }
            }
        }

        /// <summary>
        /// Считывает доступные устройства.
        /// </summary>
        /// <param name="XMLDeviceLists">Список узлов, соответствующих тегу "Devices", содержащих информацию о доступных устройствах.</param>
        private void LoadDevices(XmlNodeList XMLDeviceLists)
        {
            //
            // Да. Конечно, наблюдательный программист заметит,
            // что информация по пилам и станкам однинакова и дублировать
            // код бессмысленно. Для этого я хочу оставить своё замечание.
            // В перспективе, программа может здорово расшириться и учитывать гораздо больше
            // ньюансов, чем в данный момент, поэтому и код чтения может измениться как в общем
            // так и отдельно. Если Вы всё же решите изменить код, то меняйте на здоровье, но
            // учитывайте мой посыл.
            //
            
            // очищаем списки
            saws.Clear();
            grinders.Clear();

            foreach (XmlNode Dev in XMLDeviceLists)
            {
                XmlNodeList DeviceDescription = Dev.ChildNodes;
                foreach (XmlNode item in DeviceDescription)
                {
                    // проверяем узел на возможность обработки
                    if (item.Name == "Saw")
                    {
                        // получаем идентификатор
                        XmlNode XMLItemAttribute = item.Attributes["id"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Saw\" не содержит обязательный атрибут \"id\"");

                        int ItemId = -1;
                        try
                        {
                            ItemId = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"id\": " + ex.Message);
                        }
                        
                        
                        // считываем имя пилы
                        XMLItemAttribute = item.Attributes["Name"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Saw\" не содержит обязательный атрибут \"Name\"");

                        string DeviceName = XMLItemAttribute.Value;

                        // считываем ответственного человека
                        XMLItemAttribute = item.Attributes["Responsible"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Saw\" не содержит обязательный атрибут \"Responsible\"");

                        string Responsible = XMLItemAttribute.Value;

                        // считываем период технического обслуживания
                        XMLItemAttribute = item.Attributes["ServicePeriod"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Saw\" не содержит обязательный атрибут \"ServicePeriod\"");

                        int ServiceTimePeriod = -1;
                        try
                        {
                            ServiceTimePeriod = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"ServicePeriod\": " + ex.Message);
                        }

                        // считываем время технического обслуживания
                        XMLItemAttribute = item.Attributes["ServiceTime"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Saw\" не содержит обязательный атрибут \"ServiceTime\"");

                        int ServiceTime = -1;
                        try
                        {
                            ServiceTime = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"ServiceTime\": " + ex.Message);
                        }

                        // считываем настройку пилы по умолчанию
                        XMLItemAttribute = item.Attributes["DefaultMaterialId"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Saw\" не содержит обязательный атрибут \"DefaultMaterialId\"");

                        int DefaultMaterialId = -1;
                        try
                        {
                            DefaultMaterialId = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"DefaultMaterialId\": " + ex.Message);
                        }

                        TimeDevice NewTimeDevice = new TimeDevice(ItemId, DeviceName, Responsible);
                        NewTimeDevice.ServiceTimePeriod = ServiceTimePeriod;
                        NewTimeDevice.ServiceTime = ServiceTime;

                        // считываем материалы, которые способна обрабатывать пила
                        XmlNodeList SupportedMaterials = item.ChildNodes;
                        if (SupportedMaterials != null)
                        {
                            foreach (XmlNode SupportedMaterial in SupportedMaterials)
                            {
                                if (SupportedMaterial.Name != "SupportedMaterial")
                                    throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Saw. Единственный допустимый тег: \"SupportedMaterial\"");

                                XMLItemAttribute = SupportedMaterial.Attributes["MaterialId"];
                                if (XMLItemAttribute == null)
                                    throw new Exception("Узел \"SupportedMaterial\" не содержит обязательный атрибут \"MaterialId\"");

                                int MaterialId = -1;
                                try
                                {
                                    MaterialId = Convert.ToInt32(XMLItemAttribute.Value);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("\"MaterialId\": " + ex.Message);
                                }

                                // уникальный идентификатор материала
                                int MaterialIndex = materials.GetIndexById(MaterialId);

                                if (MaterialIndex == -1)
                                    throw new Exception("\"" + XMLItemAttribute.Value + "\": описание материала не найдено!");

                                XmlNode XMLMaterialConfTime = SupportedMaterial.Attributes["ConfTime"];
                                if (XMLMaterialConfTime == null)
                                    throw new Exception("Узел \"SupportedMaterial\" не содержит обязательный атрибут \"ConfTime\"");

                                // время на настройку пилы под этот материал
                                int ConfTime = 0;
                                try
                                {
                                    ConfTime = Convert.ToInt32(XMLMaterialConfTime.Value);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Атрибут \"ConfTime\": " + ex.Message);
                                }

                                NewTimeDevice.AddMaterial(new MaterialPair(MaterialId, ConfTime));
                            }
                        }

                        NewTimeDevice.DefaultMaterialId = DefaultMaterialId;

                        saws.Add(NewTimeDevice);
                    }
                    else if (item.Name == "Grinder")
                    {
                        // получаем идентификатор
                        XmlNode XMLItemAttribute = item.Attributes["id"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Grinder\" не содержит обязательный атрибут \"id\"");

                        int ItemId = -1;
                        try
                        {
                            ItemId = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"id\": " + ex.Message);
                        }
                        
                        // добавление шлифовального станка
                        XMLItemAttribute = item.Attributes["Name"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Grinder\" не содержит обязательный атрибут \"Name\"");

                        string DeviceName = XMLItemAttribute.Value;

                        // считываем ответственного человека
                        XMLItemAttribute = item.Attributes["Responsible"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Grinder\" не содержит обязательный атрибут \"Responsible\"");

                        string Responsible = XMLItemAttribute.Value;

                        // считываем период технического обслуживания
                        XMLItemAttribute = item.Attributes["ServicePeriod"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Grinder\" не содержит обязательный атрибут \"ServicePeriod\"");

                        int ServiceTimePeriod = -1;
                        try
                        {
                            ServiceTimePeriod = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"ServicePeriod\": " + ex.Message);
                        }

                        // считываем время технического обслуживания
                        XMLItemAttribute = item.Attributes["ServiceTime"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Grinder\" не содержит обязательный атрибут \"ServiceTime\"");

                        int ServiceTime = -1;
                        try
                        {
                            ServiceTime = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"ServiceTime\": " + ex.Message);
                        }

                        // считываем настройку пилы по умолчанию
                        XMLItemAttribute = item.Attributes["DefaultMaterialId"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Grinder\" не содержит обязательный атрибут \"DefaultMaterialId\"");

                        int DefaultMaterialId = -1;
                        try
                        {
                            DefaultMaterialId = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"DefaultMaterialId\": " + ex.Message);
                        }

                        TimeDevice NewTimeDevice = new TimeDevice(ItemId, DeviceName, Responsible);
                        NewTimeDevice.ServiceTimePeriod = ServiceTimePeriod;
                        NewTimeDevice.ServiceTime = ServiceTime;

                        // считываем материалы, которые способна обрабатывать пила
                        XmlNodeList SupportedMaterials = item.ChildNodes;
                        if (SupportedMaterials != null)
                        {
                            foreach (XmlNode SupportedMaterial in SupportedMaterials)
                            {
                                if (SupportedMaterial.Name != "SupportedMaterial")
                                    throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Saw. Единственный допустимый тег: \"SupportedMaterial\"");

                                XMLItemAttribute = SupportedMaterial.Attributes["MaterialId"];
                                if (XMLItemAttribute == null)
                                    throw new Exception("Узел \"SupportedMaterial\" не содержит обязательный атрибут \"MaterialId\"");

                                int MaterialId = -1;
                                try
                                {
                                    MaterialId = Convert.ToInt32(XMLItemAttribute.Value);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("\"MaterialId\": " + ex.Message);
                                }

                                // уникальный идентификатор материала
                                int MaterialIndex = materials.GetIndexById(MaterialId);

                                if (MaterialIndex == -1)
                                    throw new Exception("\"" + XMLItemAttribute.Value + "\": описание материала не найдено!");

                                XmlNode XMLMaterialConfTime = SupportedMaterial.Attributes["ConfTime"];
                                if (XMLMaterialConfTime == null)
                                    throw new Exception("Узел \"SupportedMaterial\" не содержит обязательный атрибут \"ConfTime\"");

                                // время на настройку пилы под этот материал
                                int ConfTime = 0;
                                try
                                {
                                    ConfTime = Convert.ToInt32(XMLMaterialConfTime.Value);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Атрибут \"ConfTime\": " + ex.Message);
                                }

                                NewTimeDevice.AddMaterial(new MaterialPair(MaterialId, ConfTime));
                            }
                        }

                        NewTimeDevice.DefaultMaterialId = DefaultMaterialId;
                        grinders.Add(NewTimeDevice);
                    }
                    else throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Devices. Допустимые узлы: \"Saw\", \"Grinder\".");
                }
            }
        }

        /// <summary>
        /// Загружает доступные продукции.
        /// </summary>
        /// <param name="XMLProductLists"></param>
        private void LoadProducts(XmlNodeList XMLProductLists)
        {
            // очищаем список
            productions.Clear();

            // перебируем все списки содержащие доступные продукции
            foreach (XmlNode Product in XMLProductLists)
            {
                XmlNodeList SameProlList = Product.ChildNodes;
                foreach (XmlNode item in SameProlList)
                {
                    // проверяем узел на возможность обработки
                    if (item.Name == "ProductItem")
                    {
                        // получаем идентификатор
                        XmlNode XMLItemAttribute = item.Attributes["id"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"Saw\" не содержит обязательный атрибут \"id\"");

                        int ItemId = -1;
                        try
                        {
                            ItemId = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"id\": " + ex.Message);
                        }

                        // считываем имя продукции
                        XMLItemAttribute = item.Attributes["Name"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"Name\"");

                        string ProductionName = XMLItemAttribute.Value;

                        ProductionItem NewProductionItem = new ProductionItem(ItemId, ProductionName);

                        XmlNodeList SupportedSizes = item.ChildNodes;
                        if (SupportedSizes != null)
                        {
                            foreach (XmlNode SupportedSize in SupportedSizes)
                            {
                                if (SupportedSize.Name != "Size")
                                    throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла ProductItem. Единственный допустимый тег: \"Size\"");

                                // считываем индекс
                                XMLItemAttribute = SupportedSize.Attributes["Index"];
                                if (XMLItemAttribute == null)
                                    throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"Index\"");
                                
                                int Index = 0;
                                if (XMLItemAttribute.Value.Length > 0)
                                {
                                    try
                                    {
                                        Index = Convert.ToInt32(XMLItemAttribute.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Атрибут \"Index\": " + ex.Message);
                                    }
                                }

                                // считываем длину заготовки
                                XMLItemAttribute = SupportedSize.Attributes["Length"];
                                if (XMLItemAttribute == null)
                                    throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"Length\"");

                                double Length = 0;
                                if (XMLItemAttribute.Value.Length > 0)
                                {
                                    try
                                    {
                                        Length = Convert.ToDouble(XMLItemAttribute.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Атрибут \"Length\": " + ex.Message);
                                    }
                                }

                                // считываем ширину заготовки
                                XMLItemAttribute = SupportedSize.Attributes["Width"];
                                if (XMLItemAttribute == null)
                                    throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"Width\"");

                                double Width = 0;
                                if (XMLItemAttribute.Value.Length > 0)
                                {
                                    try
                                    {
                                        Width = Convert.ToDouble(XMLItemAttribute.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Атрибут \"Width\": " + ex.Message);
                                    }
                                }

                                // считываем высоту заготовки
                                XMLItemAttribute = SupportedSize.Attributes["Height"];
                                if (XMLItemAttribute == null)
                                    throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"Height\"");

                                double Height = 0;
                                if (XMLItemAttribute.Value.Length > 0)
                                {
                                    try
                                    {
                                        Height = Convert.ToDouble(XMLItemAttribute.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Атрибут \"Height\": " + ex.Message);
                                    }
                                }

                                NewProductionItem.SupSizes.Add(new ProductionSize(Index, Length, Width, Height));
                            }
                        }

                        productions.Add(NewProductionItem);

                    }
                    else throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Devices. Допустимые узлы: \"Saw\", \"Grinder\".");
                }
            }
        }

        /// <summary>
        /// Считывает все задания
        /// </summary>
        /// <param name="XMLTaskLists"></param>
        public void LoadTasks(XmlNodeList XMLTaskLists)
        {
            // очищаем список
            tasks.Clear();

            // перебируем все списки содержащие задания.
            foreach (XmlNode Tasks in XMLTaskLists)
            {
                XmlNodeList Task = Tasks.ChildNodes;
                foreach (XmlNode item in Task)
                {
                    // проверяем узел на возможность обработки
                    if (item.Name == "TaskItem")
                    {
                        // получаем идентификатор
                        XmlNode XMLItemAttribute = item.Attributes["id"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"TaskItem\" не содержит обязательный атрибут \"id\"");

                        int ItemId = -1;
                        try
                        {
                            ItemId = Convert.ToInt32(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"id\": " + ex.Message);
                        }

                        // считываем id продукции
                        XMLItemAttribute = item.Attributes["ProductionId"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"TaskItem\" не содержит обязательный атрибут \"ProductionId\"");

                        int ProductionId = -1;
                        try
                        {
                            ProductionId = Int32.Parse(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"ProductionId\": " + ex.Message);
                        }

                        int ProductIndex = productions.GetIndexById(ProductionId);
                        if (ProductIndex == -1)
                            throw new Exception(item.Name + ": продукция с id " + XMLItemAttribute.Value + " не существует!");

                        // считываем индекс продукции
                        XMLItemAttribute = item.Attributes["SizeIndex"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"SizeIndex\"");

                        int ProductionSize = -1;
                        try
                        {
                            ProductionSize = productions[ProductIndex].SupSizes.IndexOf(Int32.Parse(XMLItemAttribute.Value));
                        }
                        catch(Exception ex)
                        {
                            throw new Exception("\"SizeIndex\": " + ex.Message);
                        }

                        // считываем id материала
                        XMLItemAttribute = item.Attributes["MaterialId"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"MaterialId\"");

                        int MaterialId = -1;
                        try
                        {
                            MaterialId = Int32.Parse(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"MaterialId\": " + ex.Message);
                        }

                        int MaterialIndex = materials.GetIndexById(MaterialId);
                        if (MaterialIndex == -1)
                            throw new Exception(item.Name + ": материал " + XMLItemAttribute.Value + " не существует!");

                        // считываем id заказа
                        XMLItemAttribute = item.Attributes["OrderId"];
                        if (XMLItemAttribute == null)
                            throw new Exception("Узел \"ProductItem\" не содержит обязательный атрибут \"OrderId\"");

                        int OrderId = -1;
                        try
                        {
                            OrderId = Int32.Parse(XMLItemAttribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("\"OrderId\": " + ex.Message);
                        }

                        int OrderIndex = orders.GetIndexById(OrderId);
                        if (OrderIndex == -1)
                            throw new Exception(item.Name + ": заказ " + XMLItemAttribute.Value + " не существует!");

                        tasks.Add(new TaskItem(ItemId, "", MaterialId, ProductionId, ProductionSize, OrderId));
                    }
                    else throw new Exception("Не поддерживаемый дочерний узел: \"" + item.Name + "\" для узла Tasks. Допустимые узлы: \"TaskItem\".");
                }
            }
        }

        /// <summary>
        /// Данный метод загружает конфигурацию из файла.
        /// </summary>
        /// <param name="FileName">Путь к файлу.</param>
        public void LoadFromFile(string FileName)
        {
            // загружаем XML документ
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(System.IO.File.ReadAllText(FileName));

            //
            // Не изменяете порядок чтения ресурсов.!
            //
            XmlNodeList l = Doc.GetElementsByTagName("ProductionScheduleConfiguration");
            if (l.Count == 0)
                throw new Exception("Неверный формат файла.");

            XmlNode XMLItemAttribute = l[0].Attributes["BaseTime"];
            if (XMLItemAttribute == null)
                throw new Exception("Узел \"ProductionScheduleConfiguration\" не имеет обязательного атрибута \"BaseTime\"");

            baseTime = StringToDateTime(XMLItemAttribute.Value);


            XMLItemAttribute = l[0].Attributes["Version"];
            if (XMLItemAttribute == null)
                throw new Exception("Узел \"ProductionScheduleConfiguration\" не имеет обязательного атрибута \"Version\"");

            int FileVersion = -1;
            bool bVersionRead = true;
            try
            {
                FileVersion = Convert.ToInt32(XMLItemAttribute.Value);
            }
            catch(Exception)
            {
                // помечаем о неудаче считывания версии
                bVersionRead = false;
            }

            string ErrorMsg = "Данный тип проекта не поддерживается!";
            if (!bVersionRead) // удаётся ли считать версию проекта
            {
                throw new Exception(ErrorMsg);
            }

            if (FileVersion != version) // совпадает ли версия проекта
            {
                throw new Exception(ErrorMsg);
            }

            // получаем список доступных материалов
            LoadMaterials(Doc.GetElementsByTagName("Materials"));

            // получаем список доступных устройств
            LoadDevices(Doc.GetElementsByTagName("Devices"));

            // получаем список доступной продукции
            LoadProducts(Doc.GetElementsByTagName("Productions"));

            // получаем список заказчиков
            LoadCustomers(Doc.GetElementsByTagName("Customers"));

            // получаем список заказов
            LoadOrders(Doc.GetElementsByTagName("Orders"));

            // получаем список заданий
            LoadTasks(Doc.GetElementsByTagName("Tasks"));
        }

        public void WriteToFile(string FileName)
        {
            // создаём файловый поток
            FileStream fs = new FileStream(FileName, FileMode.Create);
            // создаём управляющий пишуший механизм
            XmlTextWriter xmlOut = new XmlTextWriter(fs, System.Text.Encoding.Unicode);

            // устанавливаем форматирование для более наглядного представления
            xmlOut.Formatting = Formatting.Indented;

            xmlOut.WriteStartDocument();
            xmlOut.WriteComment("                  !!! ВНИМАНИЕ !!!                ");
            xmlOut.WriteComment(" Этот файл должен быть сохранен в формате Unicode ");

            xmlOut.WriteStartElement("ProductionScheduleConfiguration");
            xmlOut.WriteAttributeString("BaseTime", DateToString(baseTime));
            xmlOut.WriteAttributeString("Version", version.ToString());

            // записываем материалы
            {
                xmlOut.WriteStartElement("Materials");
                for (int i = 0; i < materials.Count; i++)
                {
                    xmlOut.WriteStartElement("MaterialItem");

                    xmlOut.WriteAttributeString("id", materials[i].Id.ToString());
                    xmlOut.WriteAttributeString("Name", materials[i].Text);
                    xmlOut.WriteAttributeString("SawingTime", materials[i].SawingTime.ToString());
                    xmlOut.WriteAttributeString("PolishingTime", materials[i].PolishingTime.ToString());
                    xmlOut.WriteString(materials[i].Description);

                    xmlOut.WriteEndElement();
                }
                xmlOut.WriteEndElement();
            }

            // записываем устройства
            {
                xmlOut.WriteStartElement("Devices");
                // записываем пилы
                for (int i = 0; i < saws.Count; i++)
                {
                    xmlOut.WriteStartElement("Saw");

                    xmlOut.WriteAttributeString("id", saws[i].Id.ToString());
                    xmlOut.WriteAttributeString("Name", saws[i].Text);
                    xmlOut.WriteAttributeString("Responsible", saws[i].Responsible);
                    xmlOut.WriteAttributeString("DefaultMaterialId", saws[i].DefaultMaterialId.ToString());
                    xmlOut.WriteAttributeString("ServicePeriod", saws[i].ServiceTimePeriod.ToString());
                    xmlOut.WriteAttributeString("ServiceTime", saws[i].ServiceTime.ToString());
                    // записываем поддерживаемые насадки
                    {
                        for (int j = 0; j < saws[i].SupportedMaterials.Count; j++)
                        {
                            xmlOut.WriteStartElement("SupportedMaterial");
                            xmlOut.WriteAttributeString("MaterialId", saws[i].SupportedMaterials[j].ID.ToString());
                            xmlOut.WriteAttributeString("ConfTime", saws[i].SupportedMaterials[j].Time.ToString());
                            xmlOut.WriteEndElement();
                        }
                    }

                    xmlOut.WriteEndElement(); // Saw
                }

                // записываем шлифовальные станки
                for (int i = 0; i < grinders.Count; i++)
                {
                    xmlOut.WriteStartElement("Grinder");

                    xmlOut.WriteAttributeString("id", grinders[i].Id.ToString());
                    xmlOut.WriteAttributeString("Name", grinders[i].Text);
                    xmlOut.WriteAttributeString("Responsible", grinders[i].Responsible);
                    xmlOut.WriteAttributeString("DefaultMaterialId", grinders[i].DefaultMaterialId.ToString());
                    xmlOut.WriteAttributeString("ServicePeriod", grinders[i].ServiceTimePeriod.ToString());
                    xmlOut.WriteAttributeString("ServiceTime", grinders[i].ServiceTime.ToString());
                    // записываем поддерживаемые насадки
                    {
                        for (int j = 0; j < grinders[i].SupportedMaterials.Count; j++)
                        {
                            xmlOut.WriteStartElement("SupportedMaterial");
                            xmlOut.WriteAttributeString("MaterialId", grinders[i].SupportedMaterials[j].ID.ToString());
                            xmlOut.WriteAttributeString("ConfTime", grinders[i].SupportedMaterials[j].Time.ToString());
                            xmlOut.WriteEndElement(); // SupportedMaterial
                        }
                    }

                    xmlOut.WriteEndElement(); // Saw
                }

                xmlOut.WriteEndElement(); // Devices
            }

            // записываем доступную продукцию
            {
                xmlOut.WriteStartElement("Productions");

                for (int i = 0; i < productions.Count; i++)
                {
                    xmlOut.WriteStartElement("ProductItem");

                    xmlOut.WriteAttributeString("id", productions[i].Id.ToString());
                    xmlOut.WriteAttributeString("Name", productions[i].Text);

                    for (int j = 0; j < productions[i].SupSizes.Count; j++)
                    {
                        xmlOut.WriteStartElement("Size");
                        xmlOut.WriteAttributeString("Index", productions[i].SupSizes[j].Index.ToString());
                        xmlOut.WriteAttributeString("Length", productions[i].SupSizes[j].Length.ToString());
                        xmlOut.WriteAttributeString("Width", productions[i].SupSizes[j].Width.ToString());
                        xmlOut.WriteAttributeString("Height", productions[i].SupSizes[j].Height.ToString());
                        xmlOut.WriteEndElement();
                    }

                    xmlOut.WriteEndElement();
                }

                xmlOut.WriteEndElement();
            }

            // записываем заказчиков
            {
                xmlOut.WriteStartElement("Customers");
                for (int i = 0; i < customers.Count; i++)
                {
                    xmlOut.WriteStartElement("Customer");

                    xmlOut.WriteAttributeString("id", customers[i].Id.ToString());
                    xmlOut.WriteAttributeString("Name", customers[i].Text);
                    xmlOut.WriteAttributeString("Phone", customers[i].Phone);
                    xmlOut.WriteAttributeString("Address", customers[i].Address);

                    xmlOut.WriteEndElement();
                }

                xmlOut.WriteEndElement(); 
            }

            // записываем заказы
            {
                xmlOut.WriteStartElement("Orders");
                for (int i = 0; i < orders.Count; i++)
                {
                    xmlOut.WriteStartElement("Order");

                    xmlOut.WriteAttributeString("Index", orders[i].Id.ToString());
                    xmlOut.WriteAttributeString("CustomerId", orders[i].CustomerId.ToString());
                    xmlOut.WriteAttributeString("Date", DateToString(orders[i].Date));

                    if (orders[i].DeadLine != null)
                        xmlOut.WriteAttributeString("DeadLine", DateToString(orders[i].DeadLine));

                    xmlOut.WriteEndElement();
                }

                xmlOut.WriteEndElement();
            }

            // записываем задания
            {
                xmlOut.WriteStartElement("Tasks");
                for (int i = 0; i < tasks.Count; i++)
                {
                    xmlOut.WriteStartElement("TaskItem");

                    xmlOut.WriteAttributeString("id", tasks[i].Id.ToString());
                    xmlOut.WriteAttributeString("Name", tasks[i].Text);
                    xmlOut.WriteAttributeString("ProductionId", tasks[i].ProductionId.ToString());
                    xmlOut.WriteAttributeString("SizeIndex", tasks[i].SizeIndex.ToString());
                    xmlOut.WriteAttributeString("OrderId", tasks[i].OrderId.ToString());
                    xmlOut.WriteAttributeString("MaterialId", tasks[i].MaterialId.ToString());

                    xmlOut.WriteEndElement();
                }

                xmlOut.WriteEndElement();
            }

            xmlOut.WriteEndElement(); // ProductionScheduleConfiguration

            xmlOut.Close();
            fs.Close();
        }

        public static string DateToString(DateTime? dt)
        {
            if (dt == null)
                return "";
            return dt.Value.Day.ToString("00") +
            "." + dt.Value.Month.ToString("00") +
            "." + dt.Value.Year.ToString("0000") +
            " " + dt.Value.Hour.ToString("00") +
            ":" + dt.Value.Minute.ToString("00");
        }

        public static DateTime StringToDateTime(string Str)
        {
            if (Str.Length != 16)
                throw new Exception("Входная строка времени имела неверный формат.");
            int Day = Convert.ToInt32(Str.Substring(0, 2));
            int Mouth = Convert.ToInt32(Str.Substring(3, 2));
            int Year = Convert.ToInt32(Str.Substring(6, 4));
            int Hour = Convert.ToInt32(Str.Substring(11, 2));
            int Minute = Convert.ToInt32(Str.Substring(14, 2));
            return new DateTime(Year, Mouth, Day, Hour, Minute, 0); ;
        }

        /// <summary>
        /// Данный метод удаляет разделительные знаки в начале и в конце строки.
        /// </summary>
        /// <param name="value">Входная строка.</param>
        /// <returns>Обрезанная строка.</returns>
        private string DeleteWhiteSpace(string value)
        {
            // удаляем символы в начале строки
            {
                int i;
                for (i = 0; i < value.Length; i++)
                {
                    if (value[i] != '\r' && value[i] != '\n' && value[i] != '\t' && value[i] != ' ')
                        break;
                }
                value = value.Remove(0, i);
            }
            // удаляем символы в конце строки
            {
                int i;
                for (i = value.Length - 1; i >= 0; i--)
                {
                    if (value[i] != '\r' && value[i] != '\n' && value[i] != '\t' && value[i] != ' ')
                        break;
                }
                value = value.Remove(i + 1, value.Length - i - 1);
            }
            return value;
        }

        public object Clone()
        {
            Configuration clone = new Configuration();

            clone.baseTime = baseTime;
            clone.materials = (MaterialList)materials.Clone();
            clone.saws = (DeviceList)saws.Clone();
            clone.grinders = (DeviceList)grinders.Clone();
            clone.productions = (ProductionList)productions.Clone();
            clone.customers = (CustomerList)customers.Clone();
            clone.orders = (OrderList)orders.Clone();
            clone.tasks = (TaskList)tasks.Clone();

            return clone;
        }

        public void Assign(Configuration Obj)
        {
            baseTime = Obj.baseTime;
            materials = (MaterialList)Obj.materials.Clone();
            saws = (DeviceList)Obj.saws.Clone();
            grinders = (DeviceList)Obj.grinders.Clone();
            productions = (ProductionList)Obj.productions.Clone();
            customers = (CustomerList)Obj.customers.Clone();
            orders = (OrderList)Obj.orders.Clone();
            tasks = (TaskList)Obj.tasks.Clone();
        }
    }
}
