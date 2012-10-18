using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DeviceHandler
{
    public abstract class Pin
    {
        /// <summary>
        /// Имя пина. Имя должно быть уникальным в пределах
        /// одного апплета
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Пин подключен
        /// </summary>
        public abstract bool IsConnect { get; }

        /// <summary>
        /// Конструктор пина
        /// </summary>
        /// <param name="PinName">Имя пина. Оно должно быть уникальным впределах одного апплета</param>
        /// <param name="applet">Апплет, которому принадлежит пин</param>
        public Pin(string PinName, Applet applet)
        {
            Name = PinName;
        }

        /// <summary>
        /// Присоединить пины.
        /// Выполняет базовые проверки при подключении.
        /// </summary>
        /// <param name="pin"></param>
        public virtual void Connect(Pin pin)
        {
            Type this_data_type = this.GetType().GetGenericArguments()[0];
            Type pin_data_type = pin.GetType().GetGenericArguments()[0];

            if (this_data_type != pin_data_type)
                throw new ConnectException("Pins transmitted different data types");
        }

        /// <summary>
        /// Проверяет соединены ли пины между собой
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public abstract bool Connected(Pin pin);

        /// <summary>
        /// Отсоединить пины
        /// </summary>
        public abstract void Disconnect();
    }

    public class ConnectException : Exception
    {
        public ConnectException(string message)
            : base(message)
        {
        }
    }
}
