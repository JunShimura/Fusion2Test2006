using Fusion;
using UnityEngine;

public class PlayerAvatar : NetworkBehaviour
{
    // プレイヤー名のネットワークプロパティを定義する
    [Networked]
    public NetworkString<_16> NickName { get; set; }

    private NetworkCharacterController characterController;

    public override void Spawned()
    {
        characterController = GetComponent<NetworkCharacterController>();
        var view = GetComponent<PlayerAvatarView>();
        // プレイヤー名をテキストに反映する
        view.SetNickName(NickName.Value);
        // 自身がアバターの権限を持っているなら、カメラの追従対象にする
        if (HasStateAuthority)
        {
            view.MakeCameraTarget();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 移動
        var cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        var inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        characterController.Move(inputDirection);
        characterController.Move(cameraRotation * inputDirection);
        // ジャンプ
        if (Input.GetKey(KeyCode.Space))
        {
            characterController.Jump();
        }
    }
}
