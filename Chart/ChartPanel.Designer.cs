namespace Chart
{
  partial class ChartPanel
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.SuspendLayout();
      // 
      // ChartPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.DoubleBuffered = true;
      this.Name = "ChartPanel";
      this.Size = new System.Drawing.Size(625, 447);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.ChartPanel_Paint);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_MouseMove);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_MouseDown);
      this.Resize += new System.EventHandler(this.ChartPanel_Resize);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_MouseUp);
      this.ResumeLayout(false);

    }

    #endregion
  }
}
