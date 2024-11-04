using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerEventHanlder : MonoBehaviour
{
    [Header("# Timer Warnning Event Info")]
    [SerializeField] private float warnningRatio;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private Color warnningTargetColor;
    [SerializeField] private float warnningColorLerpTime;
    [SerializeField] private float silderDamping;
    [SerializeField] private float warnningScaleLerpTime;
    private bool isWarnning;

    [Header("# Timer Interact Event Info")]
    [SerializeField] private float interactImageFadeTime;
    [SerializeField] private float interactImageDamping;
    [SerializeField] private Image interactImage;
    [SerializeField] private Sprite[] interactSprites;

    private Slider timerSlider;
    private TextMeshProUGUI timerText;
    private float curTimer;

    // 타이머 가중치
    [Header("# Timer Weight Info")]
    [SerializeField] private float timerWeightIncreaseTime;
    [SerializeField] private float increaseWeight;
    private float weightTimer;
    private float weight = 1f;

    [Header("# Timer Smooth Animation Event Info")]
    [SerializeField] private float smoothLerpTime;
    private bool isSmooth;
    private CancellationTokenSource smoothAnimationCTS;
    
    private void Awake()
    {
        interactImage.color = new Color(1, 1, 1, 0);
    }

    public void TimerInit()
    {
        timerSlider = GetComponentInChildren<Slider>();
        timerText = GetComponentInChildren<TextMeshProUGUI>();

        FishingManager.Instance.OnFishingResultEvent += UpdateTimer;

        GameManager.Instance.OnGameStartEvent += StartTimerEvent;
    }

    public void StartTimerEvent()
    {
        TimerEventCoroutine().Forget();
    }

    private async UniTaskVoid TimerEventCoroutine()
    {
        float maxTimer = curTimer = FishingManager.Instance.MaxTimer;
        float warnningTime = maxTimer * warnningRatio;

        while (curTimer > 0) {
            if (!isSmooth) {
                curTimer -= Time.deltaTime * weight;
                weightTimer += Time.deltaTime;

                RunTimer(warnningTime, maxTimer);
                IncreaseTimerWeight();
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        timerSlider.value = 0f;
        timerText.text = "0";

        UIManager.Instance.UIAnimation.KillColorAnimation();
        GameManager.Instance.GameOver();
    }

    private void RunTimer(float warnningTime, float maxTimer)
    {
        if (!isWarnning && curTimer <= warnningTime) {
            isWarnning = true;
            UIManager.Instance.UIAnimation.ColorAnimation(sliderFillImage, warnningTargetColor, warnningColorLerpTime);
            UIManager.Instance.UIAnimation.ScaleAnimation(timerSlider.transform, silderDamping, warnningScaleLerpTime);
        }

        if (isWarnning && curTimer > warnningTime) {
            isWarnning = false;
            UIManager.Instance.UIAnimation.KillColorAnimation();
            sliderFillImage.color = Color.white;

        }

        timerSlider.value = curTimer / maxTimer;
        timerText.text = $"{curTimer:F2}";
    }

    private void IncreaseTimerWeight()
    {
        if (weightTimer >= timerWeightIncreaseTime) {
            weightTimer = 0f;
            weight += increaseWeight;
        }
    }
    
    private async UniTaskVoid SmoothGaugeAnimation(float cur, float goal, CancellationToken cancellationToken)
    {
        float currentTime = 0f;
        isSmooth = true;
        while (currentTime < smoothLerpTime) {
            if (cancellationToken.IsCancellationRequested)
                break;

            currentTime += Time.deltaTime;
            float t = currentTime / smoothLerpTime;
            t = Mathf.Min(t, 1);
            float smoothValue = Mathf.Lerp(cur, goal, t);
            timerSlider.value = smoothValue;
        
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        isSmooth = false;
    }

    private void UpdateTimer(E_FishingResultType type)
    {
        curTimer = Mathf.Min(curTimer + GetUpdateTimerValue(type), FishingManager.Instance.MaxTimer);
        float cur = timerSlider.value;
        float goal = curTimer / FishingManager.Instance.MaxTimer;
        
        smoothAnimationCTS?.Cancel();
        smoothAnimationCTS?.Dispose();
        
        smoothAnimationCTS = new CancellationTokenSource();
        SmoothGaugeAnimation(cur, goal, smoothAnimationCTS.Token).Forget();

        UpdateInteractTimer(type);
    }

    private void UpdateInteractTimer(E_FishingResultType type)
    {
        int num = Mathf.Max(0, (int)type - 1);
        interactImage.sprite = interactSprites[num];
        interactImage.SetNativeSize();

        UIManager.Instance.UIAnimation.PunchAnimation(interactImage.transform, interactImageDamping);
        UIManager.Instance.UIAnimation.FadeUI.TryImageFadeEvent(interactImage, interactImageFadeTime);
    }

    private float GetUpdateTimerValue(E_FishingResultType type)
    {
        return type switch
        {
            E_FishingResultType.Good => 3f,
            E_FishingResultType.Perfect => 3f,
            E_FishingResultType.Fail => -3f,
            _ => 0f
        };
    }
}
