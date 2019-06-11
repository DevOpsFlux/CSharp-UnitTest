using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Text;

namespace TLSTest
{
    class Program
    {
        // https://support.microsoft.com/ko-kr/help/3069494/cannot-connect-to-a-server-by-using-the-servicepointmanager-or-sslstre

        private const string DisableCachingName = @"TestSwitch.LocalAppContext.DisableCaching";
        private const string DontEnableSchUseStrongCryptoName = @"Switch.System.Net.DontEnableSchUseStrongCrypto";
        static void Main(string[] args)
        {
            //AppContext.SetSwitch(DisableCachingName, true);
            //AppContext.SetSwitch(DontEnableSchUseStrongCryptoName, true);

            // 4.6 : Tls, Tls11, Tls12
            // 4.5 이하 : Ssl3, Tls

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolTypeExtensions.Tls11 | SecurityProtocolTypeExtensions.Tls12;

            string strVersion = ServicePointManager.SecurityProtocol.ToString();
            Console.WriteLine("Test : " + strVersion);

            Console.WriteLine("Ssl3 : " + SecurityProtocolType.Ssl3.ToString());
            Console.WriteLine("Tls : " + SecurityProtocolType.Tls.ToString());



        }
    }

}

#region # SecurityProtocolTypeExtensions / SslProtocolsExtensions
namespace System.Security.Authentication
{
    public static class SslProtocolsExtensions
    {
        public const SslProtocols Tls12 = (SslProtocols)0x00000C00;
        public const SslProtocols Tls11 = (SslProtocols)0x00000300;
    }
}
namespace System.Net
{
    using System.Security.Authentication;
    public static class SecurityProtocolTypeExtensions
    {
        public const SecurityProtocolType Tls12 = (SecurityProtocolType)SslProtocolsExtensions.Tls12;
        public const SecurityProtocolType Tls11 = (SecurityProtocolType)SslProtocolsExtensions.Tls11;
        public const SecurityProtocolType SystemDefault = (SecurityProtocolType)0;
    }
}
#endregion
