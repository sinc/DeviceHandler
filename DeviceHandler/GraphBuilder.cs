using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public class GraphBuilder: IDisposable
    {
        private List<Applet> m_Applets;

        public GraphBuilder()
        {
            m_Applets = new List<Applet>();
        }

        /// <summary>
        /// Зарегистрировать апплет
        /// </summary>
        /// <param name="applet"></param>
        public void RegisterApplet(Applet applet)
        {
            m_Applets.Add(applet);
        }

        /// <summary>
        /// Подключить апплеты.
        /// Если все успешно, вернет true.
        /// По умолчанию соединяются MainInPin и MainOutPin
        /// </summary>
        /// <param name="src_app">Апплет, к которому подключаемся</param>
        /// <param name="dst_app">Апплет, который подключаем</param>
        public bool ConnectApplets(Applet src_app, Applet dst_app)
        {
            return ConnectApplets(src_app, dst_app, "MainOutPin", "MainInPin");
        }

        /// <summary>
        /// Подключить апплеты.
        /// Если все успешно, вернет true. 
        /// </summary>
        /// <param name="SrcApplet">Апплет, к которому подключаемся</param>
        /// <param name="DstApplet">Апплет, который подключаем</param>
        /// <param name="SrcPinName">Имя выходного пина, к которому подключаемся</param>
        /// <param name="DstPinName">Имя входного пина, который подключаем</param>
        /// <returns></returns>
        public bool ConnectApplets(Applet SrcApplet, Applet DstApplet, string SrcPinName, string DstPinName)
        {
            Pin src_outpin = SrcApplet.OutputPins.First(pin => pin.Name == SrcPinName);
            Pin dst_inpin = DstApplet.InputPins.First(pin => pin.Name == DstPinName);

<<<<<<< HEAD
            if (src_outpin == null)
            {
                System.Diagnostics.Debug.Print("Pin {0} is mising", SrcPinName);
                return false;
            }

            if (dst_inpin == null)
            {
                System.Diagnostics.Debug.Print("Pin {0} is mising", DstPinName);
                return false;
            }

=======
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
            try
            {
                //соединяем
                src_outpin.Connect(dst_inpin);
                dst_inpin.Connect(src_outpin);
            }
            catch (ConnectException cex)
            {
                System.Diagnostics.Debug.Print(cex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Полностью разъединяет апплеты между собой
        /// </summary>
        /// <param name="Applet1"></param>
        /// <param name="Applet2"></param>
        public void DisconnectApplets(Applet Applet1, Applet Applet2)
        {
            foreach (Pin outpin in Applet1.OutputPins)
            {
                foreach (Pin inpin in Applet2.InputPins)
                {
                    if (outpin.Connected(inpin))
                        inpin.Disconnect();
                }
            }

            foreach (Pin outpin in Applet2.OutputPins)
            {
                foreach (Pin inpin in Applet1.InputPins)
                {
                    if (outpin.Connected(inpin))
                        inpin.Disconnect();
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (Applet app in m_Applets)
            {
                foreach (Pin outpins in app.OutputPins)
                    outpins.Disconnect();
            }
        }

        #endregion
    }
}