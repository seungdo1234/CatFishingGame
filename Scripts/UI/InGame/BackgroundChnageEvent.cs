using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundChnageEvent : MonoBehaviour
{
    [Header("# BackGround Resource Info")]
    [SerializeField] private Sprite inGameBackGround;
    [SerializeField] private Sprite lobbyBackGround;

    [Header("# BackGround Component Info")]
    [SerializeField]  private Image backgroundImage;


    void Start()
    {
        GameManager.Instance.OnGameStartEvent += ChangeInGameBackGround;
        GameManager.Instance.OnGameOverEvent += ChangeLobbyBackGround;
    }

    private void ChangeInGameBackGround()
    {
        backgroundImage.sprite = inGameBackGround;
    }
    private void ChangeLobbyBackGround()
    {
        backgroundImage.sprite = lobbyBackGround;
    }
}
