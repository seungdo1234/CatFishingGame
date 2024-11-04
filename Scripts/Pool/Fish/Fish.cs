using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Fish : PoolObject
{
    [Header("# Fish Move Event Info")]
    [SerializeField] private AnimationCurve curve;
    
    private float targetPosY;
    private float lerpTime;
    public void FishMoveInit(Vector2 startPos, Vector2 targetPos, AnimationCurve curve, float lerpTime, bool isPunch)
    {
        targetPosY = targetPos.y;
        this.lerpTime = lerpTime;
        this.curve = curve;
        transform.position = startPos;
       
        MoveTask(isPunch).Forget();
    }
    
    private async UniTaskVoid MoveTask(bool isPunch)
    {
        float elapsedTime = 0f, fishPosY = transform.position.y;

        while (elapsedTime < lerpTime) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lerpTime;

            float xValue = curve.Evaluate(t);
            float yValue = Mathf.Lerp(fishPosY, targetPosY, t);

            transform.position = new Vector3(xValue, yValue, 0);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        FishingManager.Instance.UpdateFishingCount(isPunch);
        gameObject.SetActive(false);
    }
}
