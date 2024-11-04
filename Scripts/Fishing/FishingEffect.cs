using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FishingEffect
{
    [Header("# Fishing Effect Info")]
    [SerializeField] private Transform fishingPoint;
    [SerializeField] private float lerpTime;
    [SerializeField] private float randomCurveOffset;

    private Transform targetPos;
    private Vector2 startPos;
    public void FishingEffectInit(Transform target)
    {
        FishingManager.Instance.OnFishingResultEvent += FishingEffectEvent;

        targetPos = target;
        this.startPos = UIManager.Instance.MainCamera.WorldToScreenPoint(fishingPoint.position);
    }

    private void FishingEffectEvent(E_FishingResultType type)
    {
        if (type == E_FishingResultType.Fail)
            return;

        // 퍼지고 이동
        int count = FishingManager.Instance.GetFishCount(type);
        StartFishingEffect(count).Forget();
    }

    private async UniTaskVoid StartFishingEffect(int count)
    {
        AnimationCurve curve = new AnimationCurve();
        // 가운데 랜덤 값을 구해서 애니메이션 커브에 적용
        float randomValue = UnityEngine.Random.Range(-randomCurveOffset, randomCurveOffset);
        Keyframe[] keys = new Keyframe[3];
        keys[0] = new Keyframe(0f, startPos.x);
        keys[1] = new Keyframe(0.5f, targetPos.position.x + randomValue);
        keys[2] = new Keyframe(1f, targetPos.position.x);
        curve.keys = keys;

        for (int i = 0; i < count; i++)
        {
            Fish fish = PoolManager.Instance.SpawnFromPool(E_PoolObjectType.Fish).ReturnMyComponent<Fish>();
            bool isPunch = i  == count - 1;
            fish.FishMoveInit(startPos,targetPos.position, curve, lerpTime, isPunch);
            await UniTask.Delay(100);
        }
    }
}
