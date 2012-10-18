using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTD2XX_NET;
using System.Threading;

namespace DeviceHandler
{
    public class FTDIBulkDriver: Applet
    {
        private FTDI m_device;
        private OutPin<byte[]> m_outputPin;
        private Thread m_readThread;
        private volatile bool m_isRunning = false;

        public FTDIBulkDriver(GraphBuilder gb, FTDI device, string deviceDescriptor, uint baudRate)
            : base(gb)
        {
            FTDI.FT_STATUS fs;
            if (device != null)
            {
                m_device = device;
                if ((fs = device.OpenBySerialNumber(deviceDescriptor)) != FTDI.FT_STATUS.FT_OK)
                    throw new Exception(string.Format("Невозможно открыть устройство. Устройство не готово. ({0})", fs));
                if ((fs = device.SetBaudRate(baudRate)) != FTDI.FT_STATUS.FT_OK)
                    throw new Exception(string.Format("Устройство не поддерживает заданную скорость ({0})", fs));
                m_outputPin = RegisterOutputPin<byte[]>("MainOutPin");
            }
        }

        private bool waitBytes(int count)
        {
            uint enableBytes = 0;
            System.Diagnostics.Stopwatch watcher = new System.Diagnostics.Stopwatch();
            watcher.Start();
            while (enableBytes < count)
            {
                if (watcher.ElapsedMilliseconds < 5000)
                    m_device.GetRxBytesAvailable(ref enableBytes);
                else
                    return false;
            }
            return true;
        }

        public void start(int packetLength)
        {
            if (!m_isRunning)
            {
                m_isRunning = true;
                m_readThread = new Thread(new ThreadStart(
                    delegate()
                    {
                        byte[] buf = new byte[packetLength];
                        uint readingBytes = 0;
                        while (m_isRunning)
                        {
                            while (!waitBytes(packetLength)) ;
                            m_device.Read(buf, (uint)packetLength, ref readingBytes);
                            m_outputPin.Push(buf);
                        }
                    }));
                m_readThread.IsBackground = true;
                m_readThread.Start();
            }
        }

        public void stop()
        {
            if (m_isRunning && m_readThread != null)
            {
                m_isRunning = false;
                m_readThread.Join(100);
                m_readThread = null;
            }
        }
    }
}
