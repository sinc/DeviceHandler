using System;
using System.Collections.Generic;
using System.Linq;
using DeviceHandler;
using System.Drawing;

namespace adnsWatcher
{
    public class Printer: SimpleApplet<Bitmap, int>
    {
        private Graphics m_DestGraphics;

        public Printer(GraphBuilder gb, Graphics g): base(1, 1, gb)
        {
            m_DestGraphics = g;
        }

        protected override int AppletEngine(IEnumerable<Bitmap> data)
        {
            //interlock?
            foreach (Bitmap bmp in data)
            {
                m_DestGraphics.DrawImage(bmp, new Point(20, 20));
            }
            return 0;
        }
    }
}
