using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public enum E_FishingResultType{Good, Perfect, Fail}
public class FishingResultEventHandler : MonoBehaviour
{
    [Header("# Image Data Info")]
    [SerializeField] private Image eventImage;
    [SerializeField] private Sprite[] fishingResultImage;

    [Header("# Image Animation Info")]
    [SerializeField] private float fadeTime;
    [SerializeField] private float damping;

    private void Awake()
    {
        eventImage.color = new Color(1, 1, 1, 0);
    }
    public void FishingTextInit()
    {
        FishingManager.Instance.OnFishingResultEvent += UpdateFishingResultResult;
    }

    private void UpdateFishingResultResult(E_FishingResultType type)
    {
        eventImage.sprite = fishingResultImage[(int)type];
        eventImage.SetNativeSize();

        UIManager.Instance.UIAnimation.PunchAnimation(eventImage.transform, damping);

        UIManager.Instance.UIAnimation.FadeUI.TryImageFadeEvent(eventImage, fadeTime);
    }

}
