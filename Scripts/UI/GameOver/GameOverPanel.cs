using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [Header("# Game Over Panel Component")] 
    [SerializeField] private Button menuBtn;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI ComboText;
    [SerializeField] private TextMeshProUGUI RewardText;
    [SerializeField] private float rewardDuration;


    public void OnGameOverPanel()
    {
        AddMenuBtnEvent(ClickedMenuButton);
        
        gameObject.SetActive(true);

        // 점수 집계 및 보상 설정
        ScoreText.text = $"{FishingManager.Instance.Score}점";
        ComboText.text = $"{FishingManager.Instance.MaxComboCount} Combo !";
        
        AnimateNumber(0, FishingManager.Instance.GetReward);
    }

    private void AnimateNumber(int startValue, int endValue)
    {
        DOTween.To(() => startValue, x => 
        {
            startValue = x;
            RewardText.text = startValue.ToString(); 
        }, endValue, rewardDuration).SetEase(Ease.Linear);
    }
    
    public void AddMenuBtnEvent(UnityAction func)
    {
        menuBtn.onClick.AddListener(func);
    }
    
    private void ClickedMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}