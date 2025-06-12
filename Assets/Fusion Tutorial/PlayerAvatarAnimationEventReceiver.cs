using UnityEngine;

public class PlayerAvatarAnimationEventReceiver : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] footstepAudioClipArray;
    [SerializeField]
    private AudioClip landingAudioClip;

    // ‘«‰¹‚ðÄ¶‚·‚é
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            var index = Random.Range(0, footstepAudioClipArray.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClipArray[index], transform.position);
        }
    }

    // ’…’n‰¹‚ðÄ¶‚·‚é
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.position);
        }
    }
}