using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action OnGameOverEvent;
    public event Action OnGameStartEvent;
    public bool IsGameOver { get; private set; }
    
    public void GameOver()
    {
        IsGameOver = true;
        GameOverTask().Forget();
    }

    private async UniTaskVoid GameOverTask()
    {
        if (!FishingManager.Instance.IsFishingClear()) {
            await UniTask.WaitUntil(() => FishingManager.Instance.IsFishingClear());
            await UniTask.Delay(1000);
        }
        
        OnGameOverEvent?.Invoke();
        UIManager.Instance.CanvasLocator.GetCanvas(E_CanvasName.GameOverCanvas).GetComponent<GameOverPanel>().OnGameOverPanel();
    }
    
    public void GameStart()
    {
        OnGameStartEvent?.Invoke();
    }
}
