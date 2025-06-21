using Fusion;
using UnityEngine;
using TMPro;
using System.Linq;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Collections;
namespace GetItemGame
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private GameLauncher gameLauncher;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private GameObject gameUI;

        [Networked] private bool gameStarted { get; set; }
        [Networked] public int[] playerScore { get; set; }
        [Networked] private float timer { get; set; } = 20f;
        [SerializeField] private NetworkObject[] players;
        [SerializeField] private TextMeshProUGUI[] playerScoreTexts;
        [SerializeField] private GameObject startButton;

        private const int MAX_PLAYERS = 2; // デフォルト値


        private void Awake()
        {
            if (gameLauncher == null)
            {
                Debug.LogError("GameLauncher is not assigned.");
            }
            gameLauncher.OnPlayerJoinedAction += OnPlayerJoined;
        }

        private void OnPlayerJoined(NetworkRunner runner, PlayerRef player, int playerIndex, NetworkObject instance)
        {

            players[playerIndex] = instance;
            instance.GetComponent<Player>().playerId = playerIndex; // プレイヤーIDを設定
            playerScore = new int[MAX_PLAYERS]; // スコア配列を初期化
            instance.GetComponent<Player>().OnItemCollected += OnGetItem; // アイテム取得イベントを登録
            Debug.Log($"Player {playerIndex} has joined the game. Player count: {runner.SessionInfo.PlayerCount}");
        }
        private void OnGetItem(Player player)
        {
            // アイテムを取得したときの処理
            Debug.Log("Item collected by player: " + gameObject.name);
            // アイテム取得イベントを発火
            if (players[player.playerId] != null)
            {
                playerScore[player.playerId]++;
                playerScoreTexts[player.playerId].GetComponent<TextMeshProUGUI>().text = "Score: " + playerScore[player.playerId];
            }
        }

        private void OnDestroy()
        {
            if (gameLauncher != null)
            {
                gameLauncher.OnPlayerJoinedAction -= OnPlayerJoined;
            }
        }

        private void Start()
        {
            // SessionPropertiesから最大人数を取得（存在すれば）
            //if (Runner.SessionInfo != null && Runner.SessionInfo.Properties != null && Runner.SessionInfo.Properties.ContainsKey("MaxPlayers"))
            //{
            //    maxPlayers = (int)Runner.SessionInfo.Properties["MaxPlayers"];
            //}
        }


        public override void FixedUpdateNetwork()
        {
            if (Object == null)
                return;

            // 最大人数に達したらゲーム開始
            if (!gameStarted && Runner.SessionInfo.PlayerCount == MAX_PLAYERS)
            {
                Debug.Log("All players have joined. Starting game...");
                StartGame();
            }

            if (Object.HasStateAuthority && gameStarted)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    EndGame();
                }
            }
        }

        void StartGame()
        {
            gameStarted = true;
            timer = 20f;
        }

        void EndGame()
        {
            if (playerScore[0] > playerScore[1])
                resultText.text = "You Win!";
            else if (playerScore[0] < playerScore[1])
                resultText.text = "You Lose!";
            else
                resultText.text = "Draw!";
        }
    }
}
