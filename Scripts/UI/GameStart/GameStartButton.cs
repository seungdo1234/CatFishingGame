using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour
{
    private Button startBtn;

    private void Awake()
    {
        startBtn = GetComponent<Button>();
        startBtn.onClick.AddListener(GameStartEvent);
    }

    private void GameStartEvent()
    {
        GameObject startAnimationCanvas = UIManager.Instance.CanvasLocator.GetCanvas(E_CanvasName.StartAnimationCanvas);
        
        startAnimationCanvas.GetComponent<GameStartEventHandler>().StartCountDown();
        
        UIManager.Instance.CanvasLocator.GetCanvas(E_CanvasName.LobbyCanvas).SetActive(false);
    }
}
