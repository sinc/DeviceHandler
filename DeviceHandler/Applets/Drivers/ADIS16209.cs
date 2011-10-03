using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceHandler
{
    public class Accelerate
    {
        public float Ax { get; private set; }
        public float Ay { get; private set; }
        public float Az { get; private set; }
        public float TimeStamp { get; private set; }
    }

    public class ADIS16209: Applet
    {
        private OutPin<float[]> m_InclOut;
        private OutPin<Accelerate> m_AcclOut;

        public ADIS16209(GraphBuilder gb)
            : base(gb)
        {
            m_InclOut = RegisterOutputPin<float[]>("MainOutputPin");    //inclinometer
            m_AcclOut = RegisterOutputPin<Accelerate>("AccelOutPin");
        }
    }
}
