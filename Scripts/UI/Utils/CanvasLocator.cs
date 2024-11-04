using System;
using System.Collections.Generic;
using UnityEngine;

public enum E_CanvasName { HUDCanvas, InGameCanvas, BackGroundCanvas, StartAnimationCanvas, LobbyCanvas, GameOverCanvas }
public class CanvasLocator : MonoBehaviour
{
    [SerializeField] private E_CanvasName[] activateCompileCanvasArr;

    private Dictionary<int , GameObject> canvasDict = new Dictionary<int , GameObject>();

    private void Awake()
    {
        CanvasLocatorInit();
    }

    private void Start()
    {
        UIManager.Instance.CanvasLocatorInit(this);
    }

    private void CanvasLocatorInit()
    {
        foreach (E_CanvasName canvasName in Enum.GetValues(typeof(E_CanvasName)))
        {
            Transform findCanvas = transform.Find(canvasName.ToString());

            if (findCanvas != null)
            {
                findCanvas.gameObject.SetActive(Array.Exists(activateCompileCanvasArr, element => canvasName == element));

                canvasDict[(int)canvasName] = findCanvas.gameObject;
            }
            else
            {
                Debug.LogError($"{canvasName} is Empty");
            }
        }
    }

    public GameObject GetCanvas(E_CanvasName canvasName)
    {
        if (canvasDict.TryGetValue((int)canvasName, out GameObject canvas))
        {
            return canvas;
        }
        return null;
    }
}
