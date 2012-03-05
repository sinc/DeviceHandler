using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DeviceHandler
{
    public class AppletStream<DataType> : IAppletStream<DataType>
    {
        private ReaderWriterLockSlim m_Locker;
        private List<DataType> m_DataList;

        public AppletStream()
        {
            m_DataList = new List<DataType>();
            m_Locker = new ReaderWriterLockSlim();
        }

        #region IAppletStream<DataType> Members

        public int Length
        {
            get { return m_DataList.Count; }
        }

        public void Write(DataType sample)
        {
            m_Locker.EnterWriteLock();

            try
            {
                m_DataList.Add(sample);
            }
            finally
            {
                m_Locker.ExitWriteLock();
            }
        }

        public int Read(DataType[] buf, int pos, int index, int count, int TimeOut)
        {
            int ReadBytes = 0;

            if (m_Locker.TryEnterReadLock(TimeOut))
            {
                try
                {
                    if (pos >= m_DataList.Count)
                        return 0;

                    ReadBytes = Length - pos;
                    ReadBytes = (ReadBytes < count) ? ReadBytes : count;

                    Array.Copy(m_DataList.ToArray(), pos, buf, index, ReadBytes);
                }
                finally
                {
                    m_Locker.ExitReadLock();
                }
            }

            return ReadBytes;
        }

        #endregion
    }
}
