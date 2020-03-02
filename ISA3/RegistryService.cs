using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace ISA3
{
    class RegistryService
    {
        public static List<SoftwareModel> GetInstalledSoftwares()
        {
            List<SoftwareModel> items = new List<SoftwareModel>();
            List<string> diaplayNameIndex = new List<string>();

            Dictionary<string, RegistryKey> regKeys = new Dictionary<string, RegistryKey>();
            regKeys.Add("CurrentUser", Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"));
            regKeys.Add("LocalMachine", Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"));
            regKeys.Add("LocalMachineWow64", Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"));

            foreach (KeyValuePair<string, RegistryKey> rk in regKeys)
            {
                foreach (string skName in rk.Value.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.Value.OpenSubKey(skName))
                    {
                        try
                        {
                            string displayName = (string)sk.GetValue("DisplayName");
                            if (string.IsNullOrEmpty(displayName) || diaplayNameIndex.Contains(displayName))
                            {
                                continue; // exclude empty or duplicate item
                            }

                            string displayVersion = (string)sk.GetValue("DisplayVersion");
                            if (string.IsNullOrEmpty(displayVersion))
                            {
                                displayVersion = "알 수 없음";
                            }

                            string publisher = (string)sk.GetValue("Publisher");
                            if (string.IsNullOrEmpty(publisher))
                            {
                                publisher = "알 수 없음";
                            }

                            string installDate = (string)sk.GetValue("InstallDate");
                            if (string.IsNullOrEmpty(installDate))
                            {
                                installDate = "알 수 없음";
                            }

                            items.Add(new SoftwareModel
                            {
                                Origin = rk.Key,
                                ResourceName = (string)sk.GetValue("ResourceName"),
                                Default = (string)sk.GetValue(null),
                                InstallDate = installDate,
                                InstallLocation = (string)sk.GetValue("InstallLocation"),
                                Publisher = publisher,
                                DisplayIcon = (string)sk.GetValue("DisplayIcon"),
                                DisplayName = displayName,
                                DisplayVersion = displayVersion,
                                HelpLink = (string)sk.GetValue("HelpLink"),
                                UninstallString = (string)sk.GetValue("UninstallString")
                            });

                            diaplayNameIndex.Add(displayName);
                        }
                        catch (Exception)
                        {
                            // pass
                        }
                    }
                }
            }

            return items;
        }
    }
}
