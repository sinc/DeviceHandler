using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Chart;
using System.Threading;

namespace DeviceHandler
{
    class DipoleData
    {
        public CHDSample[] data { get; set; }

        public override string ToString()
        {
            CHDSample first = data.First();
            CHDSample last = data.Last();
            return string.Format("{0}:{1}", first.x, last.x);
        }
    }

    public class DefectoscopeApplet: Applet
    {
        private const int kVisiblePoints = 100;
        private LineSeries m_mainChartSeriesCh1;
        private LineSeries m_mainChartSeriesCh2;
        private LineSeries m_mainChartSeriesCh3;
        private LineSeries m_dipoleChartSeriesCh1;
        private LineSeries m_dipoleChartSeriesCh2;
        private LineSeries m_dipoleChartSeriesCh3;
        public ChartPanel m_mainChartPanel;
        public ChartPanel m_dipoleChartPanel;
        public ListBox m_dipoleListBox;
        public Form m_defectoscopeForm;

        public DefectoscopeApplet(GraphBuilder gb, ChartPanel mainPanel, ChartPanel dipolePanel, ListBox dipoleList, Form defectoscopeForm)
            : base(gb)
        {
            m_defectoscopeForm = defectoscopeForm;
            m_mainChartPanel = mainPanel;
            m_dipoleChartPanel = dipolePanel;
            m_mainChartSeriesCh1 = new LineSeries(m_mainChartPanel.Chart, "1", Color.Red, 2.0f);
            m_mainChartSeriesCh2 = new LineSeries(m_mainChartPanel.Chart, "2", Color.Green, 2.0f);
            m_mainChartSeriesCh3 = new LineSeries(m_mainChartPanel.Chart, "3", Color.Blue, 2.0f);

            m_dipoleChartSeriesCh1 = new LineSeries(m_dipoleChartPanel.Chart, "1", Color.Red, 2.0f);
            m_dipoleChartSeriesCh2 = new LineSeries(m_dipoleChartPanel.Chart, "2", Color.Green, 2.0f);
            m_dipoleChartSeriesCh3 = new LineSeries(m_dipoleChartPanel.Chart, "3", Color.Blue, 2.0f);

            m_mainChartPanel.Chart.SeriesList.Add(m_mainChartSeriesCh1);
            m_mainChartPanel.Chart.SeriesList.Add(m_mainChartSeriesCh2);
            m_mainChartPanel.Chart.SeriesList.Add(m_mainChartSeriesCh3);
            m_dipoleChartPanel.Chart.SeriesList.Add(m_dipoleChartSeriesCh1);
            m_dipoleChartPanel.Chart.SeriesList.Add(m_dipoleChartSeriesCh2);
            m_dipoleChartPanel.Chart.SeriesList.Add(m_dipoleChartSeriesCh3);

            InPin<CHDSample> mainIn = RegisterInputPin<CHDSample>(1, kVisiblePoints);
            mainIn.OnNewDataEnabled += new NewDataEnabled<CHDSample>(mainIn_OnNewDataEnabled);

            InPin<CHDSample[]> subIn = RegisterInputPin<CHDSample[]>("SubInPin", 1, 1);
            subIn.OnNewDataEnabled += new NewDataEnabled<CHDSample[]>(subIn_OnNewDataEnabled);

            m_dipoleListBox = dipoleList;
            m_dipoleListBox.SelectedIndexChanged += new EventHandler(dipoleList_SelectedIndexChanged);
        }

        void dipoleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CHDSample[] sample = (m_dipoleListBox.Items[m_dipoleListBox.SelectedIndex] as DipoleData).data;
            m_dipoleChartSeriesCh1.SetData(sample.Select(s => s.hall[0]).ToArray());
            m_dipoleChartSeriesCh2.SetData(sample.Select(s => s.hall[1]).ToArray());
            m_dipoleChartSeriesCh3.SetData(sample.Select(s => s.hall[2]).ToArray());
            m_dipoleChartPanel.Chart.AutoFit();
            m_dipoleChartPanel.Refresh();
        }

        void subIn_OnNewDataEnabled(IEnumerable<CHDSample[]> data)
        {
            m_defectoscopeForm.Invoke(new MethodInvoker(
                delegate()
                {
                    m_dipoleListBox.Items.Add(new DipoleData() { data = data.First() });
                }
            ));
        }

        void mainIn_OnNewDataEnabled(IEnumerable<CHDSample> data)
        {
            m_mainChartSeriesCh1.SetData(data.Select(s => s.hall[0]).ToArray());
            m_mainChartSeriesCh2.SetData(data.Select(s => s.hall[1]).ToArray());
            m_mainChartSeriesCh3.SetData(data.Select(s => s.hall[2]).ToArray());
            m_mainChartPanel.Chart.AutoFit();
        }
    }
}
