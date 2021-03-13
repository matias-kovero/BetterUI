using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterUI.Patches
{
  class Helpers
  {
    public static string Repeat(string value, int count)
    {
      return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
    }

    public static string TimeString(double val1, double val2)
    {
      TimeSpan t = TimeSpan.FromSeconds(val1 - val2);
      return t.Hours > 0 ?
        string.Format("{0:D2}h {1:D2}m {2:D2}s", t.Hours, t.Minutes, t.Seconds) : t.Minutes > 0 ? 
        string.Format("{0:D2}m {1:D2}s", t.Minutes, t.Seconds) : string.Format("{0:D2}s", t.Seconds);
    }

    public static string TimeString(double seconds)
    {
      TimeSpan t = TimeSpan.FromSeconds(seconds);
      return t.Hours > 0 ?
        string.Format("{0:D2}h {1:D2}m {2:D2}s", t.Hours, t.Minutes, t.Seconds) : t.Minutes > 0 ?
        string.Format("{0:D2}m {1:D2}s", t.Minutes, t.Seconds) : string.Format("{0:D2}s", t.Seconds);
    }
  }
}
