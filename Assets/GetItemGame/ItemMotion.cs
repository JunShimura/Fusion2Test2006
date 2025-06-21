using UnityEngine;
using Fusion;
using DG.Tweening;
//

public class ItemMotion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOLocalRotate(new Vector3(0,360,0),1f,RotateMode.FastBeyond360)
			.SetEase(Ease.Linear)
			.SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
