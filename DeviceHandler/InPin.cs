using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DeviceHandler
{
    public delegate void NewDataEnabled<T>(IEnumerable<T> data);

    public class InPin<DataType>: Pin
    {
        private IAppletStream<DataType> m_Stream;
        private IAppletStreamReader<DataType> m_StreamReader;
        private OutPin<DataType> m_ConnectedPin;
        private int m_Shift;
        private int m_Count;
        private volatile bool m_Worked;

        public InPin(string PinName, int shift, int count, Applet applet): base(PinName, applet)
        {
            m_Shift = shift;
            m_Count = count;
            m_Worked = false;
        }

        private void Connect(OutPin<DataType> ConnectedPin)
        {
            if (IsConnect)
                throw new ConnectException("This input pin is already connected.");

            if (ConnectedPin.BaseStream != null)
            {
                m_Stream = ConnectedPin.BaseStream;
                m_StreamReader = new AppletStreamReader<DataType>(m_Stream, 1000);
            }
            else
            {
                throw new ConnectException("Applet stream is null! aaaa!!");
            }

            m_ConnectedPin = ConnectedPin;
            ConnectedPin.OnNewSampleEnable += new EnableNewSample(ConnectedPin_OnNewSampleEnable);
        }

        private void ConnectedPin_OnNewSampleEnable()
        {
            //Если поток-обработчик не работает уже
            if (m_StreamReader.Position + m_Count <= m_Stream.Length && !m_Worked)
            {
                m_Worked = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    delegate(Object obj)
                    {
                        DataType[] buf = new DataType[m_Count];
                        while (m_StreamReader.Position + m_Count <= m_Stream.Length)
                        {
                            if (OnNewDataEnabled != null)
                            {
                                m_StreamReader.Read(buf, 0, m_Shift, m_Count);
                                OnNewDataEnabled(buf);
                            }
                        }
                        m_Worked = false;
                    }));
            }
        }

        public override bool IsConnect
        {
            get { return m_ConnectedPin != null; }
        }

        public override void Connect(Pin pin)
        {
            base.Connect(pin);

            try
            {
                OutPin<DataType> outpin = pin as OutPin<DataType>;
                Connect(outpin);
            }
            catch
            {
                throw new ConnectException("This pins cannot connect");
            }
        }

        public override bool Connected(Pin pin)
        {
            return m_ConnectedPin == pin;
        }

        public override void Disconnect()
        {
            //отписываемся от события
            m_ConnectedPin.OnNewSampleEnable -= ConnectedPin_OnNewSampleEnable;
            m_ConnectedPin.Disconnect(this);
            m_ConnectedPin = null;
            m_StreamReader.Close();
        }

        /// <summary>
        /// Событие генерируется при поступлении новых отсчетов
        /// Событие возникает в новом потоке
        /// </summary>
        public event NewDataEnabled<DataType> OnNewDataEnabled; 
    }
}
