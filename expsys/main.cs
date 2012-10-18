using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DeviceHandler;
using System.IO;

namespace expsys
{
    class main
    {
        class ReadApplet : Applet
        {
            private OutPin<CHDSample[]> m_outputPin;

            public ReadApplet(GraphBuilder gb): base(gb)
            {
                m_outputPin = RegisterOutputPin<CHDSample[]>();
            }

            public void run(string fileName)
            {
                /*List<CHDSample> list = new List<CHDSample>();
                using (TextReader tr = new StreamReader(fileName))
                {
                    string str;
                    while ((str = tr.ReadLine()) != null)
                    {
                        string[] split = str.Replace('.', ',').Split('\t');
                        double[] halls = new double[] { double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]) };
                        //m_outputPin.Push(new CHDSample(halls));
                        list.Add(new CHDSample(halls, double.Parse(split[0])*1000));
                    }
                }
                m_outputPin.Push(list.ToArray());*/
            }
        }

        static void Main(string[] args)
        {
            /*Dipole d = new Dipole() { m = 1.5, betta = 0.2, ro = -0.2, gamma = 0.0, zd = 90 };
            for (int ch = 0; ch < 3; ch++)
            {
                double alpha = ch * 2 * Math.PI / 3;
                double A1 = d.m * Math.Sin(d.betta) * Math.Cos(alpha - d.gamma);
                double A2 = 3 * d.m * (2.75 - d.ro * Math.Cos(alpha));
                double A3 = (2.75 * Math.Cos(alpha - d.gamma) - d.ro * Math.Cos(d.gamma)) * Math.Sin(d.betta);
                double B0 = 2.75 * 2.75 + d.ro * d.ro - 2 * 2.75 * d.ro * Math.Cos(alpha);
                using (System.IO.TextWriter tw = new System.IO.StreamWriter(string.Format("dip{0}.txt", ch)))
                {
                    for (double z = 0; z < 200; z += 0.4)
                    {
                        double A4 = (z - d.zd) * Math.Cos(d.betta);
                        double B1 = Math.Pow((z - d.zd) * (z - d.zd) + B0, -1.5);
                        double B2 = Math.Pow((z - d.zd) * (z - d.zd) + B0, -2.5);
                        double H = -A1 * B1 + A2 * B2 * (A3 + A4);  //отсчет модели
                        tw.WriteLine("{0}\t{1}", z, H);
                    }
                }
            }*/
            GraphBuilder gb = new GraphBuilder();
            AllanDetectorApplet allanDetector = new AllanDetectorApplet(gb, 10.0f);
            ReadApplet reader = new ReadApplet(gb);
            DipoleRestoreApplet dra = new DipoleRestoreApplet(gb);

            //gb.ConnectApplets(reader, allanDetector);
            gb.ConnectApplets(reader, dra);
            reader.run("test4.txt");

            Console.ReadKey();
        }
    }
}
