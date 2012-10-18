using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DeviceHandler;
using FTD2XX_NET;
using System.Drawing;

namespace adnsWatcher
{
    class mainForm: Form
    {
        public const string kDevice_Descriptor = "A7004rbo";

        private GraphBuilder m_graphBuilder;
        private FTDIBulkDriver m_bulk;
        private AdnsReader m_reader;
        private Printer m_printer;

        public mainForm(FTDI dev)
        {
            InitializeComponent();

            m_graphBuilder = new GraphBuilder();

            m_bulk = new FTDIBulkDriver(m_graphBuilder, dev, kDevice_Descriptor, 9600u);
            m_reader = new AdnsReader(m_graphBuilder);
            m_printer = new Printer(m_graphBuilder, Graphics.FromHwnd(Handle));

            m_graphBuilder.ConnectApplets(m_bulk, m_reader);
            m_graphBuilder.ConnectApplets(m_reader, m_printer);

            m_bulk.start(900);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // mainForm
            // 
            this.ClientSize = new System.Drawing.Size(365, 330);
            this.Name = "mainForm";
            this.Activated += new System.EventHandler(this.mainForm_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mainForm_FormClosed);
            this.ResumeLayout(false);

        }

        private void mainForm_Activated(object sender, EventArgs e)
        {
        }
        
        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_bulk.stop();

            m_graphBuilder.DisconnectApplets(m_reader, m_printer);
            m_graphBuilder.DisconnectApplets(m_bulk, m_reader);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FTDI dev = new FTDI();
            uint num_of_devices = 0;
            bool device_found = false;

            while (!device_found)
            {
                dev.GetNumberOfDevices(ref num_of_devices);
                if (num_of_devices > 0)
                {
                    FTDI.FT_DEVICE_INFO_NODE[] devList = new FTDI.FT_DEVICE_INFO_NODE[num_of_devices];
                    dev.GetDeviceList(devList);
                    foreach (FTDI.FT_DEVICE_INFO_NODE dev_info in devList)
                    {
                        if (dev_info.SerialNumber == kDevice_Descriptor)
                        {
                            device_found = true;
                            break;
                        }
                    }
                    if (device_found)
                    {
                        try
                        {
                        Application.Run(new mainForm(dev));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("Устройство не обнаружено. Подключите и нажмите ОК.", "Ошибка", MessageBoxButtons.OKCancel) ==
                        DialogResult.Cancel)
                    {
                        break;
                    }
                }
            }
        }
    }
}
