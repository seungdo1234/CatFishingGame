using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvasEvent : MonoBehaviour
{
    [Header("# HUD Canvas Componet Info")]
    [SerializeField] private Button StopBtn;

    private void Start()
    {
        GameManager.Instance.OnGameStartEvent += GameStartEvent;
        GameManager.Instance.OnGameOverEvent += GameOverEvnet;
    }

    private void GameStartEvent()
    {
        StopBtn.gameObject.SetActive(true);
    }

    private void GameOverEvnet()
    {
        StopBtn.gameObject.SetActive(false);
    }
}
