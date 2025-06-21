using UnityEngine;
using Fusion;
//using DG.Tweening;
// This script is used to rotate an item continuously in a 3D space using DOTween.
// It uses the Fusion networking library for potential multiplayer functionality.
// Ensure you have the DOTween and Fusion packages installed in your Unity project.     
public class ItemMotion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // transform.DOLocalRotate(new Vector3(0,360,0),1f,RotateMode.FastBeyond360)
       //     .SetEase(Ease.Linear)
       //     .SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
