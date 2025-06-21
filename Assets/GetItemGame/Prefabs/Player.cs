using System;
using UnityEngine;
namespace GetItemGame
{
    // プレイヤーのアバターを管理するクラス
    // プレイヤーがアイテムを取得したときのイベントを定義する
    // プレイヤーIDを追加して、複数のプレイヤーを区別できるようにする

    public class Player : MonoBehaviour
    {

        public Action<Player> OnItemCollected; // アイテム取得時のイベント
        public int playerId; // プレイヤーIDを追加

        public void GetItem()
        {
            // プレイヤーがアイテムを取得したときの処理をここに記述
            Debug.Log("Item collected by player: " + gameObject.name);
            OnItemCollected?.Invoke(this);
        }
    }
}
