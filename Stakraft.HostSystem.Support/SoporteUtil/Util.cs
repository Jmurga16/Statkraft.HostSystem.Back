using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stakraft.HostSystem.Support.SoporteUtil
{
    public static class Util
    {
        public static bool EsFecha(string fecha)
        {
            try
            {
                DateTime.Parse(fecha);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
