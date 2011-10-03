using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public delegate void EnableNewSample();

    public class OutPin<DataType>: Pin
    {
        private volatile bool m_Stopped;
        private IAppletStream<DataType> m_Stream;
        private List<InPin<DataType>> m_ConnectedPins;

        public OutPin(string PinName, Applet applet): base(PinName, applet)
        {
            m_ConnectedPins = new List<InPin<DataType>>();
            m_Stream = new AppletStream<DataType>();
            m_Stopped = false;
        }

<<<<<<< HEAD
        /// <summary>
        /// Функция подключения с учетом типизации пинов
        /// </summary>
        /// <param name="ConnectedPin"></param>
        private void Connect(InPin<DataType> ConnectedPin)
        {
            if (ConnectedPin.IsConnect)
                throw new ConnectException("This input pin is already connected.");

            m_ConnectedPins.Add(ConnectedPin);
            //ConnectedPin.Connect(this, m_Stream);
        }

=======
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
        public override bool IsConnect
        {
            get { return m_ConnectedPins.Count > 0; }
        }

        public override void Connect(Pin pin)
        {
            base.Connect(pin);

            try
            {
                InPin<DataType> inpin = pin as InPin<DataType>;
                Connect(inpin);
            }
<<<<<<< HEAD
            catch (ConnectException cex)
            {
                throw cex;
            }
=======
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
            catch
            {
                throw new ConnectException("This pins cannot connect");
            }
        }

<<<<<<< HEAD
        public override bool Connected(Pin pin)
        {
            InPin<DataType> inpin;
            try
            {
                inpin = pin as InPin<DataType>;
            }
            catch
            {
                return false;
            }

            return m_ConnectedPins.Contains(inpin);
=======
        private void Connect(InPin<DataType> ConnectedPin)
        {
            if (ConnectedPin.IsConnect)
                throw new ConnectException("This input pin is already connected.");

            m_ConnectedPins.Add(ConnectedPin);
            //ConnectedPin.Connect(this, m_Stream);
        }

        public override bool Connected(Pin pin)
        {
            return m_ConnectedPins.Contains(pin as InPin<DataType>);
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
        }

        public void Disconnect(InPin<DataType> Pin)
        {
            if (!m_ConnectedPins.Contains(Pin))
                throw new ConnectException("This pin does not connected with this output pin");

            m_ConnectedPins.Remove(Pin);
            //Pin.Disconnect();
        }

        public override void Disconnect()
        {
            m_Stopped = true;
            //отсоединяем подключенные пины
            foreach (InPin<DataType> pin in m_ConnectedPins)
                pin.Disconnect();
            //и удаляем их из списка
            m_ConnectedPins.Clear();
        }

        /// <summary>
        /// Пропустить новый отсчет в пин
        /// </summary>
        /// <param name="sample">Новый отсчет</param>
        public void Push(DataType sample)
        {
            if (!m_Stopped)
            {
                m_Stream.Write(sample);

                if (OnNewSampleEnable != null)
                    OnNewSampleEnable();
            }
        }

<<<<<<< HEAD
        internal IAppletStream<DataType> BaseStream
=======
        public IAppletStream<DataType> BaseStream
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
        {
            get { return m_Stream; }
        }

        /// <summary>
        /// Это событие генерируется в том же потоке,
        /// в котором работает выходной пин.
        /// При подписке на это событие не следует выполнять
        /// в обработчике длительных операций.
        /// </summary>
<<<<<<< HEAD
        internal event EnableNewSample OnNewSampleEnable;
=======
        public event EnableNewSample OnNewSampleEnable;
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
    }
}
