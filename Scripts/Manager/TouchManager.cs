using System;
using UnityEngine;

public class TouchManager : Singleton<TouchManager>
{
    public event Action OnTouchBeganEvent;
    public event Action OnTouchMovedEvent;
    public event Action OnTouchEndedEvent;

    // Editor
    private Touch simulatedTouch;
    private bool isSimulatingTouch = false;

    private void Update()
    {
        if (GameManager.Instance.IsGameOver)
            return;

#if UNITY_EDITOR
        ProcessEditorInput();
#else
        ProcessTouches();
#endif
    }

    private void ProcessTouches()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            ProcessTouch(touch);
        }
    }

    private void ProcessEditorInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            simulatedTouch = CreateSimulatedTouch(TouchPhase.Began);
            isSimulatingTouch = true;
            ProcessTouch(simulatedTouch);
        }
        else if (Input.GetMouseButton(0) && isSimulatingTouch)
        {
            simulatedTouch = CreateSimulatedTouch(TouchPhase.Moved);
            ProcessTouch(simulatedTouch);
        }
        else if (Input.GetMouseButtonUp(0) && isSimulatingTouch)
        {
            simulatedTouch = CreateSimulatedTouch(TouchPhase.Ended);
            isSimulatingTouch = false;
            ProcessTouch(simulatedTouch);
        }
    }

    private Touch CreateSimulatedTouch(TouchPhase touchPhase)
    {
        return new Touch
        {
            phase = touchPhase,
            position = Input.mousePosition,
            fingerId = 0
        };
    }

    private void ProcessTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                OnTouchBeganEvent?.Invoke();
                break;
            case TouchPhase.Moved:
                OnTouchMovedEvent?.Invoke();
                break;
            case TouchPhase.Ended:
                OnTouchEndedEvent?.Invoke();
                break;
        }
    }
}