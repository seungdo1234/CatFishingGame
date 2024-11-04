using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FishingSystem : MonoBehaviour
{

    private float curFishLerp = 0f;
    private float goalFishLerp = 1f;
    private float curTime = 0f;
    private float curRoundTime = 0f;

    [field: Header("# Fishing Component Info")]
    [field: SerializeField] public RectTransform SliderRect { get; private set; }
    [field: SerializeField] public Slider FishingSlider { get; private set; }
    [field: SerializeField] public FishingZoneSystem FishingZoneSystem { get; private set; }
    [field: SerializeField] public RectTransform FishRect { get; private set; }

    [SerializeField] private FishingEffect fishingEffect;

    private Coroutine moveFishingRoundCoroutine;


    [field: Header("# Fishing Fever Info")]
    [SerializeField] private float feverLockDelayTime;
    [SerializeField] private float matchDelayTime;
    private Image fishImage;
    public bool IsFever => FishingManager.Instance.IsFeverTime;

    public bool IsMatchLock = false;

    [field: Header("# Fishing Slider Animation Info")]
    [SerializeField] private float sliderDamping;
    [SerializeField] private float sliderScaleAnimationDuration;
    private Vector3 feverDampingVec;

    [field: Header("# Fishing Slider Animation Info")]
    [SerializeField] private Image shadowFish;


    public void FishingInit(Transform target)
    {
        fishImage = FishRect.GetComponent<Image>();

        FishingManager.Instance.OnFishingRoundEvent += StartFishRound;
        FishingManager.Instance.OnFeverTimeEvent += FishFeverMatchLock;
        FishingManager.Instance.OnFishFadeEvent += FadeFishEvent;
        TouchManager.Instance.OnTouchBeganEvent += Match;

        fishingEffect.FishingEffectInit(target);

        FishingZoneSystem.FishingZoneInit(this);
        FishRect.gameObject.SetActive(false);
    }

    private void StartFishRound(float fishLerp)
    {
        if (!IsFever) {
            if (!FishRect.gameObject.activeSelf) {
                FishRect.gameObject.SetActive(true);
            }

            if (moveFishingRoundCoroutine != null) {
                StopCoroutine(moveFishingRoundCoroutine);
            }

            moveFishingRoundCoroutine = StartCoroutine(MoveFishingRound(fishLerp));
        }
    }

    private IEnumerator MoveFishingRound(float rountTime)
    {
        if (!Mathf.Approximately(rountTime, curRoundTime)) {
            float prevRatio = curTime / curRoundTime;
            curTime = rountTime * prevRatio;
        }
        curRoundTime = rountTime;

        while (!GameManager.Instance.IsGameOver) {
            if (curTime >= rountTime) {
                curTime = 0f;
                goalFishLerp = Mathf.Floor((goalFishLerp + 1) % 2);
                curFishLerp = Mathf.Floor((curFishLerp + 1) % 2);
            }

            curTime = Mathf.Min(curTime + Time.deltaTime, rountTime);
            ;
            FishingSlider.value = Mathf.Lerp(curFishLerp, goalFishLerp, curTime / rountTime);

            yield return null;
        }
    }
    private void FishFeverMatchLock(bool isFever)
    {
        IsMatchLock = !isFever;

        if (IsMatchLock) {
            UnLockMatch(feverLockDelayTime).Forget();
            scaleGaugeSequence.Kill();
        } else {
            SliderPunchEvent(true);
        }
    }
    private async UniTaskVoid UnLockMatch(double delayTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));

        IsMatchLock = false;
    }
    private void Match()
    {
        if (IsMatchLock) return;

        E_FishingResultType fishingResult = FishingZoneSystem.IsMatchedZone(FishRect);
        FishingManager.Instance.FishingSuccess(fishingResult);

        if (fishingResult != E_FishingResultType.Fail) {
            if(!IsFever)
                SliderPunchEvent(false);

            PlayCatchEffectEvent();
        }
    }

    private void PlayCatchEffectEvent()
    {
        Catch catchEffect = PoolManager.Instance.SpawnFromPool(E_PoolObjectType.Catch).ReturnMyComponent<Catch>();
        catchEffect.CatchEffectInit();
        catchEffect.transform.position = FishRect.position;
    }

private Sequence scaleGaugeSequence;
    private void SliderPunchEvent(bool isLoop )
    {
        feverDampingVec = Vector3.one * sliderDamping;
        
        scaleGaugeSequence = DOTween.Sequence();

        float duration = sliderScaleAnimationDuration ;
        scaleGaugeSequence.Append(SliderRect.transform.DOScale(Vector3.one + feverDampingVec, duration)).SetEase(Ease.Linear);
        scaleGaugeSequence.Append(SliderRect.transform.DOScale(Vector3.one, duration)).SetEase(Ease.Linear);

        if (isLoop) {
            scaleGaugeSequence.SetLoops(-1, LoopType.Yoyo);
        }
        
        scaleGaugeSequence.Play();
    }

    private void FadeFishEvent(float duration)
    {
        if (moveFishingRoundCoroutine != null) {
            StopCoroutine(moveFishingRoundCoroutine);
        }

        float onceFadeTime = duration / 2;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(fishImage.DOFade(0, onceFadeTime).SetEase(Ease.OutQuad));
        sequence.AppendCallback(() => { FishingSlider.value = 0.5f; });

        sequence.Append(fishImage.DOFade(1, onceFadeTime).SetEase(Ease.OutQuad));
        sequence.Play();
    }

}
