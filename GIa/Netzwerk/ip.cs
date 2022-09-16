using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GIa.Netzwerk
{
    internal class ip
    {
        public static bool IsValidIPAdress (string IPAddress)
        {
            Regex regxValidateIP = new Regex("\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}");
            return regxValidateIP.IsMatch (IPAddress);
        }
    }
}
