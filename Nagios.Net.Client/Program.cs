using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Nagios.Net.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if ((1 == args.Length) && ("-runAsApp".ToUpper() == args[0].ToUpper()))
            {
                Main m = new Main();
                m.Config();
                Console.WriteLine("Modules loaded and configured...");
                m.RunModules();
                Console.WriteLine("Run modules...");
                Console.WriteLine("Press any key for exit...");
                Console.ReadKey();
            }
            else
            {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
            { 
                new NService() 
            };

                ServiceBase.Run(ServicesToRun);
            }

        }
    }
}
