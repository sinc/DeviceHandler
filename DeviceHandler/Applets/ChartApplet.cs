using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Chart;

namespace DeviceHandler
{
    public class ChartApplet: Applet
    {
        public ChartPanel Chart { get; private set; }

        private double m_XStep;

        /// <summary>
        /// Здесь отсчеты идут эквидистантно с шагом x_step
        /// </summary>
        /// <param name="inputs">Количество входов</param>
        /// <param name="x_step">шаг по х</param>
        public ChartApplet(GraphBuilder gb, int inputs, double x_step): base(gb)
        {
            m_XStep = x_step;
            Chart = new ChartPanel();
            Chart.Dock = DockStyle.Fill;
            Chart.Location = new Point(0, 0);

            Random rnd_clr = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < inputs; i++)
            {
                Chart.Chart.SeriesList.Add(new LineSeries(Chart.Chart, "series" + i.ToString(),
                    Color.FromArgb(rnd_clr.Next(255), rnd_clr.Next(255), rnd_clr.Next(255)), 2.0f));
                
                int index = i;
                double x = 0.0;
                InPin<double> pin = RegisterInputPin<double>("ChartInput" + i.ToString(), 1, 1);
                pin.OnNewDataEnabled += new NewDataEnabled<double>(
                    delegate(IEnumerable<double> data)
                    {
                        LineSeries ls = (Chart.Chart.SeriesList[index] as LineSeries);
                        foreach (double y in data)
                        {
                            ls.AddPoint(x, y);
                            x += m_XStep;
                        }
                        
                        Chart.Chart.AutoFit();
                    });
            }
        }
    }
}
