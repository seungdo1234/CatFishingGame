using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KikiAnimation : MonoBehaviour
{
    [SerializeField] private float duration;

    private void Start()
    {
        UIManager.Instance.CanvasLocator.GetCanvas(E_CanvasName.GameOverCanvas).GetComponent<GameOverPanel>().AddMenuBtnEvent(KillAnimation);
    }

    private void OnEnable()
    {
        StartAnimation();
    }

    private void StartAnimation()
    {
        transform.DORotate(new Vector3(0, 0, 5), duration)
            .OnComplete(LoopAnimation)
            .SetId(0);
    }

    private void LoopAnimation()
    {
        Sequence rotationSequence = DOTween.Sequence();

        rotationSequence.Append(transform.DORotate(new Vector3(0, 0, -2.5f), duration)
            .SetEase(Ease.Linear));

        rotationSequence.Append(transform.DORotate(new Vector3(0, 0, 2.5f), duration)
            .SetEase(Ease.Linear));

        rotationSequence.SetLoops(-1, LoopType.Yoyo).SetId(0);
    }

    private void KillAnimation()
    {
        DOTween.Kill(0);
    }


}
