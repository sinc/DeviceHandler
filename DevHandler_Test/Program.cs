using System;
using System.Collections.Generic;
using System.Linq;
using DeviceHandler;
using System.Threading;

namespace DevHandler_Test
{
    class RandomGenerator : SimpleApplet<int, double>
    {
        private Random m_rnd;
        private Timer m_timer;

        public RandomGenerator(GraphBuilder gb): base(1,1, gb)
        {
            m_rnd = new Random((int)DateTime.Now.Ticks);
            m_timer = new Timer(new TimerCallback(
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
            : base(1, 10, gb)
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

    class Program
    {
        static void Main(string[] args)
        {
            GraphBuilder graph_builder = new GraphBuilder();

            RandomGenerator gen = new RandomGenerator(graph_builder);
            aver a = new aver(graph_builder);
            console_printer cp = new console_printer(graph_builder);

            graph_builder.ConnectApplets(gen, a);
            graph_builder.ConnectApplets(a, cp);

            Thread.Sleep(1000);

            graph_builder.DisconnectApplets(a, cp);

            Thread.Sleep(1000);

            graph_builder.ConnectApplets(a, cp);

            Console.ReadKey();
        }
    }
}
