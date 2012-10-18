using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceHandler
{
    public class MagnetizationRestoreApplet: SimpleApplet<CHDSample, double[]>
    {
        /// <summary>
        /// временно заменено на константы.
        /// они зависят от геометрии датчика
        /// </summary>
        private const double sumP = 0.253817;
        private const double sumW = 2.4491;
        private double m_prevHz = 0;
        private bool m_initialized = false;

        public MagnetizationRestoreApplet(GraphBuilder gb)
            : base(1, 1, gb)
        { }

        protected override double[] AppletEngine(IEnumerable<CHDSample> data)
        {
            CHDSample sample = data.First();
            double Hx = (2.0 * sample.hall[0] - sample.hall[1] - sample.hall[2]) / 3.0 / sample.dx;
            double Hy = (sample.hall[1] - sample.hall[2]) / Math.Sqrt(3.0) / sample.dx;
            if (m_initialized)
            {
                m_prevHz += (sample.hall[0] + sample.hall[1] + sample.hall[2]) / 3.0;
            }
            else
            {
                m_prevHz = 2.0 * (sample.hall[0] + sample.hall[1] + sample.hall[2]) / 3.0;
            }            
            return new double[] { Hx / sumP, Hy / sumP, m_prevHz / sumW };
        }
    }
}
