using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Chart
{
  public static class ColorWheel
  {
    public static Color GetGrayscale(double val, double min, double max)
    {
      int r;
      int g;
      int b;

      r = g = b = (int)((val - min) / (max - min) * 255.0);

      r = Math.Min(r, 255);
      r = Math.Max(r, 0);
      g = Math.Min(g, 255);
      g = Math.Max(g, 0);
      b = Math.Min(b, 255);
      b = Math.Max(b, 0);

      return Color.FromArgb(r, g, b);
    }
  }
}
