using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public class AppletStreamReader<DataType>: IAppletStreamReader<DataType>
    {
        private DataType[] m_InternalBuf;
        private int m_TimeOut;

        public int Position { get; set; }
        public IAppletStream<DataType> Stream { get; protected set; }

        public AppletStreamReader(AppletStream<DataType> stream, int TimeOut)
        {
            Stream = stream;
            m_InternalBuf = new DataType[1];
            m_TimeOut = TimeOut;
        }

        public DataType Read()
        {
            if (Stream.Read(m_InternalBuf, Position++, 0, 1, m_TimeOut) > 0)
                return m_InternalBuf[0];

            return null;
        }

        public int Read(DataType[] buf, int index, int Count)
        {
            return Stream.Read(buf, Position++, index, Count, m_TimeOut);
        }
    }
}
