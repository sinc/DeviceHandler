using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    class AppletStreamReader<DataType>: IAppletStreamReader<DataType>
    {
        private DataType[] m_InternalBuf;
        private int m_TimeOut;
        private volatile bool m_Closed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <param name="TimeOut">Время ожидания чтения в мс</param>
        public AppletStreamReader(IAppletStream<DataType> stream, int TimeOut)
        {
            if (stream != null)
                Stream = stream;
            else
                throw new Exception("Stream cannot be null");

            if (TimeOut > 1)
                m_TimeOut = TimeOut;
            else
                throw new Exception("Time out must be great 0");
            
            m_InternalBuf = new DataType[1];
            m_Closed = false;
        }

        #region IAppletStreamReader<DataType> Members
        
        public int Position { get; set; }
        public IAppletStream<DataType> Stream { get; protected set; }

        public DataType Read()
        {
            if (!m_Closed)
            {
                if (Stream.Read(m_InternalBuf, Position++, 0, 1, m_TimeOut) > 0)
                    return m_InternalBuf[0];
            }
            return default(DataType);
        }

        public int Read(DataType[] buf, int index, int Count)
        {
            if (m_Closed)
                return 0;

            return Stream.Read(buf, Position++, index, Count, m_TimeOut);
        }

        public void Close()
        {
            m_Closed = true;
            Stream = null;
        }

        #endregion
    }
}
