using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SitesMonitoring.Models;

namespace SitesMonitoring
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ThreadTask threadTask = new ThreadTask();
            try
            {
            
                threadTask.Start();
                BuildWebHost(args).Run();
                
                threadTask.Stop();
            }
            catch
            {
                threadTask.Stop();

            }
        }
            public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
