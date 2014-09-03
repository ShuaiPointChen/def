
namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System;
    using System.Collections;

    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;

    public interface IGameList
    {
        int Count { get; }

        void AddGameState(GameState gameState);

        int CheckJoinTimeOuts(TimeSpan timeOut);

        int CheckJoinTimeOuts(DateTime minDateTime);

        bool ContainsGameId(string gameId);

        IGameListSubscibtion AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount);

        void RemoveGameServer(IncomingGameServerPeer gameServer);

        bool RemoveGameState(string gameId);

        bool TryGetGame(string gameId, out GameState gameState);

        bool TryGetRandomGame(JoinRandomType joinType, ILobbyPeer peer, Hashtable gameProperties, out GameState gameState);

        bool UpdateGameState(UpdateGameEvent updateOperation, out GameState gameState);

        void PublishGameChanges();
    }
}
