using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public class ConsolePrinter: Applet
    {
        public ConsolePrinter()
        {
            InPin<Object> pin = RegisterInputPin<Object>("MainInPin", 1, 1);
            pin.OnNewDataEnabled += new NewDataEnabled<object>(pin_OnNewDataEnabled);
        }

        void pin_OnNewDataEnabled(IEnumerable<object> data)
        {
            foreach (Object obj in data)
                Console.WriteLine(obj.ToString());
        }
    }
}
