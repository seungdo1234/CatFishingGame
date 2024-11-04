using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDataInitialSystem : MonoBehaviour
{
    [SerializeField] private FishingSystem fishingSystem;
    [SerializeField] private TimerEventHanlder timerEventHanlder;
    [SerializeField] private FishingResultEventHandler fishingResultEvent;
    [SerializeField] private ScoreTextEventHandler scoreTextEvent;
    [SerializeField] private ComboEventHandler comboEventHandler;
    [SerializeField] private FeverUIEvnetHandler FeverUIEvnetHandler;

    public void GameInit()
    {
        scoreTextEvent.ScoreTextInit();
        fishingSystem.FishingInit(scoreTextEvent.transform);
        timerEventHanlder.TimerInit();
        fishingResultEvent.FishingTextInit();
        comboEventHandler.ComboEventInit();
        FeverUIEvnetHandler.FeverGaugeInit();
    }


}
