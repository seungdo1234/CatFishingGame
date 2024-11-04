using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class FishingManager : Singleton<FishingManager>
{
    public event Action<float> OnFishingRoundEvent;
    public event Action OnFishingSettingEvent;
    public event Action<E_FishingResultType> OnFishingResultEvent;
    public event Action<int, bool> OnFishingScoreEvent;

    [field: Header("# Fishing Info")]
    [field: SerializeField] public int BaseFishCount { get; private set; }
    [field: SerializeField] public int PerfectFishCount { get; private set; }
    [field: SerializeField] public int Score { get; private set; }
    [field: SerializeField] public int FishingCount { get; private set; }
    [field: SerializeField] public float MaxTimer { get; private set; }

    [field: Header("# Fishing Fever Time Info")]
    [field: SerializeField] public float FeverDuration { get; private set; }
    [field: SerializeField] public int FeverMaxGauge { get; private set; }
    [field: SerializeField] public int IncreaseFeverValue { get; private set; }
    public bool IsFeverTime { get; private set; }

    public event Action<float> OnFishFadeEvent;
    public event Action<bool> OnFeverTimeEvent;
    public event Func <int , bool> OnUpdateFeverGaugeEvent;


    [field: Header("# Fish Speed Randomizer Info")]
    [SerializeField] private FishSpeedRandomizer Randomizer;

    [field: Header("# Combo System Info")]
    [field: SerializeField] public int MaxComboCount { get; private set; }

    public int GetReward => Score * MaxComboCount;
    
    protected override void Awake()
    {
        base.Awake();

        Randomizer = new FishSpeedRandomizer();
        Randomizer.Init();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStartEvent += FishingStart;
    }

    private void FishingStart()
    {
        StartNewFishing();
    }

    private void StartNewFishing()
    {
        // 랜덤 존 설정
        OnFishingSettingEvent?.Invoke();

        // 물고기 라운드 시간 설정
        float rand = Randomizer.GetRandomFishSpeed();

        // 이벤트 시작
        CallFishingRoundEvent(rand);
    }

    public void CallFishingRoundEvent(float rand)
    {
        OnFishingRoundEvent?.Invoke(rand);
    }

    public void FishingSuccess(E_FishingResultType type)
    {
        if (type != E_FishingResultType.Fail) {
            UpdateScore(type);
            UpdateFishingText(type);

            if (OnUpdateFeverGaugeEvent != null && OnUpdateFeverGaugeEvent.Invoke(IncreaseFeverValue)) {
                StartNewFishing();
            }
        } else {
            UpdateFishingText(type);
        }
    }

    private void UpdateFishingText(E_FishingResultType type)
    {
        OnFishingResultEvent?.Invoke(type);
    }

    public void UpdateFishingCount(bool isPunch)
    {
        OnFishingScoreEvent?.Invoke(++FishingCount, isPunch);
    }
    
    public void CallFishFadeEvent(float duration)
    {
        OnFishFadeEvent?.Invoke(duration);
    }
    
    public void CallFeverTimeEvent(bool isFever)
    {
        IsFeverTime = isFever;
        OnFeverTimeEvent?.Invoke(isFever);
        StartNewFishing() ;
    }
    
    private void UpdateScore(E_FishingResultType type)
    {
        Score += GetScoreCount(type);
    }

    private int GetScoreCount(E_FishingResultType type)
    {
        return type switch
        {
            E_FishingResultType.Good => 1,
            E_FishingResultType.Perfect => 5,
            _ => 0
        };
    }

    public int GetFishCount(E_FishingResultType type)
    {
        return type == E_FishingResultType.Good ? BaseFishCount : PerfectFishCount;
    }

    public bool IsFishingClear()
    {
        return Score == FishingCount;
    }
    
    public void ComboUpdate(int curComboCount)
    {
        if (MaxComboCount < curComboCount)
            MaxComboCount = curComboCount;
    }

}
