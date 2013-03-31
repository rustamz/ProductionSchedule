using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public class MaterialItem : BaseScheduleItem
    {
        /// <summary>
        /// Краткое описание материала.
        /// </summary>
        private string description;

        /// <summary>
        /// Время, необходимое для распиливания единицы продукции в минутах.
        /// </summary>
        private int sawingTime;


        /// <summary>
        /// Время, необходимое для шлифовки единицы продукции в минутах.
        /// </summary>
        private int polishingTime;

        /// <summary>
        /// Возвращает или задаёт описание материала.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Возвращает или задаёт время, необходимое для распиливания единицы продукции в минутах.
        /// </summary>
        public int SawingTime
        {
            get { return sawingTime; }
            set { sawingTime = value; }
        }

        /// <summary>
        /// Возвращает или задаёт время, необходимое для шлифовки единицы продукции в минутах.
        /// </summary>
        public int PolishingTime
        {
            get { return polishingTime; }
            set { polishingTime = value; }
        }


        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        /// <param name="Text">Название материла.</param>
        /// <param name="Description">Описание материала.</param>
        public MaterialItem(string Text, string Description, int SawingTime, int PolishingTime)
            : base(Text)
        {
            description = Description;
            sawingTime = SawingTime;
            polishingTime = PolishingTime;
        }

        /// <summary>
        /// Инициализация параметров по умолчанию.
        /// </summary>
        /// <param name="Id">Цникальный код материала</param>
        /// <param name="Text">Название материла.</param>
        /// <param name="Description">Описание материала.</param>
        public MaterialItem(int Id, string Text, string Description, int SawingTime, int PolishingTime)
            : base(Id,Text)
        {
            description = Description;
            sawingTime = SawingTime;
            polishingTime = PolishingTime;
        }

        /// <summary>
        /// Создаёт и возвращает копию объекта.
        /// </summary>
        /// <returns>Копия объекта.</returns>
        public new object Clone()
        {
            return new MaterialItem(id, text, description, sawingTime, polishingTime);
        }

        public override string ToString()
        {
            return text;
        }
    }
}
