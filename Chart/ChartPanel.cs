using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Imaging;

namespace Chart
{
  public partial class ChartPanel : UserControl
  {
    //private enum MouseMode { None, Left, Middle, Right }

    //MouseMode _mouseMode = MouseMode.None;

    MouseButtons _pressedMouseButton = MouseButtons.None;

    private Chart _chart;

    public Chart Chart
    {
      get { return _chart; }
    }

    private Point _startPoint;

    private Rectangle _selectionRectangle;

    public Rectangle SelectionRectangle
    {
      get { return _selectionRectangle; }
    }

    public void ExportImage(string filename)
    {
      switch (Path.GetExtension(filename).ToLower())
      {
        case ".png":
          Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
          using (Graphics g = Graphics.FromImage(bmp))
          {
            _chart.Draw(g);
            bmp.Save(filename);
          }
          break;

        case ".emf":
          using (Graphics g = this.CreateGraphics())
          {
            IntPtr hdc = g.GetHdc();

            Metafile mf = new Metafile(filename, hdc);

            using (Graphics mg = Graphics.FromImage(mf))
            {
              using (mf)
              {
                _chart.Draw(mg);
              }
            }

            g.ReleaseHdc(hdc);
          }
          break;
      }
    }

    public ChartPanel()
    {
      InitializeComponent();

      _startPoint = Point.Empty;
      _selectionRectangle = Rectangle.Empty;
      _chart = new Chart(this, "Graphic");
    }

    private void ChartPanel_Paint(object sender, PaintEventArgs e)
    {
      _chart.Draw(e.Graphics);
    }

    private void ChartPanel_Resize(object sender, EventArgs e)
    {
      _chart.ClientRectangle = ClientRectangle;
    }

    private void ChartPanel_MouseDown(object sender, MouseEventArgs e)
    {
      _pressedMouseButton = e.Button;

      switch (e.Button)
      {
        case MouseButtons.Right:
          _chart.AutoFit();
          break;

        default:
          _startPoint = new Point(e.X, e.Y);
          break;
      }
    }

    private void ChartPanel_MouseMove(object sender, MouseEventArgs e)
    {
      int x1;
      int x2;
      int y1;
      int y2;

      switch (_pressedMouseButton)
      {
        case MouseButtons.Left:
          x1 = Math.Min(_startPoint.X, e.X);
          x2 = Math.Max(_startPoint.X, e.X);
          y1 = Math.Min(_startPoint.Y, e.Y);
          y2 = Math.Max(_startPoint.Y, e.Y);
          _selectionRectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);
          Refresh();
          break;

        case MouseButtons.Middle:
          /*x1 = Math.Min(_startPoint.X, e.X);
          x2 = Math.Max(_startPoint.X, e.X);
          y1 = Math.Min(_startPoint.Y, e.Y);
          y2 = Math.Max(_startPoint.Y, e.Y);
          _selectionRectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);*/
          /*double x1 = _chart.AxisX.GetOriginalValue(_startPoint.X);
          double x2 = _chart.AxisX.GetOriginalValue(e.X);
          double y1 = _chart.AxisY.GetOriginalValue(_startPoint.Y);
          double y2 = _chart.AxisY.GetOriginalValue(e.Y);*/

          //_chart.AxisX.Min += _chart.AxisX.GetOriginalValue(e.X - _startPoint.X);
          /*_chart.AxisX.Max = Math.Max(x1, x2);
          _chart.AxisY.Min = Math.Min(y1, y2);
          _chart.AxisY.Max = Math.Max(y1, y2);*/

          Refresh();
          break;
      }
    }

    private void ChartPanel_MouseUp(object sender, MouseEventArgs e)
    {
      switch (_pressedMouseButton)
      {
        case MouseButtons.Left:
          if (_startPoint != e.Location)
          {
            double x1 = _chart.AxisX.GetOriginalValue(_startPoint.X);
            double x2 = _chart.AxisX.GetOriginalValue(e.X);
            double y1 = _chart.AxisY.GetOriginalValue(_startPoint.Y);
            double y2 = _chart.AxisY.GetOriginalValue(e.Y);

            _chart.AxisX.Min = Math.Min(x1, x2);
            _chart.AxisX.Max = Math.Max(x1, x2);
            _chart.AxisY.Min = Math.Min(y1, y2);
            _chart.AxisY.Max = Math.Max(y1, y2);
          }
          _selectionRectangle = Rectangle.Empty;
          Refresh();
          break;
      }

      _pressedMouseButton = MouseButtons.None;
    }
  }
}
