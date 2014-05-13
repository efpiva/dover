using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace AddOne.Framework.Service
{
    public class MachineInformation
    {
        private string ip;
        private string macAddress;
        private string machineName;
        private string windowsUser;

        public string IP { get { return ip; } }
        public string MacAddress { get { return macAddress; } }
        public string MachineName { get { return machineName; } }
        public string WindowsUser { get { return windowsUser; } }

        public MachineInformation()
        {
            machineName = System.Environment.MachineName.ToString();

            IPHostEntry iphostentry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress itemIP in iphostentry.AddressList)
            {
                ip = itemIP.ToString();
                if (itemIP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = itemIP.ToString();
                    break;
                }
            }

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
            {
                macAddress = "Not Found";
            }
            else
            {
                foreach (NetworkInterface adapter in nics)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();

                    if (adapter.OperationalStatus.ToString().Trim() == "Up")
                    {
                        macAddress = adapter.GetPhysicalAddress().ToString();
                        break;
                    }
                }
            }

            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            int _barra = wp.Identity.Name.LastIndexOf('\\') + 1;

            windowsUser = wp.Identity.Name.Substring(_barra, wp.Identity.Name.Length - _barra);
        }

    }
}
