// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Connected.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using ExitGames.Client.Photon;
    using ExitGames.Client.Photon.LoadBalancing;

    using Lite.Operations;

    using NUnit.Framework;

    using Photon.LoadBalancing.UnitTests.Client;

    using EventCode = ExitGames.Client.Photon.LoadBalancing.EventCode;
    using OperationCode = ExitGames.Client.Photon.LoadBalancing.OperationCode;

    /// <summary>
    ///   Test cases for the LoadBalancing applications. 
    ///   Requires that Photon's "LoadBalancing" is started with Master, Game1 and Game2 application.
    /// </summary>
    [TestFixture]
    public class ApiTests
    {
        #region Constants and Fields

        private static string appId = "Master";

        private static string appVersion = "1.0";

        private static IPEndPoint endPointGame1 = new IPEndPoint(IPAddress.Loopback, 4531);

        private static IPEndPoint endPointGame2 = new IPEndPoint(IPAddress.Loopback, 4532);

        private static IPEndPoint endPointMaster = new IPEndPoint(IPAddress.Loopback, 4530);

        private static string player1 = "Player1";

        private static string player2 = "Player2";

        private static int waitTime = 5000;

        #endregion

        //private AutoResetEvent connected;

        #region Public Methods

        [SetUp]
        [Test]
        public void ConnectionTest()
        {
            // master: 
            TcpClient client = new TcpClient();
            client.Connect(endPointMaster);
            AssertConnected(client);
            client.Close();

            // game1: 
            client = new TcpClient();
            client.Connect(endPointGame1);
            AssertConnected(client);
            client.Close();

            // game2: 
            client = new TcpClient();
            client.Connect(endPointGame2);
            AssertConnected(client);
            client.Close();
        }

        [Test]
        public void CreateGameTwice()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);

            try
            {
                // create
                string roomName = "CreateGameTwice_" + Guid.NewGuid().ToString().Substring(0, 6);
                Assert.IsTrue(
                    masterClient1.PhotonClient.OpCreateRoom(
                        roomName, true, true, 0, new Hashtable(), new string[0], new Hashtable()));
                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                Assert.IsTrue(
                    masterClient1.PhotonClient.OpCreateRoom(
                        roomName, true, true, 0, new Hashtable(), new string[0], new Hashtable()));
                operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.GameIdAlreadyExists,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
            }
        }

        [Test]
        public void InvisibleGame()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);
            Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
            masterClient1.WaitForOperationResponse(1000); 
            masterClient1.OperationResponseQueue.Clear();

            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);
            masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient2.WaitForConnect(waitTime);
            Assert.IsTrue(masterClient2.PhotonClient.OpJoinLobby());
            masterClient2.WaitForOperationResponse(1000); 
            masterClient2.OperationResponseQueue.Clear();

            var gameClient1 = new TestClient(ConnectionProtocol.Tcp);
            var gameClient2 = new TestClient(ConnectionProtocol.Tcp);
            try
            {
                string gameServerIP;
                int gameServerPort;
                string roomName = "InvisibleGame_" + Guid.NewGuid().ToString().Substring(0, 6);
                 
                CreateRoomOnGameServer(
                    masterClient1, false, true, 10, roomName, out gameServerIP, out gameServerPort, out gameClient1);

                // join random 2nd client on master - no match found: 
                Assert.IsTrue(
                    masterClient2.PhotonClient.OpJoinRandomRoom(
                        new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom));
                var operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.NoMatchFound,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // join 2nd client on master - ok: 
                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRoom(roomName, new Hashtable()));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);


                // join directly on GS - game full: 
                gameClient2.Connect(gameServerIP, gameServerPort, appId);
                gameClient2.WaitForConnect(waitTime);

                gameClient2.PhotonClient.OpJoinRoom(roomName, null);
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }


        [Test]
        public void ClosedGame()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);
            Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
            masterClient1.WaitForOperationResponse(1000); 
            masterClient1.OperationResponseQueue.Clear();

            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);
            masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient2.WaitForConnect(waitTime);
            Assert.IsTrue(masterClient2.PhotonClient.OpJoinLobby());
            masterClient2.WaitForOperationResponse(1000); 
            masterClient2.OperationResponseQueue.Clear();

            var gameClient1 = new TestClient(ConnectionProtocol.Tcp);
            var gameClient2 = new TestClient(ConnectionProtocol.Tcp);
            
            try
            {
                string gameServerIP;
                int gameServerPort;
                string roomName = "ClosedGame_" + Guid.NewGuid().ToString().Substring(0, 6);

                CreateRoomOnGameServer(
                    masterClient1, true, false, 10, roomName, out gameServerIP, out gameServerPort, out gameClient1);

                // join 2nd client on master - closed: 
                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRoom(roomName, new Hashtable()));
                var operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.GameClosed,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // join random 2nd client on master - no match found: 
                Assert.IsTrue(
                    masterClient2.PhotonClient.OpJoinRandomRoom(
                        new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.NoMatchFound,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // join directly on GS - game closed: 
                gameClient2.Connect(gameServerIP, gameServerPort, appId);
                gameClient2.WaitForConnect(waitTime);

                gameClient2.PhotonClient.OpJoinRoom(roomName, null);
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.GameClosed,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }

        [Test]
        public void LobbyGameListEvents()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);

            TestClient gameClient1 = new TestClient(ConnectionProtocol.Tcp); 

            try
            {
                masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient1.WaitForConnect(waitTime);

                Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
                var ev = masterClient1.WaitForEvent(EventCode.GameList, waitTime);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                var gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                Assert.AreEqual(0, gameList.Count);

                masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient2.WaitForConnect(waitTime);

                Assert.IsTrue(masterClient2.PhotonClient.OpJoinLobby());
                ev = masterClient2.WaitForEvent(EventCode.GameList, waitTime);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                Assert.AreEqual(0, gameList.Count);

                // join lobby again: 
                masterClient1.OperationResponseQueue.Clear();
                Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinLobby, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);


                masterClient1.EventQueue.Clear();
                masterClient2.EventQueue.Clear();

                // open game

                string roomName = "LobbyGamelistEvents_1_" + Guid.NewGuid().ToString().Substring(0, 6);
                string gameServerIP;
                int gameServerPort;
                
                CreateRoomOnGameServer(masterClient1, true, true, 0, roomName, out gameServerIP, out gameServerPort, out gameClient1);


                var timeout = Environment.TickCount + 10000; 

                bool gameListUpdateReceived = false;
                bool appStatsReceived = false;  

                while (Environment.TickCount < timeout && (!gameListUpdateReceived || !appStatsReceived))
                {
                    try
                    {
                        ev = masterClient2.WaitForEvent(1000);

                        if (ev.Code == EventCode.AppStats)
                        {
                            appStatsReceived = true;
                            Assert.AreEqual(1, ev[ParameterCode.GameCount]);
                        }
                        else if (ev.Code == EventCode.GameListUpdate)
                        {
                            gameListUpdateReceived = true;
                            var roomList = (Hashtable)ev.Parameters[(byte)ParameterCode.GameList];
                            Assert.AreEqual(1, roomList.Count);
                            
                            var room = (Hashtable)roomList[roomName];
                            Assert.IsNotNull(room);
                            Assert.AreEqual(3, room.Count);

                            Assert.IsNotNull(room[GameProperties.IsOpen], "IsOpen");
                            Assert.IsNotNull(room[GameProperties.MaxPlayers], "MaxPlayers");
                            Assert.IsNotNull(room[GameProperties.PlayerCount], "PlayerCount");
                            
                            Assert.AreEqual(true, room[GameProperties.IsOpen]);
                            Assert.AreEqual(0, room[GameProperties.MaxPlayers]);
                            Assert.AreEqual(1, room[GameProperties.PlayerCount]);
                        }
                    }
                    catch (TimeoutException)
                    {
                    }
                }

                Assert.IsTrue(gameListUpdateReceived, "GameListUpdate event received");
                Assert.IsTrue(appStatsReceived, "AppStats event received");
                
                
                gameClient1.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave });

                gameListUpdateReceived = false;
                appStatsReceived = false;

                timeout = Environment.TickCount + 10000;
                while (Environment.TickCount < timeout && (!gameListUpdateReceived || !appStatsReceived))
                {
                    try
                    {
                        ev = masterClient2.WaitForEvent(1000);

                        if (ev.Code == EventCode.AppStats)
                        {
                            appStatsReceived = true;
                            Assert.AreEqual(0, ev[ParameterCode.GameCount]);
                        }

                        if (ev.Code == EventCode.GameListUpdate)
                        {
                            gameListUpdateReceived = true;
                            
                            var roomList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                            Assert.AreEqual(1, roomList.Count);
                            var room = (Hashtable)roomList[roomName];
                            Assert.IsNotNull(room);

                            Assert.AreEqual(1, room.Count);
                            Assert.IsNotNull(room[GameProperties.Removed], "Removed");
                            Assert.AreEqual(true, room[GameProperties.Removed]);
                        }
                    }
                    catch (TimeoutException)
                    {
                    }
                }

                Assert.IsTrue(gameListUpdateReceived, "GameListUpdate event received");
                Assert.IsTrue(appStatsReceived, "AppStats event received");
                
                // leave lobby
                masterClient2.PhotonClient.OpLeaveLobby();

                gameListUpdateReceived = false;
                appStatsReceived = false;

                masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient1.WaitForConnect(waitTime);

                roomName = "LobbyGamelistEvents_2_" + Guid.NewGuid().ToString().Substring(0, 6);

                CreateRoomOnGameServer(masterClient1, true, true, 0, roomName, out gameServerIP, out gameServerPort, out gameClient1);

                timeout = Environment.TickCount + 10000;

                while (Environment.TickCount < timeout && (!gameListUpdateReceived || !appStatsReceived))
                {
                    try
                    {
                        ev = masterClient2.WaitForEvent(1000);

                        if (ev.Code == EventCode.AppStats)
                        {
                            appStatsReceived = true;
                            Assert.AreEqual(1, ev[ParameterCode.GameCount]);
                        }

                        if (ev.Code == EventCode.GameListUpdate)
                        {
                            gameListUpdateReceived = true;
                        }
                    }
                    catch (TimeoutException)
                    {
                    }

                }
                Assert.IsFalse(gameListUpdateReceived, "GameListUpdate event received");
                Assert.IsTrue(appStatsReceived, "AppStats event received"); 
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }

                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }
            }
        }

        [Test]
        public void JoinNoMatchFound()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);

            try
            {
                masterClient1.EventQueue.Clear();

                Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
                var ev = masterClient1.WaitForEvent(EventCode.GameList, waitTime);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                var gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                Assert.AreEqual(0, gameList.Count);

                masterClient1.OperationResponseQueue.Clear();
                masterClient1.EventQueue.Clear();

                string roomName = "JoinNoMatchFound_" + Guid.NewGuid().ToString().Substring(0, 6);

                Assert.IsTrue(masterClient1.PhotonClient.OpJoinRoom(roomName, new Hashtable()));
                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.GameIdNotExists,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
            }
        }

        [Test]
        public void JoinOnGameServer()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);

            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);
            masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient2.WaitForConnect(waitTime);

            var gameClient1 = new TestClient(ConnectionProtocol.Tcp);

            var gameClient2 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                // create
                string roomName = "JoinOnGameServer_" + Guid.NewGuid().ToString().Substring(0, 6);
                Assert.IsTrue(
                    masterClient1.PhotonClient.OpCreateRoom(
                        roomName, true, true, 0, new Hashtable(), new string[0], new Hashtable()));
                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress1 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Console.WriteLine("Match on GS: " + gameServerAddress1);

                // join on master while the first client is not yet on GS:
                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRoom(roomName, new Hashtable()));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.GameIdNotExists,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // move 1st client to GS: 
                masterClient1.PhotonClient.Disconnect();

                string[] split = gameServerAddress1.Split(':');
                gameClient1.Connect(split[0], int.Parse(split[1]), appId);
                gameClient1.WaitForConnect(waitTime);

                Hashtable player1Properties = new Hashtable();
                player1Properties.Add("Name", player1);

                gameClient1.PhotonClient.OpCreateRoom(
                    roomName, true, true, 0, new Hashtable(), new string[0], player1Properties);
                operationResponse = gameClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // get own join event: 
                var ev = gameClient1.WaitForEvent(waitTime);
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
                var playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(player1, playerProperties["Name"]);

                // join 2nd client on master - ok: 
                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRoom(roomName, new Hashtable()));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress2 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Assert.AreEqual(gameServerAddress1, gameServerAddress2);

                // disconnect and move 2nd client to GS: 
                masterClient2.PhotonClient.Disconnect();

                gameClient2.Connect(split[0], int.Parse(split[1]), appId);
                gameClient2.WaitForConnect(waitTime);

                // clean up - just in case: 
                gameClient1.OperationResponseQueue.Clear();
                gameClient2.OperationResponseQueue.Clear();

                gameClient1.EventQueue.Clear();
                gameClient2.EventQueue.Clear();

                // join 2nd client on GS: 
                Hashtable player2Properties = new Hashtable();
                player2Properties.Add("Name", player2);

                gameClient2.PhotonClient.OpJoinRoom(roomName, player2Properties);
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                ev = gameClient1.WaitForEvent(waitTime);
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(player2, playerProperties["Name"]);

                ev = gameClient2.WaitForEvent(waitTime);
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(player2, playerProperties["Name"]);

                // TODO: continue implementation
                // raise event, leave etc.        
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }

        
        [Test]
        public void JoinRandomNoMatchFound()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);

            try
            {
                Assert.IsTrue(
                    masterClient1.PhotonClient.OpJoinRandomRoom(
                        new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom));
                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                   Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(Operations.ErrorCode.NoMatchFound, (Operations.ErrorCode)operationResponse.ReturnCode, operationResponse.DebugMessage);
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
            }
        }

        [Test]
        public void JoinRandomOnGameServer()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);

            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);
            masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient2.WaitForConnect(waitTime);

            var gameClient1 = new TestClient(ConnectionProtocol.Tcp);

            var gameClient2 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                // create
                string roomName = "JoinRandomOnGameServer_" + Guid.NewGuid().ToString().Substring(0, 6);
                Assert.IsTrue(
                    masterClient1.PhotonClient.OpCreateRoom(
                        roomName, true, true, 0, new Hashtable(), new string[0], new Hashtable()));
                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress1 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Console.WriteLine("Match on GS: " + gameServerAddress1);

                // join on master while the first client is not yet on GS:
                Assert.IsTrue(
                    masterClient2.PhotonClient.OpJoinRandomRoom(
                        new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.NoMatchFound,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // move 1st client to GS: 
                masterClient1.PhotonClient.Disconnect();

                string[] split = gameServerAddress1.Split(':');
                gameClient1.Connect(split[0], int.Parse(split[1]), appId);
                gameClient1.WaitForConnect(waitTime);

                Hashtable player1Properties = new Hashtable();
                player1Properties.Add("Name", player1);

                gameClient1.PhotonClient.OpCreateRoom(
                    roomName, true, true, 0, new Hashtable(), new string[0], player1Properties);
                operationResponse = gameClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // get own join event: 
                var ev = gameClient1.WaitForEvent(waitTime);
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
                var playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(player1, playerProperties["Name"]);

                // join 2nd client on master - ok: 
                Assert.IsTrue(
                    masterClient2.PhotonClient.OpJoinRandomRoom(
                        new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress2 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Assert.AreEqual(gameServerAddress1, gameServerAddress2);

                string roomName2 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.GameId];
                Assert.AreEqual(roomName, roomName2);

                // disconnect and move 2nd client to GS: 
                masterClient2.PhotonClient.Disconnect();

                gameClient2.Connect(split[0], int.Parse(split[1]), appId);
                gameClient2.WaitForConnect(waitTime);

                // clean up - just in case: 
                gameClient1.OperationResponseQueue.Clear();
                gameClient2.OperationResponseQueue.Clear();

                gameClient1.EventQueue.Clear();
                gameClient2.EventQueue.Clear();

                // join 2nd client on GS: 
                Hashtable player2Properties = new Hashtable();
                player2Properties.Add("Name", player2);

                Assert.IsTrue(gameClient2.PhotonClient.OpJoinRoom(roomName, player2Properties));
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                ev = gameClient1.WaitForEvent(waitTime);
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(player2, playerProperties["Name"]);

                ev = gameClient2.WaitForEvent(waitTime);
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(player2, playerProperties["Name"]);

                // TODO: continue implementation
                // raise event, leave etc.        
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }

        [Test]
        public void MaxPlayers()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);
            Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
            masterClient1.WaitForOperationResponse(1000); 
            masterClient1.OperationResponseQueue.Clear();

            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);
            masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient2.WaitForConnect(waitTime);
            Assert.IsTrue(masterClient2.PhotonClient.OpJoinLobby());
            masterClient2.WaitForOperationResponse(1000); 
            masterClient2.OperationResponseQueue.Clear();

            var gameClient1 = new TestClient(ConnectionProtocol.Tcp);
            var gameClient2 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                string gameServerIP;
                int gameServerPort;
                string roomName = "MaxPlayers_" + Guid.NewGuid().ToString().Substring(0, 6);
               

                CreateRoomOnGameServer(
                    masterClient1, true, true, 1, roomName, out gameServerIP, out gameServerPort, out gameClient1);

                // join 2nd client on master - full: 
                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRoom(roomName, new Hashtable()));
                var operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.GameFull,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // join random 2nd client on master - full: 
                Assert.IsTrue(
                    masterClient2.PhotonClient.OpJoinRandomRoom(
                        new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                if (operationResponse.ReturnCode == ErrorCode.Ok)
                {
                    Console.WriteLine(operationResponse.Parameters[ParameterCode.RoomName]);
                    Assert.Fail("Joined room " + operationResponse.Parameters[ParameterCode.RoomName]);
                }

                Assert.AreEqual(
                    Operations.ErrorCode.NoMatchFound,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // join directly on GS: 
                gameClient2.Connect(gameServerIP, gameServerPort, appId);
                gameClient2.WaitForConnect(waitTime);

                gameClient2.PhotonClient.OpJoinRoom(roomName, null);
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.GameFull,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                gameClient1.SendOperationRequest(new OperationRequest() { OperationCode = OperationCode.Leave });
                gameClient2.SendOperationRequest(new OperationRequest() { OperationCode = OperationCode.Leave });

            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }

        [Test]
        public void ApplicationStats()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient1.WaitForConnect(waitTime);

            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);
            masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient2.WaitForConnect(waitTime);

            var masterClient3 = new TestClient(ConnectionProtocol.Tcp);
            masterClient3.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
            masterClient3.WaitForConnect(waitTime);

            var gameClient1 = new TestClient(ConnectionProtocol.Tcp);

            var gameClient2 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                string gameServerIP;
                int gameServerPort;
                string roomName = "ApplicationStats_" + Guid.NewGuid().ToString().Substring(0, 6);
               
                
                // app stats
                var appStatsEvent = masterClient3.WaitForEvent(10000); 
                Assert.IsNotNull(appStatsEvent, "AppStatsEvent");
                Assert.AreEqual(EventCode.AppStats, appStatsEvent.Code, "Event Code");
                Assert.AreEqual(3, appStatsEvent.Parameters[ParameterCode.MasterPeerCount], "Peer Count on Master");
                Assert.AreEqual(0, appStatsEvent.Parameters[ParameterCode.PeerCount], "Peer Count on GS");
                Assert.AreEqual(0, appStatsEvent.Parameters[ParameterCode.GameCount], "Game Count");

                masterClient3.EventQueue.Clear();


                CreateRoomOnGameServer(
                 masterClient1, true, true, 10, roomName, out gameServerIP, out gameServerPort, out gameClient1);

                // app stats
                appStatsEvent = masterClient3.WaitForEvent(10000);
                Assert.IsNotNull(appStatsEvent, "AppStatsEvent");
                Assert.AreEqual(EventCode.AppStats, appStatsEvent.Code, "Event Code");
                Assert.AreEqual(2, appStatsEvent.Parameters[ParameterCode.MasterPeerCount], "Peer Count on Master");
                Assert.AreEqual(1, appStatsEvent.Parameters[ParameterCode.PeerCount], "Peer Count on GS");
                Assert.AreEqual(1, appStatsEvent.Parameters[ParameterCode.GameCount], "Game Count");
                masterClient3.EventQueue.Clear();
                

                Assert.IsTrue(
                 masterClient2.PhotonClient.OpJoinRandomRoom(
                     new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom));
                var operationResponse = masterClient2.WaitForOperationResponse(waitTime);

                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress2 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Assert.AreEqual(string.Format("{0}:{1}", gameServerIP, gameServerPort), gameServerAddress2);

                string roomName2 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.GameId];
                Assert.AreEqual(roomName, roomName2);

                // no app stats if nothing changed?!
                //appStatsEvent = masterClient3.WaitForEvent(10000);
                //Assert.IsNotNull(appStatsEvent, "AppStatsEvent");
                //Assert.AreEqual(EventCode.AppStats, appStatsEvent.Code, "Event Code");
                //Assert.AreEqual(2, appStatsEvent.Parameters[ParameterCode.MasterPeerCount], "Peer Count on Master");
                //Assert.AreEqual(1, appStatsEvent.Parameters[ParameterCode.PeerCount], "Peer Count on GS");
                //Assert.AreEqual(1, appStatsEvent.Parameters[ParameterCode.GameCount], "Game Count");

                //masterClient3.EventQueue.Clear();


                // disconnect and move 2nd client to GS: 
                masterClient2.PhotonClient.Disconnect();

                gameClient2.Connect(gameServerIP, gameServerPort, appId);
                gameClient2.WaitForConnect(waitTime);
                
                // join 2nd client on GS: 
                Hashtable player2Properties = new Hashtable();
                player2Properties.Add("Name", player2);

                Assert.IsTrue(gameClient2.PhotonClient.OpJoinRoom(roomName, player2Properties));
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);


                // app stats: 
                appStatsEvent = masterClient3.WaitForEvent(10000);
                Assert.IsNotNull(appStatsEvent, "AppStatsEvent");
                Assert.AreEqual(EventCode.AppStats, appStatsEvent.Code, "Event Code");
                Assert.AreEqual(1, appStatsEvent.Parameters[ParameterCode.MasterPeerCount], "Peer Count on Master");
                Assert.AreEqual(2, appStatsEvent.Parameters[ParameterCode.PeerCount], "Peer Count on GS");
                Assert.AreEqual(1, appStatsEvent.Parameters[ParameterCode.GameCount], "Game Count");

                masterClient3.EventQueue.Clear();

                gameClient2.SendOperationRequest(new OperationRequest() { OperationCode = OperationCode.Leave });
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    OperationCode.Leave, operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);


                gameClient1.SendOperationRequest(new OperationRequest() { OperationCode = OperationCode.Leave });
                operationResponse = gameClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    OperationCode.Leave, operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);



                // app stats: 
                appStatsEvent = masterClient3.WaitForEvent(10000);
                Assert.IsNotNull(appStatsEvent, "AppStatsEvent");
                Assert.AreEqual(EventCode.AppStats, appStatsEvent.Code, "Event Code");
                Assert.AreEqual(1, appStatsEvent.Parameters[ParameterCode.MasterPeerCount], "Peer Count on Master");
                Assert.AreEqual(2, appStatsEvent.Parameters[ParameterCode.PeerCount], "Peer Count on GS");
                Assert.AreEqual(0, appStatsEvent.Parameters[ParameterCode.GameCount], "Game Count");
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }
                if (masterClient3.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient3.Close();
                }


                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }

        [Test]
        public void BroadcastProperties()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);

            TestClient gameClient1 = new TestClient(ConnectionProtocol.Tcp);
            TestClient gameClient2 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient1.WaitForConnect(waitTime);
                
                masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient2.WaitForConnect(waitTime);

                masterClient1.EventQueue.Clear();
                masterClient2.EventQueue.Clear();

                masterClient1.OperationResponseQueue.Clear();
                masterClient2.OperationResponseQueue.Clear();

                // open game
                string roomName = "BroadcastProperties_" + Guid.NewGuid().ToString().Substring(0, 6);

                Hashtable player1Properties = new Hashtable();
                player1Properties.Add("Name", player1);

                Hashtable gameProperties = new Hashtable();
                gameProperties["P1"] = 1;
                gameProperties["P2"] = 2;


                string[] lobbyProperties = new string[] { "L1", "L2", "L3" };


                Assert.IsTrue(
                    masterClient1.PhotonClient.OpCreateRoom(
                        roomName, true, true, 0, gameProperties, lobbyProperties, player1Properties));

                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress1 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Console.WriteLine("Created room " + roomName + " on GS: " + gameServerAddress1);

                // move 1st client to GS: 
                masterClient1.PhotonClient.Disconnect();

                string[] split = gameServerAddress1.Split(':');
                string gameServerIP = split[0];
                int gameServerPort = int.Parse(split[1]);

                gameClient1.Connect(gameServerIP, gameServerPort, appId);
                gameClient1.WaitForConnect(waitTime);


                gameClient1.PhotonClient.OpCreateRoom(
                    roomName, true, true, 0, gameProperties, lobbyProperties, player1Properties);
                operationResponse = gameClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // move 2nd client to GS: 
                masterClient2.PhotonClient.Disconnect();
                
                gameClient2.Connect(gameServerIP, gameServerPort, appId);
                gameClient2.WaitForConnect(waitTime);

                Hashtable player2Properties = new Hashtable();
                player2Properties.Add("Name", player2);

                Assert.IsTrue(gameClient2.PhotonClient.OpJoinRoom(roomName, player2Properties));
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                var room = (Hashtable)operationResponse.Parameters[ParameterCode.GameProperties]; 
                Assert.IsNotNull(room);
                Assert.AreEqual(5, room.Count);

                Assert.IsNotNull(room[GameProperties.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GameProperties.IsVisible], "IsVisisble");
                Assert.IsNotNull(room[GameProperties.PropsListedInLobby], "PropertiesInLobby");
                Assert.IsNotNull(room["P1"], "P1");
                Assert.IsNotNull(room["P2"], "P2");


                Assert.AreEqual(true, room[GameProperties.IsOpen], "IsOpen");
                Assert.AreEqual(true, room[GameProperties.IsVisible], "IsVisisble");
                Assert.AreEqual(3, ((string[])room[GameProperties.PropsListedInLobby]).Length, "PropertiesInLobby");
                Assert.AreEqual("L1", ((string[])room[GameProperties.PropsListedInLobby])[0], "PropertiesInLobby");
                Assert.AreEqual("L2", ((string[])room[GameProperties.PropsListedInLobby])[1], "PropertiesInLobby");
                Assert.AreEqual("L3", ((string[])room[GameProperties.PropsListedInLobby])[2], "PropertiesInLobby");
                Assert.AreEqual(1, room["P1"], "P1");
                Assert.AreEqual(2, room["P2"], "P2");

                // set properties: 
                var setProperties = new OperationRequest
                    { OperationCode = OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
                
                setProperties.Parameters[ParameterCode.Broadcast] = true; 
                setProperties.Parameters[ParameterCode.Properties] = new Hashtable()
                    { { "P3", 3 }, { "P1", null }, { "P2", 20 } };

                gameClient1.SendOperationRequest(setProperties); 
                operationResponse = gameClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(OperationCode.SetProperties, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                
                var ev = gameClient2.WaitForEvent(EventCode.PropertiesChanged, waitTime);
                
                room = (Hashtable)ev.Parameters[ParameterCode.Properties]; 
                Assert.IsNotNull(room);
                Assert.AreEqual(3, room.Count);

                Assert.IsNull(room["P1"], "P1");
                Assert.IsNotNull(room["P2"], "P2");
                Assert.IsNotNull(room["P3"], "P3");

                Assert.AreEqual(null, room["P1"], "P1");
                Assert.AreEqual(20, room["P2"], "P2");
                Assert.AreEqual(3, room["P3"], "P3");

                var getProperties = new OperationRequest { OperationCode = OperationCode.GetProperties, Parameters = new Dictionary<byte, object>()};
                getProperties.Parameters[ParameterCode.Properties] = PropertyType.Game; 
                
                gameClient2.SendOperationRequest(getProperties);
                operationResponse = gameClient2.WaitForOperationResponse(waitTime);
                
                Assert.AreEqual(OperationCode.GetProperties, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);

                room = (Hashtable)operationResponse.Parameters[ParameterCode.GameProperties];
                Assert.IsNotNull(room);
                Assert.AreEqual(6, room.Count);

                Assert.IsNotNull(room[GameProperties.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GameProperties.IsVisible], "IsVisisble");
                Assert.IsNotNull(room[GameProperties.PropsListedInLobby], "PropertiesInLobby");
                Assert.IsNull(room["P1"], "P1");
                Assert.IsNotNull(room["P2"], "P2");
                Assert.IsNotNull(room["P3"], "P3");


                Assert.AreEqual(true, room[GameProperties.IsOpen], "IsOpen");
                Assert.AreEqual(true, room[GameProperties.IsVisible], "IsVisisble");
                Assert.AreEqual(3, ((string[])room[GameProperties.PropsListedInLobby]).Length, "PropertiesInLobby");
                Assert.AreEqual("L1", ((string[])room[GameProperties.PropsListedInLobby])[0], "PropertiesInLobby");
                Assert.AreEqual("L2", ((string[])room[GameProperties.PropsListedInLobby])[1], "PropertiesInLobby");
                Assert.AreEqual("L3", ((string[])room[GameProperties.PropsListedInLobby])[2], "PropertiesInLobby");
                Assert.AreEqual(null, room["P1"], "P1");
                Assert.AreEqual(20, room["P2"], "P2");
                Assert.AreEqual(3, room["P3"], "P3");
              
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }

                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }

        [Test]
        public void SetPropertiesForLobby()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);

            TestClient gameClient1 = new TestClient(ConnectionProtocol.Tcp);
            TestClient gameClient2 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient1.WaitForConnect(waitTime);

                Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
                var ev = masterClient1.WaitForEvent(EventCode.GameList, waitTime);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                var gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                Assert.AreEqual(0, gameList.Count);

                masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient2.WaitForConnect(waitTime);

                Assert.IsTrue(masterClient2.PhotonClient.OpJoinLobby());
                ev = masterClient2.WaitForEvent(EventCode.GameList, waitTime);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                Assert.AreEqual(0, gameList.Count);

                masterClient1.EventQueue.Clear();
                masterClient2.EventQueue.Clear();

                masterClient1.OperationResponseQueue.Clear();
                masterClient2.OperationResponseQueue.Clear();

                // open game
                string roomName = "SetPropertiesForLobby_" + Guid.NewGuid().ToString().Substring(0, 6);

                Hashtable player1Properties = new Hashtable();
                player1Properties.Add("Name", player1);

                Hashtable gameProperties = new Hashtable();
                gameProperties["P1"] = 1;
                gameProperties["P2"] = 2;

                gameProperties["L1"] = 1;
                gameProperties["L2"] = 2;


                string[] lobbyProperties = new string[] { "L1", "L2", "L3" };

                Assert.IsTrue(
                    masterClient1.PhotonClient.OpCreateRoom(
                        roomName, true, true, 0, gameProperties, lobbyProperties, player1Properties));

                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress1 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Console.WriteLine("Created room " + roomName + " on GS: " + gameServerAddress1);

                // move 1st client to GS: 
                masterClient1.PhotonClient.Disconnect();

                string[] split = gameServerAddress1.Split(':');
                string gameServerIP = split[0];
                int gameServerPort = int.Parse(split[1]);

                gameClient1.Connect(gameServerIP, gameServerPort, appId);
                gameClient1.WaitForConnect(waitTime);


                gameClient1.PhotonClient.OpCreateRoom(
                    roomName, true, true, 0, gameProperties, lobbyProperties, player1Properties);
                operationResponse = gameClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                // get own join event: 
                ev = gameClient1.WaitForEvent(waitTime);
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
                
                var actorList = (int[])ev.Parameters[ParameterCode.ActorList];
                Assert.AreEqual(1, actorList.Length); 
                Assert.AreEqual(1, actorList[0]);

                var playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(player1, playerProperties["Name"]);
                
                ev = masterClient2.WaitForEvent(EventCode.GameListUpdate, waitTime);

                var roomList = (Hashtable)ev.Parameters[(byte)ParameterCode.GameList];
                Assert.GreaterOrEqual(roomList.Count, 1);

                var room = (Hashtable)roomList[roomName];
                Assert.IsNotNull(room);
                Assert.AreEqual(5, room.Count);

                Assert.IsNotNull(room[GameProperties.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GameProperties.MaxPlayers], "MaxPlayers");
                Assert.IsNotNull(room[GameProperties.PlayerCount], "PlayerCount");
                Assert.IsNotNull(room["L1"], "L1");
                Assert.IsNotNull(room["L2"], "L2");
                

                Assert.AreEqual(true, room[GameProperties.IsOpen], "IsOpen");
                Assert.AreEqual(0, room[GameProperties.MaxPlayers], "MaxPlayers");
                Assert.AreEqual(1, room[GameProperties.PlayerCount], "PlayerCount");
                Assert.AreEqual(1, room["L1"], "L1");
                Assert.AreEqual(2, room["L2"], "L2");

                Assert.IsTrue(gameClient1.PhotonClient.OpSetCustomPropertiesOfRoom(new Hashtable() { {"L3", 3 }, {"L1", null}, {"L2", 20 }}));
                operationResponse = gameClient1.WaitForOperationResponse(waitTime); 
                Assert.AreEqual(OperationCode.SetProperties, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode);


                ev = masterClient2.WaitForEvent(EventCode.GameListUpdate, waitTime);

                roomList = (Hashtable)ev.Parameters[(byte)ParameterCode.GameList];
                Assert.AreEqual(1, roomList.Count);

                room = (Hashtable)roomList[roomName];
                Assert.IsNotNull(room);
                Assert.AreEqual(5, room.Count);

                Assert.IsNotNull(room[GameProperties.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GameProperties.MaxPlayers], "MaxPlayers");
                Assert.IsNotNull(room[GameProperties.PlayerCount], "PlayerCount");
                Assert.IsNotNull(room["L2"], "L2");
                Assert.IsNotNull(room["L3"], "L3");

                Assert.AreEqual(true, room[GameProperties.IsOpen], "IsOpen");
                Assert.AreEqual(0, room[GameProperties.MaxPlayers], "MaxPlayers");
                Assert.AreEqual(1, room[GameProperties.PlayerCount], "PlayerCount");
                Assert.AreEqual(20, room["L2"], "L2");
                Assert.AreEqual(3, room["L3"], "L3");

                gameClient1.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave });
                gameClient2.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave }); 
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }

                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }

        [Test]
        public void MatchByProperties()
        {

            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);

            TestClient gameClient1 = new TestClient(ConnectionProtocol.Tcp);
            TestClient gameClient2 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient1.WaitForConnect(waitTime);

                masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient2.WaitForConnect(waitTime);

                masterClient1.EventQueue.Clear();
                masterClient2.EventQueue.Clear();

                masterClient1.OperationResponseQueue.Clear();
                masterClient2.OperationResponseQueue.Clear();

                // open game
                string roomName = "BroadcastProperties_" + Guid.NewGuid().ToString().Substring(0, 6);

                Hashtable player1Properties = new Hashtable();
                player1Properties.Add("Name", player1);

                Hashtable gameProperties = new Hashtable();
                gameProperties["P1"] = 1;
                gameProperties["P2"] = 2;
                gameProperties["L1"] = 1;
                gameProperties["L2"] = 2;
                gameProperties["L3"] = 3;


                string[] lobbyProperties = new string[] { "L1", "L2", "L3" };


                Assert.IsTrue(
                    masterClient1.PhotonClient.OpCreateRoom(
                        roomName, true, true, 0, gameProperties, lobbyProperties, player1Properties));

                var operationResponse = masterClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                string gameServerAddress1 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
                Console.WriteLine("Created room " + roomName + " on GS: " + gameServerAddress1);

                // move 1st client to GS: 
                masterClient1.PhotonClient.Disconnect();

                string[] split = gameServerAddress1.Split(':');
                string gameServerIP = split[0];
                int gameServerPort = int.Parse(split[1]);

                gameClient1.Connect(gameServerIP, gameServerPort, appId);
                gameClient1.WaitForConnect(waitTime);


                gameClient1.PhotonClient.OpCreateRoom(
                    roomName, true, true, 0, gameProperties, lobbyProperties, player1Properties);
                operationResponse = gameClient1.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                
                Hashtable player2Properties = new Hashtable();
                player2Properties.Add("Name", player2);

                Hashtable expectedProperties = new Hashtable();
                expectedProperties.Add("N", null);

                masterClient2.OperationResponseQueue.Clear();

                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRandomRoom(expectedProperties, 0, player2Properties, MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);
                
                Assert.AreEqual(
                    Operations.ErrorCode.NoMatchFound,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                masterClient2.OperationResponseQueue.Clear();

                expectedProperties.Clear();
                expectedProperties.Add("L1", 5);
            
                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRandomRoom(expectedProperties, 0, player2Properties, MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);

                Assert.AreEqual(
                    Operations.ErrorCode.NoMatchFound,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                masterClient2.OperationResponseQueue.Clear();

                expectedProperties.Clear();
                expectedProperties.Add("L1", 1);
                expectedProperties.Add("L2", 1);

                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRandomRoom(expectedProperties, 0, player2Properties, MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);

                Assert.AreEqual(
                    Operations.ErrorCode.NoMatchFound,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);


                expectedProperties.Clear();
                expectedProperties.Add("L1", 1);
                expectedProperties.Add("L2", 2);

                Assert.IsTrue(masterClient2.PhotonClient.OpJoinRandomRoom(expectedProperties, 0, player2Properties, MatchmakingMode.FillRoom));
                operationResponse = masterClient2.WaitForOperationResponse(waitTime);
                Assert.AreEqual(
                    Operations.OperationCode.JoinRandomGame, (Operations.OperationCode)operationResponse.OperationCode);

                Assert.AreEqual(
                    Operations.ErrorCode.Ok,
                    (Operations.ErrorCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);

                



                gameClient1.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave });
                gameClient2.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave });
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }

                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }

                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }

                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
            }
        }


        [Test]
        public void MatchmakingTypes()
        {

            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            var masterClient2 = new TestClient(ConnectionProtocol.Tcp);
            var masterClient3 = new TestClient(ConnectionProtocol.Tcp);
            var masterClient4 = new TestClient(ConnectionProtocol.Tcp);
        
            TestClient gameClient1 = new TestClient(ConnectionProtocol.Tcp);
            TestClient gameClient2 = new TestClient(ConnectionProtocol.Tcp);
            TestClient gameClient3 = new TestClient(ConnectionProtocol.Tcp); ; 

            try
            {
                masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient1.WaitForConnect(waitTime);

                masterClient2.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient2.WaitForConnect(waitTime);

                masterClient3.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient3.WaitForConnect(waitTime);

                masterClient4.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient4.WaitForConnect(waitTime);
                
                // open game
                string gameServerIP;
                int gameServerPort; 
                
                string roomName1 = "MatchmakingTypes_1_" + Guid.NewGuid().ToString().Substring(0, 6);
                CreateRoomOnGameServer(masterClient1, true, true, 0, roomName1, out gameServerIP, out gameServerPort, out gameClient1);

                string roomName2 = "MatchmakingTypes_2_" + Guid.NewGuid().ToString().Substring(0, 6);
                CreateRoomOnGameServer(masterClient2, true, true, 0, roomName2, out gameServerIP, out gameServerPort, out gameClient2);


                string roomName3 = "MatchmakingTypes_3_" + Guid.NewGuid().ToString().Substring(0, 6);
                CreateRoomOnGameServer(masterClient3, true, true, 0, roomName3, out gameServerIP, out gameServerPort, out gameClient3);

                // fill room - 3x: 
                masterClient4.PhotonClient.OpJoinRandomRoom(
                    new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom);
                var operationResponse = masterClient4.WaitForOperationResponse(waitTime); 

                Assert.AreEqual(OperationCode.JoinRandomGame, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                Assert.AreEqual(roomName1, operationResponse[ParameterCode.RoomName]);

                masterClient4.PhotonClient.OpJoinRandomRoom(
                new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom);
                operationResponse = masterClient4.WaitForOperationResponse(waitTime);

                Assert.AreEqual(OperationCode.JoinRandomGame, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                Assert.AreEqual(roomName1, operationResponse[ParameterCode.RoomName]);

                masterClient4.PhotonClient.OpJoinRandomRoom(
                new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom);
                operationResponse = masterClient4.WaitForOperationResponse(waitTime);

                Assert.AreEqual(OperationCode.JoinRandomGame, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                Assert.AreEqual(roomName1, operationResponse[ParameterCode.RoomName]);

                // serial matching - 4x: 
                masterClient4.PhotonClient.OpJoinRandomRoom(
                    new Hashtable(), 0, new Hashtable(), MatchmakingMode.SerialMatching);
                operationResponse = masterClient4.WaitForOperationResponse(waitTime);

                Assert.AreEqual(OperationCode.JoinRandomGame, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                Assert.AreEqual(roomName2, operationResponse[ParameterCode.RoomName]);


                masterClient4.PhotonClient.OpJoinRandomRoom(
              new Hashtable(), 0, new Hashtable(), MatchmakingMode.SerialMatching);
                operationResponse = masterClient4.WaitForOperationResponse(waitTime);

                Assert.AreEqual(OperationCode.JoinRandomGame, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                Assert.AreEqual(roomName3, operationResponse[ParameterCode.RoomName]);

                masterClient4.PhotonClient.OpJoinRandomRoom(
            new Hashtable(), 0, new Hashtable(), MatchmakingMode.SerialMatching);
                operationResponse = masterClient4.WaitForOperationResponse(waitTime);

                Assert.AreEqual(OperationCode.JoinRandomGame, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                Assert.AreEqual(roomName1, operationResponse[ParameterCode.RoomName]);

                masterClient4.PhotonClient.OpJoinRandomRoom(
            new Hashtable(), 0, new Hashtable(), MatchmakingMode.SerialMatching);
                operationResponse = masterClient4.WaitForOperationResponse(waitTime);

                Assert.AreEqual(OperationCode.JoinRandomGame, operationResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, operationResponse.ReturnCode, operationResponse.DebugMessage);
                Assert.AreEqual(roomName2, operationResponse[ParameterCode.RoomName]);

                

                gameClient1.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave });
                gameClient2.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave });
                gameClient3.SendOperationRequest(new OperationRequest { OperationCode = OperationCode.Leave });
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }
                if (masterClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient2.Close();
                }
                if (masterClient3.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient3.Close();
                }
                if (masterClient4.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient4.Close();
                }



                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
                if (gameClient2.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient2.Close();
                }
                if (gameClient3.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient3.Close();
                }
            }
        }
        #endregion

        #region Methods

        private static void AssertConnected(TcpClient client)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stopWatch.ElapsedMilliseconds < waitTime)
            {
                if (client.Connected)
                {
                    break;
                }

                Thread.Sleep(10);
            }

            Assert.IsTrue(client.Connected, "Client not connected.");
        }

        private static void CreateRoomOnGameServer(
            TestClient masterClient,
            bool isVisible,
            bool isOpen,
            byte maxPlayers,
            string roomName,
            out string gameServerIP,
            out int gameServerPort,
            out TestClient gameClient)
        {

            gameClient = new TestClient(ConnectionProtocol.Tcp);

            // create
            Assert.IsTrue(
                masterClient.PhotonClient.OpCreateRoom(
                    roomName, isVisible, isOpen, maxPlayers, new Hashtable(), new string[0], new Hashtable()));
            var operationResponse = masterClient.WaitForOperationResponse(waitTime);
            Assert.AreEqual(
                Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
            Assert.AreEqual(
                Operations.ErrorCode.Ok,
                (Operations.ErrorCode)operationResponse.ReturnCode,
                operationResponse.DebugMessage);

            string gameServerAddress1 = (string)operationResponse.Parameters[(byte)Operations.ParameterCode.Address];
            Console.WriteLine("Created room " + roomName + " on GS: " + gameServerAddress1);

            // move 1st client to GS: 
            masterClient.PhotonClient.Disconnect();

            string[] split = gameServerAddress1.Split(':');
            gameServerIP = split[0];
            gameServerPort = int.Parse(split[1]);

            gameClient.Connect(gameServerIP, gameServerPort, appId);
            gameClient.WaitForConnect(waitTime);

            Hashtable player1Properties = new Hashtable();
            player1Properties.Add("Name", player1);

            gameClient.PhotonClient.OpCreateRoom(
                roomName, isVisible, isOpen, maxPlayers, new Hashtable(), new string[0], player1Properties);
            operationResponse = gameClient.WaitForOperationResponse(waitTime);
            Assert.AreEqual(
                Operations.OperationCode.CreateGame, (Operations.OperationCode)operationResponse.OperationCode);
            Assert.AreEqual(
                Operations.ErrorCode.Ok,
                (Operations.ErrorCode)operationResponse.ReturnCode,
                operationResponse.DebugMessage);

            // get own join event: 
            var ev = gameClient.WaitForEvent(waitTime);
            Assert.AreEqual(EventCode.Join, ev.Code);
            Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
            var playerProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
            Assert.AreEqual(player1, playerProperties["Name"]);
        }

        [Test, Explicit]
        public void DebugHangingGame()
        {
            var masterClient1 = new TestClient(ConnectionProtocol.Tcp);
            var gameClient1 = new TestClient(ConnectionProtocol.Tcp);

            try
            {
                masterClient1.Connect(endPointMaster.Address.ToString(), endPointMaster.Port, appId);
                masterClient1.WaitForConnect(waitTime);

                Assert.IsTrue(masterClient1.PhotonClient.OpJoinLobby());
                var ev = masterClient1.WaitForEvent(EventCode.GameList, waitTime);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                var gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                Assert.GreaterOrEqual(gameList.Count, 1);
                
                // Console.WriteLine("GameList event: " + ev.ToStringFull());

                foreach (var roomName in gameList.Keys)
                {
                    var gameInfo = (Hashtable)gameList[roomName];
                    string dbg = string.Format(
                        "IsOpen: {0}, MaxPlayers: {1}, PlayerCount: {2}",
                        gameInfo[GameProperties.IsOpen],
                        gameInfo[GameProperties.MaxPlayers],
                        gameInfo[GameProperties.PlayerCount]);

                    Console.WriteLine(dbg);

                    masterClient1.OperationResponseQueue.Clear();

                    Assert.IsTrue(masterClient1.PhotonClient.OpJoinRoom((string)roomName, new Hashtable()));
                    var response = masterClient1.WaitForOperationResponse(waitTime);
                    Assert.AreEqual(ErrorCode.Ok, response.ReturnCode, response.DebugMessage);
                    Console.WriteLine("Joined hanging game " + roomName + ": " + response.ToStringFull());


                    //Assert.IsNotNull(response.Parameters[ParameterCode.Address], "Address");

                    //var address = (string)response.Parameters[ParameterCode.Address];
                    //string gameServerIp = address.Split(':')[0];
                    //short gameServerPort = short.Parse(address.Split(':')[1]);

                    //gameClient1.Connect(gameServerIp, gameServerPort, appId);
                    //gameClient1.WaitForConnect(waitTime);

                    //Assert.IsTrue(gameClient1.PhotonClient.OpJoinRoom((string)roomName, new Hashtable()));
                    //response = gameClient1.WaitForOperationResponse(waitTime);

                    //Assert.AreEqual(ErrorCode.Ok, response.ReturnCode, response.DebugMessage);

                }
            }
            finally
            {
                if (masterClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    masterClient1.Close();
                }


                if (gameClient1.PhotonClient.PeerState == PeerStateValue.Connected)
                {
                    gameClient1.Close();
                }
            }
        }

        #endregion
    }
}