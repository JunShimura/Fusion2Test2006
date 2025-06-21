using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

using Random = UnityEngine.Random;

namespace GetItemGame
{
    // ゲームの開始、プレイヤーの参加、アバターのスポーンなどを管理するクラス
    // Fusionのコールバックを実装して、ネットワークイベントに対応する
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkRunner))]

    public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]
        private NetworkRunner networkRunnerPrefab;
        [SerializeField]
        private NetworkPrefabRef playerAvatarPrefab;

        [SerializeField]
        private NetworkPrefabRef networkObjectPrefab;
        public Action<NetworkRunner, PlayerRef,int,NetworkObject> OnPlayerJoinedAction;

        private NetworkRunner networkRunner;

        [SerializeField] private Vector3[] spawnPosition
        = { new Vector3(0, 2, 0), new Vector3(5, 2, 0), new Vector3(-5, 2, 0) };
        [SerializeField] private Quaternion spawnRotation = Quaternion.identity;


        // ゲーム開始時にNetworkRunnerをインスタンス化し、コールバックを登録する
        // そして、ゲームを開始する 

        private async void Start()
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            // GameLauncherを、NetworkRunnerのコールバック対象に追加する
            networkRunner.AddCallbacks(this);
            var result = await networkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared
            });
        }

        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            int playerIndex = -1;
            NetworkObject instance = null;

            // State Authorityのみがスポーン処理を行う
            if (runner.IsSharedModeMasterClient)
            {
                playerIndex = runner.ActivePlayers.ToList().IndexOf(player);
                var spawnPos = spawnPosition[playerIndex % spawnPosition.Length];
                Debug.Log($"プレイヤー{playerIndex}のスポーン位置: {spawnPos}");

                instance = runner.Spawn(playerAvatarPrefab, spawnPos, spawnRotation, inputAuthority: player, onBeforeSpawned: (_, networkObject) =>
                {
                    networkObject.GetComponent<PlayerAvatar>().NickName = $"Player{Random.Range(0, 10000)}";
                });
            }

            // 全クライアントでコールバック
            OnPlayerJoinedAction?.Invoke(runner, player, playerIndex, instance);
        }
        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    }
} // namespace GetItemGame