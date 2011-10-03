using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Chart
{
  public abstract class Series
  {
    protected Chart _chart;

    protected string _name;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    protected List<List<double>> _dataList;

    public List<List<double>> DataList
    {
      get { return _dataList; }
    }

    public Series(Chart chart, string name)
    {
      _chart = chart;
      _name = name;
      _dataList = new List<List<double>>();
    }

    public abstract void Draw(Graphics g);
  }
}
