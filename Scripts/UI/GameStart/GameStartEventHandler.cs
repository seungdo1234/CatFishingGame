using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class GameStartEventHandler : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Sprite oneNumberSprite;
    [SerializeField] private Sprite twoNumberSprite;
    [SerializeField] private Sprite threeNumberSprite;
    [SerializeField] private Image numberImage;
    [SerializeField] private float offset;

    private Vector3 initScale;
    private Vector3 targetVec;

    private void Awake()
    {
        // Initialize scales only once
        initScale = numberImage.transform.localScale;
        targetVec = initScale - (Vector3.one * offset);
        if (targetVec.x < 0)
            targetVec = Vector3.zero;
    }

    public void StartCountDown()
    {
        gameObject.SetActive(true);
        StartAnimation();
    }

    private void StartAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        AddScaleAnimation(sequence, threeNumberSprite);
        AddScaleAnimation(sequence, twoNumberSprite);
        AddScaleAnimation(sequence, oneNumberSprite);

        sequence.OnComplete(OnAnimationComplete);
        sequence.Restart();
    }

    private void AddScaleAnimation(Sequence sequence, Sprite sprite)
    {
        sequence.AppendCallback(() => {
            numberImage.sprite = sprite;
            numberImage.transform.localScale = initScale;
        });
        sequence.Append(numberImage.transform.DOScale(targetVec, duration).SetEase(Ease.OutQuad));
    }

    private void OnAnimationComplete()
    {
        GameObject inGameCanvas = UIManager.Instance.CanvasLocator.GetCanvas(E_CanvasName.InGameCanvas);
        inGameCanvas.GetComponent<InGameDataInitialSystem>().GameInit();
        inGameCanvas.gameObject.SetActive(true);

        GameManager.Instance.GameStart();
        gameObject.SetActive(false);
    }
}
