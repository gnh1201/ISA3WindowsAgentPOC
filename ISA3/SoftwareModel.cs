using System;

namespace ISA3
{
    public class SoftwareModel
    {
        public Int32 ID { get; set; }
        public string Status { get; set; }
        public Int32 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Origin { get; set; }
        public string ResourceName { get; set; }
        public string Default { get; set; }
        public string InstallDate { get; set; }
        public string InstallLocation { get; set; }
        public string Publisher { get; set; }
        public string DisplayIcon { get; set; }
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string HelpLink { get; set; }
        public string UninstallString { get; set; }
        public Int32 GroupID { get; set; }
        public Int32 AssignID { get; set; }
        public Int32 DeviceID { get; set; }
    }
}
