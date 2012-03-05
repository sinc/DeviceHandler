using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public class OutPin<DataType>
    {
        //private IAppletHandler<DataType> m_AppHandler;
        private AppletStream<DataType> m_Stream;
        private List<InPin<DataType>> m_ConnectedPins;

        public OutPin(IAppletHandler<DataType> handler)
        {
            //m_AppHandler = handler;
            m_ConnectedPins = new List<InPin<DataType>>();
            m_Stream = new AppletStream<DataType>();
        }

        public bool IsConnect
        {
            get { return m_ConnectedPins.Count > 0; }
        }

        /// <summary>
        /// Присоединеие к выходному пину входного.
        /// 
        /// <example>
        /// Пример:
        ///     InPin<float> in_pin = new InPin();
        ///     OutPin<float> out_pin = new OutPin();
        ///     out_pin.Connect(in_pin);
        ///     in_pin.Connect(out_pin);
        /// </example>
        /// </summary>
        /// <param name="ConnectedPin"></param>
        /// <returns></returns>
        public void Connect(InPin<DataType> ConnectedPin)
        {
            if (ConnectedPin.IsConnect)
                throw new Exception("This input pin is already connected.");

            m_ConnectedPins.Add(ConnectedPin);
            //pin.Connect(this);
        }

        public void Disconnect(InPin<DataType> Pin)
        {
            if (m_ConnectedPins.Contains(Pin))
                throw new Exception("This pin does not connected with this output pin");

            m_ConnectedPins.Remove(Pin);
            Pin.Disconnect();
        }

        public void Disconnect()
        {
            m_ConnectedPins.Clear();
        }

        public AppletStream<DataType> BaseStream
        {
            get { return m_Stream; }
        }
    }
}
