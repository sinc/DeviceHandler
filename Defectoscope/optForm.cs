using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Defectoscope
{
    class optForm : Form
    {
        private GroupBox groupBox1;
        private Label label2;
        private NumericUpDown averUpDown;
        private NumericUpDown windowsUpDown;
        private GroupBox gbSaveToFile;
        private TextBox txtFileName;
        private Button btnBrowse;
        private SaveFileDialog saveFileDialog;
        private CheckBox recordEnable;
        private Button okButton;
        private Button cancelButton;
        private Label label1;

        public optForm()
        {
            InitializeComponent();
        }

        public bool record { get; private set; }
        public string fileName { get; private set; }
        public int windowLength { get; private set; }
        public int averLength { get; private set; }

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.averUpDown = new System.Windows.Forms.NumericUpDown();
            this.windowsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbSaveToFile = new System.Windows.Forms.GroupBox();
            this.recordEnable = new System.Windows.Forms.CheckBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.averUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.windowsUpDown)).BeginInit();
            this.gbSaveToFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.averUpDown);
            this.groupBox1.Controls.Add(this.windowsUpDown);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 79);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Детектор Алана";
            // 
            // averUpDown
            // 
            this.averUpDown.Location = new System.Drawing.Point(138, 42);
            this.averUpDown.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.averUpDown.Name = "averUpDown";
            this.averUpDown.Size = new System.Drawing.Size(126, 20);
            this.averUpDown.TabIndex = 5;
            this.averUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // windowsUpDown
            // 
            this.windowsUpDown.Location = new System.Drawing.Point(16, 42);
            this.windowsUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.windowsUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.windowsUpDown.Name = "windowsUpDown";
            this.windowsUpDown.Size = new System.Drawing.Size(116, 20);
            this.windowsUpDown.TabIndex = 4;
            this.windowsUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Длина окна усреднения";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Длина выборки";
            // 
            // gbSaveToFile
            // 
            this.gbSaveToFile.Controls.Add(this.recordEnable);
            this.gbSaveToFile.Controls.Add(this.txtFileName);
            this.gbSaveToFile.Controls.Add(this.btnBrowse);
            this.gbSaveToFile.Location = new System.Drawing.Point(12, 97);
            this.gbSaveToFile.Name = "gbSaveToFile";
            this.gbSaveToFile.Size = new System.Drawing.Size(276, 78);
            this.gbSaveToFile.TabIndex = 1;
            this.gbSaveToFile.TabStop = false;
            this.gbSaveToFile.Text = "Записать выборки в файл...";
            // 
            // recordEnable
            // 
            this.recordEnable.AutoSize = true;
            this.recordEnable.Location = new System.Drawing.Point(16, 20);
            this.recordEnable.Name = "recordEnable";
            this.recordEnable.Size = new System.Drawing.Size(114, 17);
            this.recordEnable.TabIndex = 2;
            this.recordEnable.Text = "Включить запись";
            this.recordEnable.UseVisualStyleBackColor = true;
            this.recordEnable.CheckedChanged += new System.EventHandler(this.recordEnable_CheckedChanged);
            // 
            // txtFileName
            // 
            this.txtFileName.Enabled = false;
            this.txtFileName.Location = new System.Drawing.Point(16, 43);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(217, 20);
            this.txtFileName.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Location = new System.Drawing.Point(239, 43);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Магнитограммы ASCII|*.txt|Все файлы|*.*";
            this.saveFileDialog.Title = "Сохранение магнитограммы";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(69, 181);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(150, 181);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // optForm
            // 
            this.ClientSize = new System.Drawing.Size(300, 211);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.gbSaveToFile);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "optForm";
            this.Text = "Настройки";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.averUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.windowsUpDown)).EndInit();
            this.gbSaveToFile.ResumeLayout(false);
            this.gbSaveToFile.PerformLayout();
            this.ResumeLayout(false);

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = saveFileDialog.FileName;
            }
        }

        private void recordEnable_CheckedChanged(object sender, EventArgs e)
        {
            txtFileName.Enabled = recordEnable.Enabled;
            btnBrowse.Enabled = recordEnable.Enabled;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            fileName = txtFileName.Text;
            record = recordEnable.Checked;
            windowLength = (int)windowsUpDown.Value;
            averLength = (int)averUpDown.Value;
        }
    }
}
