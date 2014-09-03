// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Policy.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Application serving policy files in response of FlashPlayer requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Exitgames.Realtime.Policy.Application
{
    #region using directives

    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using log4net;
    using log4net.Config;

    using PhotonHostRuntimeInterfaces;

    #endregion

    /// <summary>
    ///   Application serving policy files in response of FlashPlayer requests.
    /// </summary>
    public class Policy : IPhotonApplication, IPhotonControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The logger.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///   The applicationPath
        /// </summary>
        private readonly string applicationPath;

        /// <summary>
        ///   Name/ID of this CLR application.
        /// </summary>
        private string applicationId;

        /// <summary>
        ///   The policy file.
        /// </summary>
        private byte[] policyBytesUtf8;

        /// <summary>
        ///   The silverlight policy file.
        /// </summary>
        private byte[] silverlightPolicyBytesUtf8;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Policy" /> class.
        /// </summary>
        public Policy()
        {
            this.applicationPath = Environment.CurrentDirectory;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Converts a UTF8-string to a byte array.
        /// </summary>
        /// <param name = "str">The STR.</param>
        /// <returns>Bytes of a utf8 encoded string</returns>
        public static byte[] StrToByteArray(string str)
        {
            Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        // usually the Photon.SocketServer.Application provides the path but Policy is not inheriting that

        /// <summary>
        ///   OnDisconnect callback.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "userData">
        ///   The user data.
        /// </param>
        /// <param name = "reasonCode">Disconnect reason code.</param>
        /// <param name = "reasonDetail">Disconnect reason details.</param>
        /// <param name = "rtt">The round trip time.</param>
        /// <param name = "rttVariance">The round trip time variance.</param>
        /// <param name = "numFailures">The number of failures. </param>
        public void OnDisconnect(
            IPhotonPeer peer, 
            object userData, 
            DisconnectReason reasonCode, 
            string reasonDetail, 
            int rtt, 
            int rttVariance, 
            int numFailures)
        {
            if (log.IsInfoEnabled && reasonCode != DisconnectReason.ClientDisconnect && reasonCode != DisconnectReason.ManagedDisconnect)
            {
                log.InfoFormat(
                    "OnDisconnect: PID {0}, {1} ({2}), RTT: {3}, Variance: {4}, Failures:{5}",
                    peer.GetConnectionID(),
                    reasonCode,
                    reasonDetail,
                    rtt,
                    rttVariance,
                    numFailures);
            }
            else if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "OnDisconnect: PID {0}, {1} ({2}), RTT: {3}, Variance: {4}, Failures:{5}",
                    peer.GetConnectionID(),
                    reasonCode,
                    reasonDetail,
                    rtt,
                    rttVariance,
                    numFailures);
            }
        }

        /// <summary>
        ///   OnFlowControlEvent callback.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "userData">
        ///   The user data.
        /// </param>
        /// <param name = "flowControlEvent">
        ///   The flow control event.
        /// </param>
        public void OnFlowControlEvent(IPhotonPeer peer, object userData, FlowControlEvent flowControlEvent)
        {
        }

        /// <summary>
        ///   Called when [init].
        /// </summary>
        /// <param name = "peer">The peer.</param>
        /// <param name = "data">The data.</param>
        public void OnInit(IPhotonPeer peer, byte[] data, byte channelCount)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    Encoding utf8Encoding = Encoding.UTF8;
                    log.DebugFormat("OnInit - {0}", utf8Encoding.GetString(data));
                }

                if (data[0] == '<' && data[21] == '>')
                {
                    byte[] bytes = peer.GetLocalPort() == 943 ? this.silverlightPolicyBytesUtf8 : this.policyBytesUtf8;

                    // in case the policy app ever serves a websocket port...
                    MessageContentType contentType = peer.GetListenerType() == ListenerType.WebSocketListener
                                                         ? MessageContentType.Text
                                                         : MessageContentType.Binary; 

                    peer.Send(bytes, MessageReliablity.Reliable, 0, contentType);

                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Policy sent.");
                    }
                }

                peer.DisconnectClient(); // silverlight does not disconnect by itself
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }

        /// <summary>
        ///   OnOutboundConnectionEstablished callback.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "userData">
        ///   The user data.
        /// </param>
        public void OnOutboundConnectionEstablished(IPhotonPeer peer, byte[] data, object userData)
        {
        }

        /// <summary>
        ///   OnOutboundConnectionFailed callback.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "userData">
        ///   The user data.
        /// </param>
        /// <param name = "errorCode">
        ///   The error code.
        /// </param>
        /// <param name = "errorMessage">
        ///   The error message.
        /// </param>
        public void OnOutboundConnectionFailed(IPhotonPeer peer, object userData, int errorCode, string errorMessage)
        {
        }

        /// <summary>
        ///   Photon is now running.
        /// </summary>
        public void OnPhotonRunning()
        {
            SetupLog4Net();

            if (log.IsInfoEnabled)
            {
                log.Info(
                    "Application start. AppId: " + this.applicationId + " ApplicationPath: " + this.applicationPath);
            }

            try
            {
                this.policyBytesUtf8 = ReadPolicyFile(Path.Combine(this.applicationPath, "assets/socket-policy.xml"));
                this.silverlightPolicyBytesUtf8 =
                    ReadPolicyFile(Path.Combine(this.applicationPath, "assets/socket-policy-silverlight.xml"));
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }

        private void SetupLog4Net()
        {
            var appDomainInfo = AppDomain.CurrentDomain.SetupInformation;

            if (appDomainInfo.AppDomainInitializerArguments != null && appDomainInfo.AppDomainInitializerArguments.Length >= 4)
            {
                var unmanagedLogPath = appDomainInfo.AppDomainInitializerArguments[1];
                log4net.GlobalContext.Properties["Photon:UnmanagedLogDirectory"] = unmanagedLogPath;

                var appRootPath = appDomainInfo.AppDomainInitializerArguments[3];
                log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(appRootPath, "log");
            }
            else
            {
                log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(
                    this.applicationPath, "..\\log");
            }
            string path = Path.Combine(this.applicationPath, "assets\\log4net.config");
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                XmlConfigurator.Configure(fileInfo);
            }
        }

        /// <summary>
        ///   OnReceive callback.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "userData">
        ///   The user data.
        /// </param>
        /// <param name = "data">
        ///   The data.
        /// </param>
        /// <param name = "reliability">
        ///   The reliability.
        /// </param>
        /// <param name = "channelId">
        ///   The channel id.
        /// </param>
        /// /// <param name = "rtt">The round trip time.</param>
        /// <param name = "rttVariance">The round trip time variance.</param>
        /// <param name = "numFailures">The number of failures. </param>
        public void OnReceive(
            IPhotonPeer peer, 
            object userData, 
            byte[] data, 
            MessageReliablity reliability, 
            byte channelId, 
            MessageContentType messageContentType,
            int rtt, 
            int rttVariance, 
            int numFailures)
        {
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        /// <param name="instanceName">
        /// The instance name.
        /// </param>
        /// <param name="applicationName">
        /// Name/ID of the application.
        /// </param>
        /// <param name="sink">
        /// The PhotonApplicationSink.
        /// </param>
        /// <param name="listenerControl">
        /// The listener Control.
        /// </param>
        /// <param name="unmanagedLogDirectory">
        /// Photon's log path.
        /// </param>
        /// <returns>
        /// PhotonApplication object.
        /// </returns>
        public IPhotonApplication OnStart(string instanceName, string applicationName, IPhotonApplicationSink sink, IControlListeners listenerControl, IPhotonApplicationsCounter applicationsCounter, string unmanagedLogDirectory)
        {
            // this app does not inherit from Photon.SocketServer.Application, so it's missing the ApplicationPath
            // get the ApplicationPath "manually": Environment.CurrentDirectory is overwritten when the next app starts, so copy the value
            this.applicationId = applicationName;
            return this;
        }


        /// <summary>
        ///   Called when the application stops.
        /// </summary>
        public void OnStop()
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Application stop. AppId: " + this.applicationId);
            }
        }

        /// <summary>
        ///   OnStopRequested callback.
        /// </summary>
        public void OnStopRequested()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Reads a Policy File.
        /// </summary>
        /// <param name = "fileName">
        ///   The file name.
        /// </param>
        /// <returns>
        ///   The bytes of a utf8 encoded policy file.
        /// </returns>
        private static byte[] ReadPolicyFile(string fileName)
        {
            byte[] policyUtf8Binary;
            string policy;

            if (log.IsDebugEnabled)
            {
                log.Debug("Reading policy file: " + fileName);
            }

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (var sr = new StreamReader(fileName))
            {
                Encoding utf8Encoding = Encoding.UTF8;
                policy = sr.ReadToEnd();

                ////// add a \0 to the end, if needed
                ////if (nullTerminated)
                ////{
                ////    if (policy[policy.Length - 1] != '\0')
                ////    {
                ////        policy += '\0';
                ////    }
                ////}
                policyUtf8Binary = utf8Encoding.GetBytes(policy);
            }

            if (log.IsInfoEnabled)
            {
                log.InfoFormat("Policy file: \n{0}", policy);
            }

            return policyUtf8Binary;
        }

        #endregion
    }
}