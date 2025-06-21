using Fusion;
using UnityEngine;

public class ReciprocatingMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private Vector3 _startPositionOffset = new Vector3(0, 0, 0); // 開始位置のオフセット (初期位置からの相対)
    [SerializeField]
    private Vector3 _endPositionOffset = new Vector3(0, 0, 5);   // 終了位置のオフセット (初期位置からの相対)
    [SerializeField]
    private float _movementDuration = 3.0f; // 片道にかかる時間 (秒)

    // ネットワーク同期される位置
    [Networked]
    private Vector3 NetworkedPosition { get; set; }
    [Networked]
    private Vector3 NetworkedRotation { get; set; }

    // 往復運動の現在の進捗 (0から1で正規化)
    [Networked]
    private float CurrentProgress { get; set; }

    // 移動方向 (1は順方向、-1は逆方向)
    [Networked]
    private int Direction { get; set; } = 1; // 初期は順方向

    private Vector3 _initialWorldPosition; // オブジェクトがスポーンされた際のワールド座標

    public override void Spawned()
    {
        // スポーン時のワールド座標を記録
        _initialWorldPosition = transform.position;

        // 初期位置を設定 (スポーン時の位置 + オフセット)
        NetworkedPosition = _initialWorldPosition + _startPositionOffset;

        // クライアント側でNetworkedPositionを適用
        transform.position = NetworkedPosition;
    }

        // ローカル、リモートの両方で呼ばれる。
    // UnityのUpdateのようなイメージ。
    public override void Render()
    {
        if (!HasStateAuthority)
        {
            // State Authority がない場合、ネットワーク同期された位置を適用
            transform.position = NetworkedPosition;
            transform.rotation = Quaternion.identity; // 回転は固定
        }
 
    }

    public override void FixedUpdateNetwork()
    {
        // State Authority のみが移動ロジックを実行
        if (HasStateAuthority)
        {
            CurrentProgress += Runner.DeltaTime / _movementDuration * Direction;

            if (Direction == 1 && CurrentProgress >= 1.0f)
            {
                CurrentProgress = 1.0f;
                Direction = -1;
            }
            else if (Direction == -1 && CurrentProgress <= 0.0f)
            {
                CurrentProgress = 0.0f;
                Direction = 1;
            }

            Vector3 targetPosition = Vector3.Lerp(_initialWorldPosition + _startPositionOffset, _initialWorldPosition + _endPositionOffset, CurrentProgress);
            NetworkedPosition = targetPosition;
        }

        NetworkedRotation = Quaternion.identity.eulerAngles; // 回転は固定
    
        // 全クライアントで位置を適用
        transform.position = NetworkedPosition;
        transform.rotation = Quaternion.Euler(NetworkedRotation);
        Debug.Log($"{Runner.LocalPlayer}: {NetworkedPosition}");
    }
}