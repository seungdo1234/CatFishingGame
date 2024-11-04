using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using Cysharp.Threading.Tasks;

public class FeverUIEvnetHandler : MonoBehaviour
{
    [Header("# Fever Gauge Component")]
    [SerializeField] private Image gaugeImage;
    [SerializeField] private Image gaugeFrame;
    [SerializeField] private SkeletonGraphic feverEffect;

    [Header("# Fever Gauge Data Info")]
    [SerializeField] private float feverUIDamping;
    [SerializeField] private float feverScaleAnimationDuration;
    private Vector3 feverDampingVec;
    private Sequence scaleGaugeSequence;

    [Header("# Fever Effect Data Info")]
    [SerializeField] private float feverEffectFadeTimeRatio;
    [SerializeField] private float feverEffectFadeDuration;
    [SerializeField] private float feverEffectPunchTime;
    [SerializeField] private float feverEffectPunchDamping;
    private Sequence fadeEffectSequence;
    private Sequence punchEffectSequence;
    private float feverDuration;
    private Vector3 feverEffectDampingVec;

    [Header("# Fever Gauge")]
    [SerializeField] private int curGaugeValue;
    private int maxGaugeValue;
    
    [Header("# Timer Smooth Animation Event Info")]
    [SerializeField] private float smoothLerpTime;
    private CancellationTokenSource smoothAnimationCTS;

    public void FeverGaugeInit()
    {
        FishingManager.Instance.OnFeverTimeEvent += FeverGaugeEvent;
        FishingManager.Instance.OnUpdateFeverGaugeEvent += UpdateFeverGauge;

        ResetFeverGauge();

    }

    private void FeverGaugeEvent(bool isFever)
    {
        if (isFever)
        {
            FeverUIAnimation();
            ActivateFeverEffet();
        }
        else
        {
            ResetFeverGauge();
        }
    }

    private bool UpdateFeverGauge(int value)
    {
        if (curGaugeValue == maxGaugeValue)
            return false;
        
        curGaugeValue = Mathf.Min(curGaugeValue + value, maxGaugeValue);
        
        float cur = gaugeImage.fillAmount;
        float goal = (float)curGaugeValue / maxGaugeValue;
        
        smoothAnimationCTS?.Cancel();
        smoothAnimationCTS?.Dispose();
        
        smoothAnimationCTS = new CancellationTokenSource();
        SmoothGaugeAnimation(cur, goal, smoothAnimationCTS.Token).Forget();

        if (curGaugeValue == maxGaugeValue) {
            FishingManager.Instance.CallFishFadeEvent(smoothLerpTime);
            return false;
        } else {
            return true;
        }

    }
    
    private async UniTaskVoid SmoothGaugeAnimation(float cur, float goal, CancellationToken cancellationToken)
    {
        float currentTime = 0f;
        while (currentTime < smoothLerpTime) {
            if (cancellationToken.IsCancellationRequested)
                break;

            currentTime += Time.deltaTime;
            float t = currentTime / smoothLerpTime;
            t = Mathf.Min(t, 1);
            float smoothValue = Mathf.Lerp(cur, goal, t);
            gaugeImage.fillAmount= smoothValue;
        
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        if (gaugeImage.fillAmount >= 1f)
        {
            FishingManager.Instance.CallFeverTimeEvent(true);
        }
    }

    private void ResetFeverGauge()
    {
        maxGaugeValue = FishingManager.Instance.FeverMaxGauge;
        feverDuration = FishingManager.Instance.FeverDuration;
        curGaugeValue = 0;
        gaugeImage.fillAmount = 0;
        feverDampingVec = Vector3.one * feverUIDamping;
        feverEffectDampingVec = Vector3.one * feverEffectPunchDamping;
        feverEffect.gameObject.SetActive(false);
    }

    private void FeverUIAnimation()
    {

        Sequence sequence = DOTween.Sequence();

        sequence.Append(gaugeImage.DOFillAmount(0f, feverDuration).SetEase(Ease.Linear));
        sequence.InsertCallback(feverDuration * feverEffectFadeTimeRatio, FadeFeverEffect);
        sequence.OnComplete(EndFeverTime);
        sequence.Play();

        scaleGaugeSequence = DOTween.Sequence();

        scaleGaugeSequence.Append(gaugeFrame.transform.DOScale(Vector3.one + feverDampingVec, feverScaleAnimationDuration)).SetEase(Ease.Linear);
        scaleGaugeSequence.Append(gaugeFrame.transform.DOScale(Vector3.one, feverScaleAnimationDuration)).SetEase(Ease.Linear);

        scaleGaugeSequence.Play().SetLoops(-1, LoopType.Yoyo);

    }

    private void FadeFeverEffect()
    { 
        fadeEffectSequence = DOTween.Sequence();

        fadeEffectSequence.Append(feverEffect.DOFade(0.3f, feverEffectFadeDuration)).SetEase(Ease.Linear);
        fadeEffectSequence.Append(feverEffect.DOFade(1f, feverEffectFadeDuration)).SetEase(Ease.Linear);

        fadeEffectSequence.SetLoops(-1, LoopType.Yoyo);
        fadeEffectSequence.Play();
    }

    private void ActivateFeverEffet()
    {
        feverEffect.gameObject.SetActive(true);
        feverEffect.AnimationState.SetAnimation(0, "FeverTime_Symbol", false);
        PunchFeverEffect();
    }

    private async UniTaskVoid PunchFeverEffect()
    {
        await UniTask.Delay(500);

        Vector3 initScale = feverEffect.transform.localScale;

        punchEffectSequence = DOTween.Sequence();

        punchEffectSequence.Append(feverEffect.transform.DOScale(initScale + feverEffectDampingVec, feverEffectPunchTime));
        punchEffectSequence.Append(feverEffect.transform.DOScale(initScale, feverEffectPunchTime /2));
        punchEffectSequence.SetLoops(-1, LoopType.Restart);
    }


    private void EndFeverTime()
    {
        scaleGaugeSequence.Kill();
        punchEffectSequence.Kill();
        punchEffectSequence.Kill();
        fadeEffectSequence.Kill();
        FishingManager.Instance.CallFeverTimeEvent(false);
        feverEffect.gameObject.SetActive(false);
    }
}
