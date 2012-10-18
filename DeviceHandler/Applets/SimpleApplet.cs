using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceHandler
{
    public abstract class SimpleApplet<InputDataType, OutputDataType>: Applet
    {
        private InPin<InputDataType> InputPin { get; set; }
        private OutPin<OutputDataType> OutputPin { get; set; }

        /// <summary>
        /// Конструктор апплета.
        /// Апплет может работать сразу с пачкой данных, причем
        /// следующую пачку данных можно считывать с некоторым шагом
        /// </summary>
        /// <param name="Shift">Сдвиг после чтения пачки данных</param>
        /// <param name="Count">Длина пачки данных</param>
        public SimpleApplet(int Shift, int Count, GraphBuilder gb): base(gb)
        {
            InputPin = RegisterInputPin<InputDataType>("MainInPin", Shift, Count);
            OutputPin = RegisterOutputPin<OutputDataType>("MainOutPin");
            InputPin.OnNewDataEnabled += new NewDataEnabled<InputDataType>(InputPin_OnNewDataEnabled);
        }

        private void InputPin_OnNewDataEnabled(IEnumerable<InputDataType> data)
        {
            OutputPin.Push(AppletEngine(data));
        }

        /// <summary>
        /// После наследования от класса Applet нужно перегрузить эту функцию
        /// </summary>
        /// <param name="data"></param>
        protected abstract OutputDataType AppletEngine(IEnumerable<InputDataType> data);
    }
}
