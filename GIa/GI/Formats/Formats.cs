using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GI.Formats
{
    public class Formats
    {
        public static double GetDouble(string value)
        {
            double result;
            string s = value.Replace(".", ",");

            //Try parsing in the current culture
            double.TryParse(s, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result);
            double.TryParse(s, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result);

            
            return result;

        }

        public static string InitFormatSectionbuilder(string Section)
        {
            return $"\\[{Section}\\].*(?:\\r?\\n\\s*[^\\]\\[\\s].*)+";
        }

    }
}
