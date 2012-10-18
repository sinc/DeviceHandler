using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceHandler
{
    public class AllanDetectorApplet: Applet
    {
        private int m_windowLength;     //L
        private int m_subWindowLength;  //MAlan
        private int m_averageRange;     //итервал усреднения KAlan
        private int m_count = 0;
        private double m_threshold;
        private double m_G1 = 0;
        private double m_G2 = 0;
        private double m_G3 = 0;
        private double[] c1, c2, c3;
        private double[] f1, f2, f3;
        private CHDSample[] m_samples;
        private List<CHDSample> m_current;
        private OutPin<CHDSample[]> m_outputPin;
        private bool m_started = false;

        //TODO: подобрать оптимальные начальные параметры!
        public AllanDetectorApplet(GraphBuilder gb, double threshold)
            : this(gb, 49, 3, threshold)
        {
        }

        public AllanDetectorApplet(GraphBuilder gb, int windowLength, int subWindowLength, double threshold)
            : base(gb)
        {
            m_threshold = threshold;
            m_windowLength = windowLength;
            m_subWindowLength = subWindowLength;
            m_averageRange = m_windowLength - 2 * m_subWindowLength + 1;
            if (m_averageRange < 0)
                throw new Exception("Длина окна меньше двух интервалов усреднения");
            c1 = new double[m_subWindowLength + m_averageRange + 1];
            c2 = new double[m_subWindowLength + m_averageRange + 1];
            c3 = new double[m_subWindowLength + m_averageRange + 1];
            f1 = new double[m_subWindowLength + 1];
            f2 = new double[m_subWindowLength + 1];
            f3 = new double[m_subWindowLength + 1];
            m_samples = new CHDSample[(m_windowLength - 1) / 2 + 1];
            m_current = new List<CHDSample>();
            InPin<CHDSample> inPin = RegisterInputPin<CHDSample>(1, windowLength);
            inPin.OnNewDataEnabled += new NewDataEnabled<CHDSample>(inPin_OnNewDataEnabled);
            m_outputPin = RegisterOutputPin<CHDSample[]>();
        }

        private void inPin_OnNewDataEnabled(IEnumerable<CHDSample> data)
        {
            CHDSample sample = data.First();
            double c01 = sample.hall[0];
            double c02 = sample.hall[1];
            double c03 = sample.hall[2];
            //shift buffers
            for (int i = m_samples.Length - 1; i > 0; i--)
            {
                m_samples[i] = m_samples[i - 1];
            }
            for (int i = m_subWindowLength; i > 0 ; i--)
            {
                f1[i] = f1[i - 1];
                f2[i] = f2[i - 1];
                f3[i] = f3[i - 1];
            }
            for (int i = m_subWindowLength + m_averageRange; i > 0; i--)
            {
                c1[i] = c1[i - 1];
                c2[i] = c2[i - 1];
                c3[i] = c3[i - 1];
            }
            m_samples[0] = sample;
            f1[0] = c01;
            f2[0] = c02;
            f3[0] = c03;
            c1[0] += c01;
            c2[0] += c02;
            c3[0] += c03;
            if (m_count >= m_subWindowLength)
            {
                c1[0] -= f1[m_subWindowLength];
                c2[0] -= f2[m_subWindowLength];
                c3[0] -= f3[m_subWindowLength];
            }
            m_G1 += Math.Pow(c1[0] - c1[m_subWindowLength], 2.0) - Math.Pow(c1[m_averageRange] - c1[m_averageRange + m_subWindowLength], 2.0);
            m_G2 += Math.Pow(c2[0] - c2[m_subWindowLength], 2.0) - Math.Pow(c2[m_averageRange] - c2[m_averageRange + m_subWindowLength], 2.0);
            m_G3 += Math.Pow(c3[0] - c3[m_subWindowLength], 2.0) - Math.Pow(c3[m_averageRange] - c3[m_averageRange + m_subWindowLength], 2.0);
            if (m_count++ >= m_windowLength)
            {
                double sigma = (m_G1 + m_G2 + m_G3) / (2 * m_subWindowLength * m_subWindowLength * m_averageRange);
                if (sigma > m_threshold)
                {
                    m_current.Add(m_samples[(m_windowLength - 1) / 2]);
                    if (!m_started)
                    {
                        m_started = true;
                    }
                }
                else if (m_started)
                {
                    m_started = false;
                    if (m_current.Count >= 3)   //??
                    {
                        m_outputPin.Push(m_current.ToArray());
                    }
                    m_current.Clear();
                }
            }
        }
    }
}