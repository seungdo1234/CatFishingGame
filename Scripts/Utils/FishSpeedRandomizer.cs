
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishSpeedRandomizer 
{
    [Header("# Randomizer Base Info")]
    public float baseLerpTime = 1.5f;
    public float decreaseLerpTime = 0.05f;
    public float lerpTimeRandomPitch = 0.2f;
    public float minLerpTime = 0.5f;

    [Header("# Randomizer Detail Info")]
    public float curLerpTime = 0f;
    public int failCount = 0;

    public void Init()
    {
        curLerpTime = baseLerpTime;

        FishingManager.Instance.OnFishingResultEvent += FishingResultEvent;
    }

    public float GetRandomFishSpeed()
    {
        float speed = curLerpTime + (failCount * decreaseLerpTime);

        speed += Random.Range(-lerpTimeRandomPitch, lerpTimeRandomPitch);

        return speed;
    }

    private void FishingResultEvent(E_FishingResultType type)
    {
        if (FishingManager.Instance.IsFeverTime)
            return;

        if(type == E_FishingResultType.Fail)
        {
            failCount++;
            FishingManager.Instance.CallFishingRoundEvent(GetRandomFishSpeed());
        }
        else
        {
            failCount = 0;
            curLerpTime = Mathf.Max(curLerpTime - decreaseLerpTime, minLerpTime);
        }
    }
}
