using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Chart
{
  public class TopographicSeries : Series
  {
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

    private double[, ] _dataZ;

    public double[, ] DataZ
    {
      get { return _dataZ; }
      private set { _dataZ = value; }
    }

    private Bitmap _bitmapDataZ;

    public TopographicSeries(Chart chart, string name)
      : base(chart, name)
    {
      _dataList.Add(null); // DataX
      _dataList.Add(null); // DataY
      _dataZ = null;
    }

    public override void Draw(System.Drawing.Graphics g)
    {
      if (_bitmapDataZ != null)
      {
        double kx = (double)_bitmapDataZ.Width / (DataX[DataX.Count - 1] - DataX[0]);
        double ky = (double)_bitmapDataZ.Height / (DataY[DataY.Count - 1] - DataY[0]);

        float x1 = (float)(kx * (_chart.AxisX.Min - DataX[0])); //(float)(0 + (DataX[DataX.GetLength(0) - 1] - DataX[0]) * (_bitmapDataZ.Width) / (_chart.AxisX.Max - _chart.AxisX.Min));
        float y1 = (float)(ky * (_chart.AxisY.Min - DataY[0])); //(float)(0 + (DataX[DataY.GetLength(0) - 1] - DataY[0]) * (_bitmapDataZ.Height) / (_chart.AxisY.Max - _chart.AxisY.Min));
        float x2 = (float)(kx * (_chart.AxisX.Max - DataX[0])); //(float)(0 + (DataX[DataY.GetLength(0) - 1] - DataX[0]) * (_bitmapDataZ.Width) / (_chart.AxisX.Max - _chart.AxisX.Min));
        float y2 = (float)(ky * (_chart.AxisY.Max - DataY[0])); //(float)(0 + (DataX[DataX.GetLength(0) - 1] - DataY[0]) * (_bitmapDataZ.Height) / (_chart.AxisY.Max - _chart.AxisY.Min));

        Rectangle dstRectangle = new Rectangle(_chart.GraphicRectangle.Left, _chart.GraphicRectangle.Bottom, _chart.GraphicRectangle.Width, -_chart.GraphicRectangle.Height);

        g.DrawImage(_bitmapDataZ, dstRectangle, x1, y1, x2 - x1, y2 - y1, GraphicsUnit.Pixel);
      }
    }

    public void SetData(double[, ] dataZ)
    {
      DataZ = dataZ;

      int h = DataZ.GetLength(0);
      int w = DataZ.GetLength(1);
      
      DataX = new List<double>();
      DataY = new List<double>();

      for (int x = 0; x < w; x++)
        DataX.Add((double)x);

      for (int y = 0; y < h; y++)
        DataY.Add((double)y);

      _bitmapDataZ = new Bitmap(w, h);

      for (int i = 0; i < h; i++)
        for (int j = 0; j < w; j++)
          _bitmapDataZ.SetPixel(j, i, ColorWheel.GetGrayscale(DataZ[i, j], 0.0, 1.0));
    }

    public void SetData(List<double> dataX, List<double> dataY, double[,] dataZ)
    {
      DataZ = dataZ;
      DataY = dataY;
      DataX = dataX;

      int h = DataZ.GetLength(0);
      int w = DataZ.GetLength(1);

      _bitmapDataZ = new Bitmap(w, h);

      for (int i = 0; i < h; i++)
        for (int j = 0; j < w; j++)
          _bitmapDataZ.SetPixel(j, i, ColorWheel.GetGrayscale(DataZ[i, j], 0.0, 1.0));
    }
  }
}
