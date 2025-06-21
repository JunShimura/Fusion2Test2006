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
        [Networked] public int player1Score { get; set; }
        [Networked] public int player2Score { get; set; }
        [Networked] private float timer { get; set; } = 20f;
        [SerializeField] private NetworkObject player1;
        [SerializeField] private NetworkObject player2;
        [SerializeField] private GameObject player1ScoreText;
        [SerializeField] private GameObject player2ScoreText;
        [SerializeField] private GameObject startButton;

        private int maxPlayers = 2; // デフォルト値


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

                if (playerIndex == 0)
                {
                    player1 = instance;
                }
                else if (playerIndex == 1)
                {
                    player2 = instance;
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


        void Update()
        {
            if (Object == null)
                return;

            // 最大人数に達したらゲーム開始
            if (!gameStarted && Runner.ActivePlayers.Count() == maxPlayers)
            {
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
            if (player1Score > player2Score)
                resultText.text = "You Win!";
            else if (player1Score < player2Score)
                resultText.text = "You Lose!";
            else
                resultText.text = "Draw!";
        }
    }
}
