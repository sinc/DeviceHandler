using System;
using System.Collections.Generic;
using System.Linq;
using DeviceHandler;
using System.Drawing;

namespace adnsWatcher
{
    public class AdnsReader: Applet
    {
        private const int kWidth = 30;
        private const int kHeight = 30;

        private OutPin<Bitmap> m_outPin;
        private Bitmap m_bitmap;
        private bool m_bitmapStarted = false;
        private int m_x, m_y;

        public AdnsReader(GraphBuilder gb): base(gb)
        {
            InPin<byte[]> inputPin = RegisterInputPin<byte[]>(1, 1);
            m_outPin = RegisterOutputPin<Bitmap>();

            inputPin.OnNewDataEnabled += new NewDataEnabled<byte[]>(AppletEngine);
        }

        protected void AppletEngine(IEnumerable<byte[]> data)
        {
            foreach (byte[] doubleBuff in data)
            {
                int len = doubleBuff.Length;

                for (int i = 0; i < len; i++)
                {
                    if ((doubleBuff[i] & 0x40) > 0)
                    {
                        m_bitmapStarted = true;
                        m_bitmap = new Bitmap(kWidth * 5, kHeight * 5);
                        m_x = 0;
                        m_y = 0;
                    }

                    if (m_bitmapStarted)
                    {
                        int color = doubleBuff[i] * 4;
                        if (color > 255)
                            color = 255;
                        Graphics g = Graphics.FromImage(m_bitmap);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(color, color, color)),
                            new Rectangle(m_x * 5, m_y * 5, 5, 5));

                        if (++m_x >= kWidth)
                        {
                            m_x = 0;
                            if (++m_y >= kHeight)
                            {
                                m_bitmapStarted = false;
                                m_outPin.Push(m_bitmap);
                            }
                        }
                    }
                }
            }
        }
    }
}
