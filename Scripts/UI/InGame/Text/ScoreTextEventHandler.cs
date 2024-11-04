using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreTextEventHandler : MonoBehaviour
{
    [Header("# Text Data Info")]
    [SerializeField] private float textDamping;

    private TextMeshProUGUI scoreText;

    public void ScoreTextInit()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        FishingManager.Instance.OnFishingScoreEvent += UpdateScoreResult;
    }

    private void UpdateScoreResult(int score, bool isPunch)
    {
        scoreText.text = score.ToString();
        if (isPunch) {
            UIManager.Instance.UIAnimation.PunchAnimation(scoreText.transform, textDamping);
        }
    }

}
