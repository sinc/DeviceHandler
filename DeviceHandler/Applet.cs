using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
<<<<<<< HEAD
=======
    //Примерчик работы:
    //class myApplet : Applet<float, float>
    //{
    //    public myApplet()
    //        : base(5, 1)
    //    {
    //    }

    //    public override void AppletEngine(IEnumerable<float> data)
    //    {
    //        float aver = 0.0f;
    //        foreach (float f in data)
    //            aver += f;
    //        aver /= 5.0;
    //        OutputPin.Push(aver);
    //    }
    //}
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
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
<<<<<<< HEAD
        /// <param name="shift">Сдвиг после чтения пачки данных</param>
        /// <param name="count">Длина пачки данных</param>
=======
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
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

<<<<<<< HEAD
        public virtual IEnumerable<Pin> InputPins
=======
        public IEnumerable<Pin> InputPins
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
        {
            get { return m_InputPins.ToArray(); }
        }

<<<<<<< HEAD
        public virtual IEnumerable<Pin> OutputPins
=======
        public IEnumerable<Pin> OutputPins
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
        {
            get { return m_OutputPins.ToArray(); }
        }
    }
}
