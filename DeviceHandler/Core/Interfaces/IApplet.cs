using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public interface IApplet
    {
        IEnumerable<IPin> InPins { get; }
        IEnumerable<IPin> OutPins { get; }
    }
}
