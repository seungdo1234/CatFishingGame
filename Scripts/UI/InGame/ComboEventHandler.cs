using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ComboEventHandler : MonoBehaviour
{
    [SerializeField] private float animationDuration;
    [SerializeField] private float moveYOffset;
    [SerializeField] private int curComboCount;

    private TextMeshProUGUI comboText;
    private Image comboImage;

    private Vector2 imageInitPos;

    public void ComboEventInit()
    {
        comboText = GetComponentInChildren<TextMeshProUGUI>();
        comboImage = GetComponent<Image>();

        imageInitPos = comboImage.transform.position;

        comboImage.gameObject.SetActive(false);

        FishingManager.Instance.OnFishingResultEvent += UpdateCombo;
    }

    private void UpdateCombo(E_FishingResultType type)
    {
        if (type == E_FishingResultType.Fail) {
            curComboCount = 0;
        } else {
            ComboImageAnimation();
            FishingManager.Instance.ComboUpdate(++curComboCount);
            comboText.text = curComboCount.ToString();
        }
    }

    private void ComboImageAnimation()
    {
        comboImage.gameObject.SetActive(true);
        comboImage.transform.position = imageInitPos;
        UIManager.Instance.UIAnimation.MoveYAnimation(comboImage.transform, animationDuration, moveYOffset, ResetCombo );
        UIManager.Instance.UIAnimation.FadeUI.TryImageFadeEvent(comboImage, animationDuration);
        UIManager.Instance.UIAnimation.FadeUI.TryTextFadeEvent(comboText, animationDuration);
    }

    private void ResetCombo()
    {
        curComboCount = 0;
    }
}
