using NzbDroneWatcher.Objects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;

namespace NzbDroneWatcher
{
    static class Program
    {
        public static void Main()
        {
            // DEBUG
            //new Service().Test();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service()
            };
            ServiceBase.Run(ServicesToRun);
        }

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static List<ServiceItem> ServiceItems
        {
            get
            {
                var list = new List<ServiceItem>();
                var serviceArray = ConfigurationManager.AppSettings["ServicesToWatch"].Split('|');

                foreach (var item in serviceArray)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var serviceProcess = item.Replace("[", "").Replace("]", "").Split(';');

                        if (serviceProcess != null && serviceProcess.Count() == 3)
                        {
                            list.Add(new ServiceItem
                            {
                                ServiceName = serviceProcess[0],
                                ProcessName = serviceProcess[1],
                                AppUrl = serviceProcess[2]
                            });
                        }
                        if (serviceProcess != null && serviceProcess.Count() == 2)
                        {
                            list.Add(new ServiceItem
                            {
                                ServiceName = serviceProcess[0],
                                ProcessName = serviceProcess[1]
                            });
                        }
                    }
                }

                return list;
            }
        }

        public static int IntervalMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);
            }
        }

        public static string EmailSmtp
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailSmtp"];
            }
        }

        public static int EmailPort
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);
            }
        }

        public static bool EmailSsl
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailSsl"].ToLower() == "yes" ||
                       ConfigurationManager.AppSettings["EmailSsl"].ToLower() == "true"
                            ? true
                            : false;
            }
        }

        public static string EmailUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailUserName"];
            }
        }

        public static string EmailPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailPassword"];
            }
        }
        public static string EmailFrom
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailFrom"];
            }
        }

        public static string EmailTo
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailTo"];
            }
        }
    }
}
