using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GIa.Netzwerk
{
    internal class Ip
    {
        public static bool IsValidIpAdress (string ipAddress)
        {
            Regex regxValidateIp = new Regex("\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}");
            return regxValidateIp.IsMatch (ipAddress);
        }
    }
}
