using System;
using System.Collections.Generic;
using System.Linq;
using alglib;

namespace DeviceHandler
{
    public class Dipole
    {
        private static Random m_random = new Random((int)System.Environment.TickCount);

        private double m_length;

        public double ro    { get; /*private*/ set; }   //mm
        public double betta { get; /*private*/ set; }   //rad
        public double gamma { get; /*private*/ set; }   //rad
        public double m     { get; /*private*/ set; }   //??
        public double zd    { get; /*private*/ set; }   //mm
        
        public Dipole(double l)
        {
            m_length = l;
            betta = 2.0 * Math.PI * (2.0 * m_random.NextDouble() - 1.0);
            gamma = 2.0 * Math.PI * (2.0 * m_random.NextDouble() - 1.0);
            m = 5.0 * m_random.NextDouble();
            zd = l * m_random.NextDouble();
            ro = 1.0 * (2.0 * m_random.NextDouble() - 1.0);
        }

        public double[] value(double z)
        {
            double[] ret = new double[CHDDriver.kCHDChannels];
            for (int i = 0; i < CHDDriver.kCHDChannels; i++)
            {
                double alpha = i * 2 * Math.PI / CHDDriver.kCHDChannels;
                double A1 = m * Math.Sin(betta) * Math.Cos(alpha - gamma);
                double A2 = 3 * m * (DipoleRestoreApplet.kH - ro * Math.Cos(alpha));
                double A3 = (DipoleRestoreApplet.kH * Math.Cos(alpha - gamma) - ro * Math.Cos(gamma)) * Math.Sin(betta);
                double B0 = DipoleRestoreApplet.kH * DipoleRestoreApplet.kH + ro * ro - 2 * DipoleRestoreApplet.kH * ro * Math.Cos(alpha);
                double A4 = (z - zd) * Math.Cos(betta);
                double B1 = Math.Pow((z - zd) * (z - zd) + B0, -1.5);
                double B2 = Math.Pow((z - zd) * (z - zd) + B0, -2.5);
                ret[i] = -A1 * B1 + A2 * B2 * (A3 + A4);  //отсчет модели
            }
            return ret;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", ro, betta, gamma, m, zd);
        }
    }

    public class DipoleRestoreApplet: SimpleApplet<CHDSample[], Dipole>
    {
        public const int kInitialConditions = 1000;
        public const double kdz = 0.1;     //mm
        public const double kH = 2.75;     //mm
        public const double kStep = 0.2;

        public DipoleRestoreApplet(GraphBuilder gb)
            : base(1, 1, gb)
        {
        }

        private double calcError(List<spline1d.spline1dinterpolant> ch, Dipole d, double sampleLength)
        {
            double error = 0;
            for (int i = 0; i < ch.Count; i++)
            {
                spline1d.spline1dinterpolant spl = ch[i];
                double alpha = i * 2 * Math.PI / ch.Count;
                double A1 = d.m * Math.Sin(d.betta) * Math.Cos(alpha - d.gamma);
                double A2 = 3 * d.m * (kH - d.ro * Math.Cos(alpha));
                double A3 = (kH * Math.Cos(alpha - d.gamma) - d.ro * Math.Cos(d.gamma)) * Math.Sin(d.betta);
                double B0 = kH * kH + d.ro * d.ro - 2 * kH * d.ro * Math.Cos(alpha);
                //using (System.IO.TextWriter tw = new System.IO.StreamWriter("dip.txt"))
                //{
                    for (double z = 0; z < sampleLength; z += kdz)
                    {
                        double A4 = (z - d.zd) * Math.Cos(d.betta);
                        double B1 = Math.Pow((z - d.zd) * (z - d.zd) + B0, -1.5);
                        double B2 = Math.Pow((z - d.zd) * (z - d.zd) + B0, -2.5);
                        double H = -A1 * B1 + A2 * B2 * (A3 + A4);  //отсчет модели
                        double F = spline1d.spline1dcalc(ref spl, z);
                        //tw.WriteLine("{0}\t{1}\t{2}", z, H, F);
                        error += (F - H) * (F - H);
                    }
                //}
            }
            return error;
        }
        /*
        private double calcError_(List<spline1d.spline1dinterpolant> ch, Dipole d, double sampleLength)
        {
            double error = 0;
            for (int i = 0; i < ch.Count; i++)
            {
                spline1d.spline1dinterpolant spl = ch[0];
                double alpha = i * 2 * Math.PI / ch.Count;
                double A1 = d.m * Math.Sin(d.betta) * Math.Cos(alpha - d.gamma);
                double A2 = 3 * d.m * (kH - d.ro * Math.Cos(alpha));
                double A3 = (kH * Math.Cos(alpha - d.gamma) - d.ro * Math.Cos(d.gamma)) * Math.Sin(d.betta);
                double B0 = kH * kH + d.ro * d.ro - 2 * kH * d.ro * Math.Cos(alpha);
                using (System.IO.TextWriter tw = new System.IO.StreamWriter(string.Format("dip{0}.txt", i)))
                {
                    for (double z = 0; z < sampleLength; z += kdz)
                    {
                        double A4 = (z - d.zd) * Math.Cos(d.betta);
                        double B1 = Math.Pow((z - d.zd) * (z - d.zd) + B0, -1.5);
                        double B2 = Math.Pow((z - d.zd) * (z - d.zd) + B0, -2.5);
                        double H = -A1 * B1 + A2 * B2 * (A3 + A4);  //отсчет модели
                        double F = spline1d.spline1dcalc(ref spl, z);
                        tw.WriteLine("{0}\t{1}\t{2}", z, H, F);
                        error += (F - H) * (F - H);
                    }
                }
            }
            return error;
        }
        */
        protected override Dipole AppletEngine(IEnumerable<CHDSample[]> data)
        {
            CHDSample[] sample = data.First();
            double z0 = sample[0].x;
            double[] z_ = sample.Select(s => s.x - z0).ToArray();
            List<spline1d.spline1dinterpolant> channels = new List<spline1d.spline1dinterpolant>();
            for (int i = 0; i < CHDDriver.kCHDChannels; i++)
            {
                double[] ch = sample.Select(x => x.hall[i]).ToArray();
                spline1d.spline1dinterpolant spl = new spline1d.spline1dinterpolant();
                spline1d.spline1dbuildakima(z_, ch, sample.Length, ref spl);
                channels.Add(spl);
            }
            double sampleLength = z_[z_.Length - 1];
            Dipole dip = new Dipole(sampleLength);
            double minError = calcError(channels, dip, sampleLength);
            for (int i = 1; i < kInitialConditions; i++)
            {
                Dipole d = new Dipole(sampleLength);
                double error = calcError(channels, d, sampleLength);
                if (minError > error)
                {
                    dip = d;
                    minError = error;
                }
            }
            int counter = 1000;
            while (minError > 0.05 && counter-- > 0)
            {
                minError = 0.0;
                double dEdbetta = 0;
                double dEdgamma = 0;
                double dEdm = 0;
                double dEdzd = 0;
                double dEdro = 0;
                for (int i = 0; i < channels.Count; i++)
                {
                    spline1d.spline1dinterpolant spl = channels[i];
                    double alpha = i * 2 * Math.PI / channels.Count;
                    double A1 = dip.m * Math.Sin(dip.betta) * Math.Cos(alpha - dip.gamma);
                    double A2 = 3 * dip.m * (kH - dip.ro * Math.Cos(alpha));
                    double A3 = (kH * Math.Cos(alpha - dip.gamma) - dip.ro * Math.Cos(dip.gamma)) * Math.Sin(dip.betta);
                    double B0 = kH * kH + dip.ro * dip.ro - 2 * kH * dip.ro * Math.Cos(alpha);
                    double dA1dgamma = dip.m * Math.Sin(dip.betta) * Math.Sin(alpha - dip.gamma);
                    double dA3dgamma = Math.Sin(dip.betta) * (dip.ro * Math.Sin(dip.gamma) + kH * Math.Sin(alpha - dip.gamma));
                    double dA1dbetta = dip.m * Math.Cos(alpha - dip.gamma) * Math.Cos(dip.betta);
                    double dA3dbetta = (kH * Math.Cos(alpha - dip.gamma) - dip.ro * Math.Cos(dip.gamma)) * Math.Cos(dip.betta);
                    for (double z = 0; z < sampleLength; z += kdz)
                    {
                        double A4 = (z - dip.zd) * Math.Cos(dip.betta);
                        double B1 = Math.Pow((z - dip.zd) * (z - dip.zd) + B0, -1.5);
                        double B2 = Math.Pow((z - dip.zd) * (z - dip.zd) + B0, -2.5);
                        double H = -A1 * B1 + A2 * B2 * (A3 + A4);  //отсчет модели
                        double F = spline1d.spline1dcalc(ref spl, z);
                        double dA4dbetta = (dip.zd - z) * Math.Sin(dip.betta);
                        double dB1dzd = 3 * B2 * (z - dip.zd);
                        double dB2dzd = 5 * Math.Pow(B1, 2.3333) * (z - dip.zd);
                        double dA4B2dzd = A4 * dB2dzd - B2 * Math.Cos(dip.betta);
                        double dB1dro = 3 * B2 * (kH * Math.Cos(alpha) - dip.ro);
                        double dA2B2dro = -3 * dip.m * Math.Cos(alpha) * B2 + 5 * A2 * Math.Pow(B1, 2.3333) * (kH * Math.Cos(alpha) - dip.ro);
                        double dA2B2A3dro = dA2B2dro * A3 - A2 * B2 * Math.Cos(dip.gamma) * Math.Sin(dip.betta);
                        minError += (F - H) * (F - H);
                        dEdbetta += (H - F) * (-B1 * dA1dbetta + A2 * B2 * (dA3dbetta + dA4dbetta));
                        dEdgamma += (H - F) * (-B1 * dA1dgamma + A2 * B2 * dA3dgamma);
                        dEdm += (H - F) * H / dip.m;
                        dEdzd += (H - F) * (-A1 * dB1dzd + A2 * A3 * dB2dzd + A2 * dA4B2dzd);
                        dEdro += (H - F) * (-A1 * dB1dro + A4 * dA2B2dro + dA2B2A3dro);
                    }
                    double mod = Math.Sqrt(dEdm * dEdm + dEdgamma * dEdgamma + dEdbetta * dEdbetta + dEdro * dEdro + dEdzd * dEdzd);
                    dip.ro -= kStep * dEdro / mod;
                    dip.betta -= kStep * dEdbetta / mod;
                    dip.gamma -= kStep * dEdgamma / mod;
                    dip.m -= kStep * dEdm / mod;
                    dip.zd -= kStep * dEdzd / mod;
                }
            }
            //{ m = 1.5, betta = 0.2, ro = -0.2, gamma = 0.0, zd = 90 };
            ///double e = calcError_(channels, dip, sampleLength);
            return dip;
        }
    }
}









