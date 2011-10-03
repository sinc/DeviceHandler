using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public abstract class Applet: IDisposable
    {
        protected List<Pin> m_InputPins;
        protected List<Pin> m_OutputPins;

        public Applet(GraphBuilder graph_builder)
        {
            graph_builder.RegisterApplet(this);

            m_InputPins = new List<Pin>();
            m_OutputPins = new List<Pin>();
        }

        /// <summary>
        /// Чтобы пины были доступны для подключения GraphBuilder`ом
        /// их необходимо зарегистрировать. По умолчанию у каждого
        /// апплета зарегистрированы входной и выходной пины.
        /// </summary>
        /// <typeparam name="T">Тип данных пина</typeparam>
        /// <param name="shift">Сдвиг после чтения пачки данных</param>
        /// <param name="count">Длина пачки данных</param>
        public InPin<T> RegisterInputPin<T>(string PinName, int shift, int count)
        {
            InPin<T> pin = new InPin<T>(PinName, shift, count, this);

            m_InputPins.Add(pin);

            return pin;
        }

        /// <summary>
        /// Чтобы пины были доступны для подключения GraphBuilder`ом
        /// их необходимо зарегистрировать. По умолчанию у каждого
        /// апплета зарегистрированы входной и выходной пины.
        /// </summary>
        /// <typeparam name="T">Тип данных пина</typeparam>
        /// <param name="pin">регистрируемый вsходной пин</param>
        public OutPin<T> RegisterOutputPin<T>(string PinName)
        {
            OutPin<T> pin = new OutPin<T>(PinName, this);

            m_OutputPins.Add(pin);

            return pin;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (Pin pin in m_InputPins)
                pin.Disconnect();

            foreach (Pin pin in m_OutputPins)
                pin.Disconnect();
        }

        #endregion

        public virtual IEnumerable<Pin> InputPins
        {
            get { return m_InputPins.ToArray(); }
        }

        public virtual IEnumerable<Pin> OutputPins
        {
            get { return m_OutputPins.ToArray(); }
        }
    }
}
