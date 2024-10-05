using System;

namespace NzbDroneWatcher.Objects
{
    public class ServiceItem
    {
        public string ServiceName { get; set; }
        public string ProcessName { get; set; }
        public string AppUrl { get; set; } = null;
        public DateTime? LastEmailDateTime { get; set; } = null;
    }
}
