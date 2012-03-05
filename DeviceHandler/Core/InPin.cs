using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public class InPin<DataType>
    {
        private OutPin<DataType> m_ConnectedPin;

        public IAppletHandler<DataType> AppletHandler
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsConnect
        {
            get { return m_ConnectedPin != null; }
        }

        public void Connect(OutPin<DataType> ConnectedPin)
        {
            if (IsConnect)
                throw new Exception("This input pin is already connected.");

            m_ConnectedPin = ConnectedPin;
        }

        public void Disconnect()
        {
            m_ConnectedPin = null;
        }

        public AppletStream<DataType> BaseStream
        {
            get { throw new NotImplementedException(); }
        }
    }
}
