using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduleCore
{
    public interface IBaseScheduleItem : ICloneable
    {
        int Id
        {
            get;
            set;
        }

        string Text
        {
            get;
            set;
        }
    }
    
    public class BaseScheduleItem : IBaseScheduleItem
    {
        protected int id;
        protected string text;

        /// <summary>
        /// Возвращает или устанавливает идентификатор объекта.
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Возвращает или устанавливает связанный с объектом текст.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        /// <param name="Id">Идентификатор объекта</param>
        /// <param name="Text">Связанный с объектом текст</param>
        public BaseScheduleItem(int Id, string Text)
        {
            id = Id;
            text = Text;
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        /// <param name="Id">Идентификатор объекта</param>
        public BaseScheduleItem(int Id)
        {
            id = Id;
            text = "";
        }

        /// <summary>
        /// Инициализация объекта по умолчанию.
        /// </summary>
        /// <param name="Text">Связанный с объектом текст</param>
        public BaseScheduleItem(string Text)
        {
            id = Id;
            text = Text;
        }

        public Object Clone()
        {
            return new BaseScheduleItem(id, text);
        }
    }
}
