using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GrimrockModelToolkit.Grim3d
{
    public class Logger
    {
        static Lazy<Logger> s_instance = new Lazy<Logger>(() => new Logger());

        public static Logger Instance { get { return s_instance.Value; } }

        public void Warning(string str)
        {
            Debug.WriteLine("[WW] - " + str);
        }

        public void Info(string str)
        {
            Debug.WriteLine("[ii] - " + str);
        }

        public void Error(string str)
        {
            Debug.WriteLine("[EE] - " + str);
        }

        public void Verbose(string str)
        {
            //Debug.WriteLine("[..] - " + str);
        }



    }
}
