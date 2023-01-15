using System.ServiceProcess;
using System.Timers;
using System.Threading;
using System;
using System.Linq;

namespace NzbDroneWatcher
{
    public partial class Service : ServiceBase
    {
        private System.Timers.Timer timer = new System.Timers.Timer();

        public Service()
        {
            InitializeComponent();
        }

        public void Test()
        {
            OnStart(new string[] { "test" });
            while (true)
                Thread.Sleep(100);
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]) && args[0] == "test")
                timer.Interval = 60000; // 60 seconds
            else
                timer.Interval = Program.IntervalMinutes * 60 * 1000;

            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckAndRestartNzbServices();
        }

        private static void CheckAndRestartNzbServices()
        {
            foreach (var serviceItem in Program.ServiceItems)
            {
                var serviceController = new ServiceController(serviceItem.ServiceName, "localhost");
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    if (serviceItem.ServiceName.Contains("Radarr"))
                    {
                        var radarrService = new ServiceController("Radarr", "localhost");
                        var radarr4KService = new ServiceController("Radarr-4K", "localhost");

                        try
                        {
                            radarrService.Stop();
                        }
                        catch (Exception ex)
                        {
                            var _ = ex;
                            if (Program.IsDebug)
                                Console.Out.WriteLine(ex.Message);
                        }

                        try
                        {
                            radarr4KService.Stop();
                        }
                        catch (Exception ex)
                        {
                            var _ = ex;
                            if (Program.IsDebug)
                                Console.Out.WriteLine(ex.Message);
                        }

                        Thread.Sleep(20000); // 20 seconds

                        try
                        {
                            if (radarr4KService.Status == ServiceControllerStatus.Running || radarr4KService.Status == ServiceControllerStatus.StopPending)
                                KillProcess(Program.ServiceItems.Where(s => s.ServiceName == "Radarr-4K").FirstOrDefault().ProcessName);
                        }
                        catch { }

                        try
                        {
                            if (radarrService.Status == ServiceControllerStatus.Running || radarrService.Status == ServiceControllerStatus.StopPending)
                                KillProcess(Program.ServiceItems.Where(s => s.ServiceName == "Radarr").FirstOrDefault().ProcessName);
                        }
                        catch { }

                        try
                        {
                            radarrService.Start();
                        }
                        catch (Exception ex)
                        {
                            var _ = ex;
                            if (Program.IsDebug)
                                Console.Out.WriteLine(ex.Message);
                        }

                        Thread.Sleep(20000); // 20 seconds

                        try
                        {
                            radarr4KService.Start();
                        }
                        catch (Exception ex)
                        {
                            var _ = ex;
                            if (Program.IsDebug)
                                Console.Out.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            serviceController.Start();
                        }
                        catch (Exception ex)
                        {
                            var _ = ex;
                            if (Program.IsDebug)
                                Console.Out.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        private static void KillProcess(string processName)
        {
            var processes = System.Diagnostics.Process.GetProcessesByName(processName);
            if (processes != null && processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch { }
                }
            }
        }

        protected override void OnStop()
        {
            try
            {
                timer.Stop();
            }
            catch { }

            try
            {
                timer.Enabled = false;
            }
            catch { }
        }
    }
}
