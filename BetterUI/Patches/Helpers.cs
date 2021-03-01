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
  }
}
