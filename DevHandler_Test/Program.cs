using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DeviceHandler;

namespace DevHandler_Test
{
    class MainForm: Form
    {
        GraphBuilder graph_builder = new GraphBuilder();
        RandomGenerator gen;
        aver a;
        ChartApplet chart;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        private static bool AppStillIdle
        {
            get
            {
                Message msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        public MainForm()
        {
            gen = new RandomGenerator(graph_builder);
            a = new aver(graph_builder);
            chart = new ChartApplet(graph_builder, 2, 1.0);

            InitializeComponent();
            this.Controls.Add(chart.Chart);

            Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                chart.Chart.Refresh();
            }
        }

        static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(673, 432);
            this.Name = "MainForm";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.ResumeLayout(false);

        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            graph_builder.ConnectApplets(gen, a);
            graph_builder.ConnectApplets(gen, chart, "MainOutPin", "ChartInput0");
            graph_builder.ConnectApplets(a, chart, "MainOutPin", "ChartInput1");
        }
    }


    class RandomGenerator : SimpleApplet<int, double>
    {
        private Random m_rnd;
        private System.Threading.Timer m_timer;

        public RandomGenerator(GraphBuilder gb)
            : base(1, 1, gb)
        {
            m_rnd = new Random((int)DateTime.Now.Ticks);
            m_timer = new System.Threading.Timer(new TimerCallback(
                delegate(Object obj)
                {
                    AppletEngine(null);
                }), null, 100, 10);
        }

        protected override void AppletEngine(IEnumerable<int> data)
        {
            OutputPin.Push(m_rnd.NextDouble());
        }
    }

    class aver : SimpleApplet<double, double>
    {
        public aver(GraphBuilder gb)
            : base(1, 100, gb)
        {
        }

        protected override void AppletEngine(IEnumerable<double> data)
        {
            double m = 0;

            foreach (double d in data)
                m += d;

            OutputPin.Push(m / (double)data.Count());
        }
    }

    class console_printer : SimpleApplet<double, int>
    {
        public console_printer(GraphBuilder gb)
            : base(1, 1, gb)
        {
        }

        protected override void AppletEngine(IEnumerable<double> data)
        {
            foreach (double d in data)
                Console.WriteLine(d);
        }
    }

}
