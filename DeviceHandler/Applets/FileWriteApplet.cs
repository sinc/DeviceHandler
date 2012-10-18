using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DeviceHandler
{
    public class FileWriteApplet<T>: Applet
    {
        private StreamWriter m_outputStream;
        public FileWriteApplet(GraphBuilder gb, StreamWriter outputStream): base(gb)
        {
            m_outputStream = outputStream;
            InPin<T> inPin = RegisterInputPin<T>(1, 1);
            inPin.OnNewDataEnabled += new NewDataEnabled<T>(inPin_OnNewDataEnabled);
        }

        private void inPin_OnNewDataEnabled(IEnumerable<T> data)
        {
            foreach (T obj in data)
            {
                m_outputStream.WriteLine(obj.ToString());
            }
        }
    }
}
