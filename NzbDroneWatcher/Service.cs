using NzbDroneWatcher.Objects;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

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


                Thread.Sleep(30000); // 30 seconds

                CheckAndEmailOfAnyError(serviceItem);
            }
        }

        private static void CheckAndEmailOfAnyError(ServiceItem serviceItem)
        {
            if (!string.IsNullOrEmpty(serviceItem.AppUrl))
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = Task.Run(() => client.GetAsync(serviceItem.AppUrl)).Result;

                        if (response.IsSuccessStatusCode)
                            Console.WriteLine($"{serviceItem.AppUrl} is up and running.");
                        else
                            SendEmailNotification(serviceItem.AppUrl, serviceItem.ServiceName);
                    }
                    catch
                    {
                        SendEmailNotification(serviceItem.AppUrl, serviceItem.ServiceName);
                    }
                }
            }
        }

        private static void SendEmailNotification(string websiteUrl, string serviceName)
        {
            // Set up the email details
            string smtpAddress = Program.EmailSmtp;
            int portNumber = Program.EmailPort;
            bool enableSSL = Program.EmailSsl;
            string emailFrom = Program.EmailFrom;
            string password = Program.EmailPassword;
            string emailTo = Program.EmailTo;
            string subject = $"NZB Drone Watcher: {serviceName} Down!";
            string body = $"<b>NZB Drone Watcher</b><br /><br />The website {websiteUrl} appears to be down. Check the issue.<br /><br />";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch
                    { }
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
