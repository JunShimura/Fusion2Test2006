using Fusion;
using UnityEngine;

public class PlayerAvatar : NetworkBehaviour
{
    // �v���C���[���̃l�b�g���[�N�v���p�e�B���`����
    [Networked]
    public NetworkString<_16> NickName { get; set; }

    private NetworkCharacterController characterController;

    public override void Spawned()
    {
        characterController = GetComponent<NetworkCharacterController>();
        var view = GetComponent<PlayerAvatarView>();
        // �v���C���[�����e�L�X�g�ɔ��f����
        view.SetNickName(NickName.Value);
        // ���g���A�o�^�[�̌����������Ă���Ȃ�A�J�����̒Ǐ]�Ώۂɂ���
        if (HasStateAuthority)
        {
            view.MakeCameraTarget();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // �ړ�
        var cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        var inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        characterController.Move(inputDirection);
        characterController.Move(cameraRotation * inputDirection);
        // �W�����v
        if (Input.GetKey(KeyCode.Space))
        {
            characterController.Jump();
        }
    }
}
