/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace Dover.Framework.Service
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
