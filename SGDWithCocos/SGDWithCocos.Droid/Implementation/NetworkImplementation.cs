using SGDWithCocos.Droid.Implementation;
using SGDWithCocos.Interface;
using System.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkImplementation))]
namespace SGDWithCocos.Droid.Implementation
{
    public class NetworkImplementation : INetwork
    {
        public string GetIP()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());

            if (adresses != null && adresses[0] != null)
            {
                return adresses[0].ToString();
            }
            else
            {
                return null;
            }
        }
    }
}