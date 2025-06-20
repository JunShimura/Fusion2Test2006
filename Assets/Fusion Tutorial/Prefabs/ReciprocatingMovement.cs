using Fusion;
using UnityEngine;

public class ReciprocatingMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private Vector3 _startPositionOffset = new Vector3(0, 0, 0); // �J�n�ʒu�̃I�t�Z�b�g (�����ʒu����̑���)
    [SerializeField]
    private Vector3 _endPositionOffset = new Vector3(0, 0, 5);   // �I���ʒu�̃I�t�Z�b�g (�����ʒu����̑���)
    [SerializeField]
    private float _movementDuration = 3.0f; // �Г��ɂ����鎞�� (�b)

    // �l�b�g���[�N���������ʒu
    [Networked]
    private Vector3 NetworkedPosition { get; set; }

    // �����^���̌��݂̐i�� (0����1�Ő��K��)
    [Networked]
    private float CurrentProgress { get; set; }

    // �ړ����� (1�͏������A-1�͋t����)
    [Networked]
    private int Direction { get; set; } = 1; // �����͏�����

    private Vector3 _initialWorldPosition; // �I�u�W�F�N�g���X�|�[�����ꂽ�ۂ̃��[���h���W

    public override void Spawned()
    {
        // �X�|�[�����̃��[���h���W���L�^
        _initialWorldPosition = transform.position;

        // �����ʒu��ݒ� (�X�|�[�����̈ʒu + �I�t�Z�b�g)
        NetworkedPosition = _initialWorldPosition + _startPositionOffset;

        // �N���C�A���g����NetworkedPosition��K�p
        transform.position = NetworkedPosition;
    }

    public override void FixedUpdateNetwork()
    {
        // State Authority �݂̂��ړ����W�b�N�����s
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

        // �S�N���C�A���g�ňʒu��K�p
        transform.position = NetworkedPosition;

        Debug.Log($"{Runner.LocalPlayer}: {NetworkedPosition}");
    }
}