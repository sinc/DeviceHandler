using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.ComponentModel;
using System.Drawing.Design;

namespace Chart
{
  public class Chart
  {
    protected ChartPanel _chartPanel;

    protected string _title;

    [Category("General")]
    public string Title
    {
      get { return _title; }
      set { _title = value; }
    }

    private Margins _margins;

    private Rectangle _clientRectangle;

    [Browsable(false)]
    public Rectangle ClientRectangle
    {
      get { return _clientRectangle; }
      set
      {
        _clientRectangle = value;
        _graphicRectangle = new Rectangle(_clientRectangle.Left + _margins.Left, _clientRectangle.Top + _margins.Top, _clientRectangle.Width - _margins.Left - _margins.Right, _clientRectangle.Height - _margins.Top - _margins.Bottom);

        AxisX.ScreenMin = _graphicRectangle.Left;
        AxisX.ScreenMax = _graphicRectangle.Right;
        AxisY.ScreenMin = _graphicRectangle.Bottom;
        AxisY.ScreenMax = _graphicRectangle.Top;

        CreateGradientBrushes();
      }
    }

    private Rectangle _graphicRectangle;

    [Browsable(false)]
    public Rectangle GraphicRectangle
    {
      get { return _graphicRectangle; }
    }

    private Pen _pnFrame;
    private Pen _pnGridMain;
    private Pen _pnGridAux;
    private Pen _pnSelection;
    private Pen _pnProgress;

    private Brush _brBackground;
    private Brush _brGraphicBackground;
    private Brush _brSelection;

    private Font _fntTickMarks;
    private Font _fntLabelsH;
    private Font _fntLabelsV;

    private List<Series> _seriesList;

    private List<Axis> _axisList;

    [Browsable(false)]
    public Axis AxisX
    {
      get { return _axisList[0]; }
    }

    [Browsable(false)]
    public Axis AxisY
    {
      get { return _axisList[1]; }
    }

    /*private int _yProgress;

    public int YProgress
    {
      get { return _yProgress; }
      set { _yProgress = value; }
    }*/

    //[TypeConverter(typeof(CollectionTypeConverter))]
    //[Editor(typeof(SeriesListCollectionEditor), typeof(UITypeEditor))]
    [Browsable(false)]
    public List<Series> SeriesList
    {
      get { return _seriesList; }
    }

    public Chart(ChartPanel chartPanel, string name)
    {
      _chartPanel = chartPanel;
      _title = name;

      _axisList = new List<Axis>();
      _axisList.Add(new Axis(this, "Theta", "Degrees"));
      _axisList.Add(new Axis(this, "R", "%"));

      _margins = new Margins(52, 24, 24, 44);

      _pnFrame = new Pen(Color.Silver, 1.0f);
      _pnGridMain = new Pen(Color.Gray, 1.0f);
      _pnGridAux = new Pen(Color.Gray, 1.0f);
      _pnGridAux.DashStyle = DashStyle.Dash;
      _pnSelection = new Pen(Color.FromKnownColor(KnownColor.HotTrack), 1.0f);
      _pnProgress = new Pen(Color.SpringGreen, 2.0f);

      _brSelection = new SolidBrush(Color.FromArgb(32, Color.FromKnownColor(KnownColor.HotTrack)));

      _fntTickMarks = new Font("Arial", 8.0f);
      _fntLabelsH = new Font("Arial", 8.0f, FontStyle.Bold);
      _fntLabelsV = new Font("Arial", 8.0f, FontStyle.Bold/*, GraphicsUnit.Pixel, Encoding. 0, true*/);

      _seriesList = new List<Series>();

      CreateGradientBrushes();
    }

    public void CreateGradientBrushes()
    {
      _brBackground = Brushes.White;
      _brGraphicBackground = Brushes.White;
      _chartPanel.Refresh();
      /*return;

      if (_clientRectangle.Width > 0 && _clientRectangle.Height > 0)
      {
        _brBackground = new LinearGradientBrush(_clientRectangle, Color.White, Color.Lavender, 90.0f);

        if (_graphicRectangle.Width > 0 && _graphicRectangle.Height > 0)
          _brGraphicBackground = new LinearGradientBrush(_graphicRectangle, Color.White, Color.Honeydew, 90.0f);

        //_chartPanel.Refresh();
      }*/
    }

    public void Draw(Graphics g)
    {
      g.SmoothingMode = SmoothingMode.AntiAlias;

      g.FillRectangle(_brBackground, _clientRectangle);

      Rectangle clipRectangle = new Rectangle(_graphicRectangle.Left, _graphicRectangle.Top, _graphicRectangle.Width + 1, _graphicRectangle.Height + 1);

      // Set clipping area
      g.Clip = new Region(clipRectangle);

      // Graphic background
      g.FillRectangle(_brGraphicBackground, _clientRectangle);

      // Series draw
      foreach (Series series in SeriesList)
        series.Draw(g);

      // Selections
      if (_chartPanel.SelectionRectangle != Rectangle.Empty)
      {
        g.FillRectangle(_brSelection, _chartPanel.SelectionRectangle);
        g.DrawRectangle(_pnSelection, _chartPanel.SelectionRectangle);
      }

      // Remove clipping area
      g.Clip = new Region();

      // Chart frame
      g.DrawRectangle(_pnFrame, _graphicRectangle);

      // Chart axis arrow params
      SizeF arrowSize = new SizeF(10.0f, 6.0f);
      float arrowGap = 6.0f;

      // AxisX Line
      float y = AxisY.GetDisplayValue(0.0);
      if (y > _graphicRectangle.Bottom)
        y = _graphicRectangle.Bottom;
      if (y < _graphicRectangle.Top)
        y = _graphicRectangle.Top;

      y = (int)(y + 0.5f); // Else thick line may occure

      g.DrawLine(_axisList[0].AxisPen, _graphicRectangle.Left, y, _graphicRectangle.Right + arrowSize.Width + arrowGap + 1, y);
      g.DrawLine(_axisList[0].AxisPen, _graphicRectangle.Right + arrowSize.Width / 2 + arrowGap, y, _graphicRectangle.Right + arrowGap, y + arrowSize.Height / 2);
      g.DrawLine(_axisList[0].AxisPen, _graphicRectangle.Right + arrowSize.Width / 2 + arrowGap, y, _graphicRectangle.Right + arrowGap, y - arrowSize.Height / 2);
      g.DrawLine(_axisList[0].AxisPen, _graphicRectangle.Right + arrowSize.Width + arrowGap, y, _graphicRectangle.Right + arrowGap, y + arrowSize.Height / 2);
      g.DrawLine(_axisList[0].AxisPen, _graphicRectangle.Right + arrowSize.Width + arrowGap, y, _graphicRectangle.Right + arrowGap, y - arrowSize.Height / 2);

      string sVal;
      SizeF labelSize;
      float maxSize = float.MinValue;

      // X tick marks
      foreach (double xVal in _axisList[0].TickMarks)
      {
        float xDisplayVal = AxisX.GetDisplayValue(xVal);

        if (xDisplayVal >= _graphicRectangle.Left && xDisplayVal <= _graphicRectangle.Right)
        {
          sVal = xVal.ToString("0.00");
          labelSize = g.MeasureString(sVal, _fntTickMarks);
          maxSize = Math.Max(maxSize, labelSize.Height);

          g.DrawLine(_axisList[0].AxisPen, xDisplayVal, _graphicRectangle.Bottom, xDisplayVal, _graphicRectangle.Bottom + arrowSize.Height / 2);
          g.DrawString(sVal, _fntTickMarks, Brushes.Black, xDisplayVal - labelSize.Width / 2, _graphicRectangle.Bottom + arrowSize.Height);
        }
      }

      // X label
      sVal = AxisX.Name;
      labelSize = g.MeasureString(sVal, _fntLabelsH);
      g.DrawString(sVal, _fntLabelsH, Brushes.Black, _graphicRectangle.Left + (_graphicRectangle.Width - labelSize.Width) / 2, _graphicRectangle.Bottom + maxSize + arrowSize.Height / 2 + arrowSize.Height);

      // X unit
      sVal = AxisX.Units;
      labelSize = g.MeasureString(sVal, _fntLabelsH);
      g.DrawString(sVal, _fntLabelsH, Brushes.Black, _graphicRectangle.Right - labelSize.Width, _graphicRectangle.Bottom + maxSize + arrowSize.Height / 2 + arrowSize.Height);

      // AxisY Line
      float x = AxisX.GetDisplayValue(0.0);
      if (x < _graphicRectangle.Left)
        x = _graphicRectangle.Left;
      if (x > _graphicRectangle.Right)
        x = _graphicRectangle.Right;

      x = (int)(x + 0.5f);

      g.DrawLine(_axisList[1].AxisPen, x, _graphicRectangle.Bottom, x, _graphicRectangle.Top - arrowSize.Width - arrowGap - 1);
      g.DrawLine(_axisList[0].AxisPen, x, _graphicRectangle.Top - arrowSize.Width / 2 - arrowGap, x + arrowSize.Height / 2, _graphicRectangle.Top - arrowGap);
      g.DrawLine(_axisList[0].AxisPen, x, _graphicRectangle.Top - arrowSize.Width / 2 - arrowGap, x - arrowSize.Height / 2, _graphicRectangle.Top - arrowGap);
      g.DrawLine(_axisList[0].AxisPen, x, _graphicRectangle.Top - arrowSize.Width - arrowGap, x + arrowSize.Height / 2, _graphicRectangle.Top - arrowGap);
      g.DrawLine(_axisList[0].AxisPen, x, _graphicRectangle.Top - arrowSize.Width - arrowGap, x - arrowSize.Height / 2, _graphicRectangle.Top - arrowGap);

      // Y tick marks
      
      maxSize = float.MinValue;

      sVal = _axisList[1].TickMarks[0].ToString("0.00");
      labelSize = g.MeasureString(sVal, _fntTickMarks);
      maxSize = Math.Max(maxSize, labelSize.Width);

      sVal = _axisList[1].TickMarks[_axisList[1].TickMarks.GetLength(0) - 1].ToString("0.00");
      labelSize = g.MeasureString(sVal, _fntTickMarks);
      maxSize = Math.Max(maxSize, labelSize.Width);

      foreach (double yVal in _axisList[1].TickMarks)
      {
        float yDisplayVal = AxisY.GetDisplayValue(yVal);

        if (yDisplayVal >= _graphicRectangle.Top && yDisplayVal <= _graphicRectangle.Bottom)
        {
          sVal = yVal.ToString("0.00");
          labelSize = g.MeasureString(sVal, _fntTickMarks);
          maxSize = Math.Max(maxSize, labelSize.Width);

          g.DrawLine(_axisList[1].AxisPen, _graphicRectangle.Left, yDisplayVal, _graphicRectangle.Left - arrowSize.Height / 2.0f, yDisplayVal);
          g.DrawString(sVal, _fntTickMarks, Brushes.Black, _graphicRectangle.Left - maxSize - arrowSize.Height / 2.0f, yDisplayVal - labelSize.Height / 2.0f);
        }
      }

      // Y label
      //sVal = AxisY.Name;
      //labelSize = g.MeasureString(sVal, _fntLabelsV);
      //g.DrawString(sVal, _fntLabelsV, Brushes.Black, 8.0f, _graphicRectangle.Top + (_graphicRectangle.Height - labelSize.Height) / 2.0f);

      // Y units
      sVal = AxisY.Units;
      labelSize = g.MeasureString(sVal, _fntLabelsH);
      g.DrawString(sVal, _fntLabelsH, Brushes.Black, 8.0f, _graphicRectangle.Top - arrowGap - labelSize.Height);
    }
    
    public void AutoFit()
    {
      for (int axisIndex = 0; axisIndex < _axisList.Count; axisIndex++)
      {
        Axis axis = _axisList[axisIndex];

        double min = double.MaxValue;
        double max = double.MinValue;

        foreach (Series series in _seriesList)
        {
          List<double> data = series.DataList[axisIndex];

          if (data == null)
            break;

          foreach (double val in data)
          {
            min = Math.Min(min, val);
            max = Math.Max(max, val);
          }
        }

        axis.Min = min;
        axis.Max = max;
      }

      // расчет ширины надписей и полей
      
      //_chartPanel.Refresh();
    }

    public void Refresh()
    {
      _chartPanel.Refresh();
    }
  }
}
