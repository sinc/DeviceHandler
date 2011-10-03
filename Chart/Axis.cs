using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Chart
{
  public class Axis
  {
    protected Chart _chart;

    private string _name;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    private string _units;

    public string Units
    {
      get { return _units; }
      set { _units = value; }
    }

    private double _min;

    public double Min
    {
      get { return _min; }
      set { _min = value; CalcTickMarks(); }
    }

    private double _max;

    public double Max
    {
      get { return _max; }
      set { _max = value; CalcTickMarks(); }
    }

    private double _screenMin;

    public double ScreenMin
    {
      get { return _screenMin; }
      set { _screenMin = value; }
    }

    private double _screenMax;

    public double ScreenMax
    {
      get { return _screenMax; }
      set { _screenMax = value; }
    }

    private int _tickMarksCount = 11;

    private double[] _tickMarks;

    public double[] TickMarks
    {
      get { return _tickMarks; }
    }

    private Pen _axisPen;

    public Pen AxisPen
    {
      get { return _axisPen; }
      set { _axisPen = value; }
    }

    public Axis(Chart chart, string name, string units)
    {
      _chart = chart;
      _name = name;
      _units = units;
      _min = 0.0;
      _max = 1.0;
      _axisPen = new Pen(Color.Black, 1.0f);

      _tickMarksCount = 11;
      _tickMarks = new double[11];
      CalcTickMarks();
    }

    private void CalcTickMarks()
    {
      double order = Math.Round(Math.Log10(_max - _min)) - 1.0;
      double scaleFactor = Math.Pow(10.0, order);

      double min = Math.Round(_min / scaleFactor) * scaleFactor;
      double max = Math.Round(_max / scaleFactor) * scaleFactor;

      for (int i = 0; i < _tickMarksCount; i++)
        _tickMarks[i] = min + (double)i * (max - min) / (double)(_tickMarksCount - 1);
    }

    public float GetDisplayValue(double val)
    {
      if (_max == _min)
        return 0.0f;

      return (float)(_screenMin + (val - _min) * (_screenMax - _screenMin) / (_max - _min)); 
    }

    public double GetOriginalValue(float val)
    {
      if (_screenMax == _screenMin)
        return 0.0;

      return _min + ((double)val - _screenMin) * (_max - _min) / (_screenMax - _screenMin);
    }
  }
}
