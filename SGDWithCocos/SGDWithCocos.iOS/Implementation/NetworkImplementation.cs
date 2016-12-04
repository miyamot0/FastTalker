
using System;
using SGDWithCocos.Interface;
using Xamarin.Forms;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SGDWithCocos.iOS.Implementation;

[assembly: Dependency(typeof(NetworkImplementation))]
namespace SGDWithCocos.iOS.Implementation
{
    public class NetworkImplementation : INetwork
    {
        public string GetIP()
        {
            String ipAddress = "";

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddress = addrInfo.Address.ToString();
                        }
                    }
                }
            }

            return ipAddress;
        }
    }
}