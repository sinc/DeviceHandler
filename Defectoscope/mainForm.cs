using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DeviceHandler;
using FTD2XX_NET;
using System.Threading;

namespace Defectoscope
{
    class mainForm: Form
    {
        public const string kDevice_Descriptor = "A7004rbr";

        private optForm m_options = new optForm();

        private GraphBuilder m_graphBuilder;
        private CHDDriver m_defectoscope;
        private DefectoscopeApplet m_defectoscopeCharts;
        private AllanDetectorApplet m_alanDetector;
        private MagnetizationRestoreApplet m_magnetization;
        private DipoleRestoreApplet m_restoreDipole;
        private FileWriteApplet<CHDSample> m_defectoscopeData;
        private FileWriteApplet<Dipole> m_dipolesData;
        private StreamWriter m_writer;
        private StreamWriter m_dipoleWriter;

        private SplitContainer splitContainer;
        private System.Windows.Forms.Timer timer;
        private System.ComponentModel.IContainer components;
        private Chart.ChartPanel mainChartPanel;
        private ToolStrip toolBar;
        private ToolStripButton calibrateButton;
        private GroupBox DipoleGroupBox;
        private SplitContainer splitContainer1;
        private Chart.ChartPanel dipoleChartPanel;
        private ListBox dipoleListBox;
        private ToolStripButton startMeasure;

        public mainForm(FTDI dev)
        {
            InitializeComponent();
            m_graphBuilder = new GraphBuilder();
            m_defectoscope = new CHDDriver(m_graphBuilder, dev, kDevice_Descriptor, 9600u);
            m_defectoscopeCharts = new DefectoscopeApplet(m_graphBuilder, mainChartPanel, dipoleChartPanel, dipoleListBox, this);

            m_graphBuilder.ConnectApplets(m_defectoscope, m_defectoscopeCharts);
        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopMeasure();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.mainChartPanel = new Chart.ChartPanel();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.calibrateButton = new System.Windows.Forms.ToolStripButton();
            this.startMeasure = new System.Windows.Forms.ToolStripButton();
            this.DipoleGroupBox = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dipoleChartPanel = new Chart.ChartPanel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.dipoleListBox = new System.Windows.Forms.ListBox();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolBar.SuspendLayout();
            this.DipoleGroupBox.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.mainChartPanel);
            this.splitContainer.Panel1.Controls.Add(this.toolBar);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.DipoleGroupBox);
            this.splitContainer.Size = new System.Drawing.Size(769, 637);
            this.splitContainer.SplitterDistance = 307;
            this.splitContainer.TabIndex = 0;
            // 
            // mainChartPanel
            // 
            this.mainChartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainChartPanel.Location = new System.Drawing.Point(0, 25);
            this.mainChartPanel.Name = "mainChartPanel";
            this.mainChartPanel.Size = new System.Drawing.Size(769, 282);
            this.mainChartPanel.TabIndex = 3;
            // 
            // toolBar
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calibrateButton,
            this.startMeasure});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(769, 25);
            this.toolBar.TabIndex = 2;
            this.toolBar.Text = "toolStrip1";
            // 
            // calibrateButton
            // 
            this.calibrateButton.Image = ((System.Drawing.Image)(resources.GetObject("calibrateButton.Image")));
            this.calibrateButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.calibrateButton.Name = "calibrateButton";
            this.calibrateButton.Size = new System.Drawing.Size(88, 22);
            this.calibrateButton.Text = "Калибровка";
            this.calibrateButton.Click += new System.EventHandler(this.calibrateButton_Click);
            // 
            // startMeasure
            // 
            this.startMeasure.Image = ((System.Drawing.Image)(resources.GetObject("startMeasure.Image")));
            this.startMeasure.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startMeasure.Name = "startMeasure";
            this.startMeasure.Size = new System.Drawing.Size(120, 22);
            this.startMeasure.Text = "Начать измерение";
            this.startMeasure.Click += new System.EventHandler(this.startMeasure_Click);
            // 
            // DipoleGroupBox
            // 
            this.DipoleGroupBox.Controls.Add(this.splitContainer1);
            this.DipoleGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DipoleGroupBox.Location = new System.Drawing.Point(0, 0);
            this.DipoleGroupBox.Name = "DipoleGroupBox";
            this.DipoleGroupBox.Size = new System.Drawing.Size(769, 326);
            this.DipoleGroupBox.TabIndex = 0;
            this.DipoleGroupBox.TabStop = false;
            this.DipoleGroupBox.Text = "Обнаруженные диполи";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 16);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dipoleListBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dipoleChartPanel);
            this.splitContainer1.Size = new System.Drawing.Size(763, 307);
            this.splitContainer1.SplitterDistance = 142;
            this.splitContainer1.TabIndex = 4;
            // 
            // dipoleChartPanel
            // 
            this.dipoleChartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dipoleChartPanel.Location = new System.Drawing.Point(0, 0);
            this.dipoleChartPanel.Name = "dipoleChartPanel";
            this.dipoleChartPanel.Size = new System.Drawing.Size(617, 307);
            this.dipoleChartPanel.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // dipoleListBox
            // 
            this.dipoleListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dipoleListBox.FormattingEnabled = true;
            this.dipoleListBox.Location = new System.Drawing.Point(0, 0);
            this.dipoleListBox.Name = "dipoleListBox";
            this.dipoleListBox.Size = new System.Drawing.Size(142, 303);
            this.dipoleListBox.TabIndex = 0;
            // 
            // mainForm
            // 
            this.ClientSize = new System.Drawing.Size(769, 637);
            this.Controls.Add(this.splitContainer);
            this.Name = "mainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mainForm_FormClosed);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.DipoleGroupBox.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        [STAThread]
        static void Main(string[] args)
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

        private void stopMeasure()
        {
            m_defectoscope.stopMeasure();
            timer.Stop();
            if (m_writer != null)
            {
                m_graphBuilder.DisconnectApplets(m_defectoscope, m_defectoscopeData);
                m_writer.Close();
                m_writer = null;
                m_defectoscopeData = null;
            }
            m_graphBuilder.DisconnectApplets(m_restoreDipole, m_dipolesData);
            m_dipoleWriter.Close();
            startMeasure.Text = "Начать измерение";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            mainChartPanel.Chart.Refresh();
            dipoleChartPanel.Chart.Refresh();
        }

        private void calibrateButton_Click(object sender, EventArgs e)
        {
            stopMeasure();
            m_defectoscope.calibrate();
        }

        private void startMeasure_Click(object sender, EventArgs e)
        {
            if (startMeasure.Text == "Начать измерение")
            {
                if (m_options.ShowDialog() == DialogResult.OK)
                {
                    if (m_options.record)
                    {
                        m_writer = new StreamWriter(m_options.fileName);
                        m_defectoscopeData = new FileWriteApplet<CHDSample>(m_graphBuilder, m_writer);
                        m_graphBuilder.ConnectApplets(m_defectoscope, m_defectoscopeData);
                    }
                    m_dipoleWriter = new StreamWriter("dipoles.txt");
                    m_dipolesData = new FileWriteApplet<Dipole>(m_graphBuilder, m_dipoleWriter);
                    m_restoreDipole = new DipoleRestoreApplet(m_graphBuilder);
                    m_alanDetector = new AllanDetectorApplet(m_graphBuilder, m_options.windowLength, m_options.averLength, 10);

                    m_graphBuilder.ConnectApplets(m_defectoscope, m_alanDetector);
                    m_graphBuilder.ConnectApplets(m_alanDetector, m_defectoscopeCharts, "MainOutPin", "SubInPin");
                    m_graphBuilder.ConnectApplets(m_alanDetector, m_restoreDipole);
                    m_graphBuilder.ConnectApplets(m_restoreDipole, m_dipolesData);

                    startMeasure.Text = "Остановить измерение";
                    m_defectoscope.startMeasure();
                    timer.Start();
                }
            }
            else
            {
                stopMeasure();
            }
        }
    }
}
