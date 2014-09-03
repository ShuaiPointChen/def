// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicIPAddressReader.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Common
{
    using System;
    using System.IO;
    using System.Net;

    using ExitGames.Logging;
    
    public static class PublicIPAddressReader
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static string[] lookupServiceUrls = new string[]
            {
                "http://licensesp.exitgames.com/echoip",
                "http://licensespch.exitgames.com/echoip",
                "http://api-sth01.exip.org/?call=ip",
                "http://api-ams01.exip.org/?call=ip", 
                "http://api-nyc01.exip.org/?call=ip",
            }; 


        public static IPAddress ParsePublicIpAddress(string publicIpAddressFromSettings)
        {
            if (string.IsNullOrEmpty(publicIpAddressFromSettings))
            {
                return LookupPublicIpAddress();
            }

            IPAddress ipAddress; 
            if (IPAddress.TryParse(publicIpAddressFromSettings, out ipAddress))
            {
                return ipAddress;
            }

            IPHostEntry hostEntry = Dns.GetHostEntry(publicIpAddressFromSettings);
            if (hostEntry.AddressList.Length > 0)
            {
                foreach (var entry in hostEntry.AddressList)
                {
                    if (entry.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ipAddress; 
                    }
                }
            }
            
            return LookupPublicIpAddress(); 
        }

        private static IPAddress LookupPublicIpAddress()
        {
            IPAddress result = null;

            if (lookupServiceUrls.Length == 0)
            {
                throw new InvalidOperationException("Could not lookup the public IP address: no Lookup Service URLs are defined.");
            }

            foreach (string url in lookupServiceUrls)
            {
                try
                {
                    result = DoLookupPublicIpAddress(url);
                    break;
                }
                catch (Exception)
                {
                }
            }

            if (result == null)
            {
                throw new InvalidOperationException("Could not retrieve the public IP address. Please make sure that internet access is available, or configure a fixed value for the PublicIPAddress in the app.config.");
            }
            
            return result; 
        }

        private static IPAddress DoLookupPublicIpAddress(string lookupServiceUrl)
        {
            WebResponse response = null;    
            Stream stream = null;

            try
            {
                WebRequest request = WebRequest.Create(lookupServiceUrl);
                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (stream == null)
                {
                    throw new InvalidOperationException(string.Format("Failed to lookup public ip address at {0}: No web response received", lookupServiceUrl));
                }

                string address;
                using (var reader = new StreamReader(stream))
                {
                    address = reader.ReadToEnd();
                }

                IPAddress result; 
                if (IPAddress.TryParse(address, out result) == false)
                {
                    throw new FormatException(string.Format("Failed to lookup public ip address at {0}: Parse address failed - Response = {1}", lookupServiceUrl, address));
                }

                return result; 
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Failed to lookup public ip address at {0}: {1}", lookupServiceUrl, ex);
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

                if (response != null)
                {
                    response.Close();
                }
            }        
        }

        public static bool IsLocalIpAddress(IPAddress hostIP)
        {
            // get local IP addresses
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            // is localhost
            if (IPAddress.IsLoopback(hostIP)) return true;

            // is local address
            foreach (IPAddress localIP in localIPs)
            {
                if (hostIP.Equals(localIP)) return true;
            }

            return false;
        }
    }
}