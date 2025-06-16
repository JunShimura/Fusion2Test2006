using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

using Random = UnityEngine.Random;

namespace GetItem.Game
{
    // ゲームの開始時に、NetworkRunnerを生成してゲームを開始するクラス
    // このクラスは、シーンの最初に実行される必要があるため、GameObjectにアタッチしておく
    // また、NetworkRunnerのコールバックを受け取るために、INetworkRunnerCallbacksインターフェースを実装する

    public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]
        private NetworkRunner networkRunnerPrefab;
        [SerializeField]
        private NetworkPrefabRef playerAvatarPrefab;

        [SerializeField]
        private Vector3[] spawnPositions = new Vector3[2]; // 2つのスポーン位置

        private NetworkRunner _networkRunner;
        private bool _sessionJoinTried = false;

        private async void Start()
        {
            _networkRunner = Instantiate(networkRunnerPrefab);
            _networkRunner.AddCallbacks(this);

            // セッションリスト取得
            await _networkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = "" // 空文字でロビーリスト取得
            });
        }

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            if (_sessionJoinTried) return;
            _sessionJoinTried = true;

            if (sessionList != null && sessionList.Count > 0)
            {
                // 既存セッションに参加
                var session = sessionList[0];
                runner.StartGame(new StartGameArgs
                {
                    GameMode = GameMode.Shared,
                    SessionName = session.Name
                });
            }
            else
            {
                // 新規セッション作成（2人制限）
                runner.StartGame(new StartGameArgs
                {
                    GameMode = GameMode.Shared,
                    SessionName = $"Room_{Random.Range(0, 10000)}",
                    PlayerCount = 2
                });
            }
        }

        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            // セッションへ参加したプレイヤーが自分自身かどうかを判定する
            if (player == runner.LocalPlayer)
            {
                // 参加順（0番目 or 1番目）を決定
                int index = 0;
                var players = new List<PlayerRef>(runner.ActivePlayers);
                players.Sort((a, b) => a.RawEncoded.CompareTo(b.RawEncoded));
                index = players.IndexOf(player);

                // スポーン位置を決定
                Vector3 spawnPosition = (index >= 0 && index < spawnPositions.Length)
                    ? spawnPositions[index]
                    : Vector3.zero;

                runner.Spawn(playerAvatarPrefab, spawnPosition, Quaternion.identity, onBeforeSpawned: (_, networkObject) =>
                {
                    networkObject.GetComponent<PlayerAvatar>().NickName = $"Player{Random.Range(0, 10000)}";
                });
            }
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
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    }
}