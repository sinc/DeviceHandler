using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Chart
{
  public class LineSeries : Series
  {
    protected Pen _pnLine;

    public Color Color
    {
      get { return _pnLine.Color; }
      set { _pnLine.Color = value; }
    }

    public float Width
    {
      get { return _pnLine.Width; }
      set { _pnLine.Width = value; }
    }

    public DashStyle DashStyle
    {
      get { return _pnLine.DashStyle; }
      set { _pnLine.DashStyle = value; }
    }

    public List<double> DataX
    {
      get { return _dataList[0]; }
      private set { _dataList[0] = value; }
    }

    public List<double> DataY
    {
      get { return _dataList[1]; }
      private set { _dataList[1] = value; }
    }

    public LineSeries(Chart chart, string name, Color color, float width)
      : base(chart, name)
    {
      _pnLine = new Pen(color, width);
      _pnLine.DashStyle = DashStyle.Solid;

      _dataList.Add(new List<double>()); // DataX
      _dataList.Add(new List<double>()); // DataY
    }

    public override void Draw(System.Drawing.Graphics g)
    {
        lock (this)
        {
            if (DataY != null)
                for (int i = 1; i < DataY.Count; i++)
                    g.DrawLine(_pnLine,
                        (float)(_chart.AxisX.GetDisplayValue(DataX[i - 1])),
                        _chart.AxisY.GetDisplayValue(DataY[i - 1]),
                        _chart.AxisX.GetDisplayValue(DataX[i]),
                        _chart.AxisY.GetDisplayValue(DataY[i]));
        }
    }

    public void AddPoint(double X, double Y)
    {
        DataX.Add(X);
        DataY.Add(Y);
    }

    public void SetData(double[] dataY)
    {
        lock (this)
        {
            DataY = new List<double>(dataY);

            int count = DataY.Count;
            DataX = new List<double>();

            for (int i = 0; i < count; i++)
                DataX.Add((double)i);
        }
    }

    public void SetData(double[] dataX, double[] dataY)
    {
      DataY = new List<double>(dataY);
      DataX = new List<double>(dataX);
    }
  }
}
