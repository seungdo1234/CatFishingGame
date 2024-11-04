using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FishingZoneSystem : MonoBehaviour
{
    [Header("Fishing Zone Info")]
    [SerializeField] private int zoneBaseHeight;
    [SerializeField] private float perfectZoneRatio;
    [SerializeField] private float zoneWidthOffset;
    [SerializeField] private RectTransform fishMoveZoneRect;
    [SerializeField] private float perfectZoneMoveTime;

    private RectTransform baseZoneRect;
    private RectTransform perfectZoneRect;
    private FishingSystem fishingSystem;
    private float zoneHeight;

    public void FishingZoneInit(FishingSystem fishingSystem)
    {
        baseZoneRect = GetComponent<RectTransform>();
        perfectZoneRect = transform.GetChild(0).GetComponent<RectTransform>();
        this.fishingSystem = fishingSystem;

        FishingManager.Instance.OnFishingSettingEvent += SetRandomFishingZone;
        gameObject.SetActive(false);
    }

    private void SetRandomFishingZone()
    {
        ActivateGameObject();
        float zoneWidth = CalculateZoneWidth();
        SetZoneHeight();
        PerfectZoneMove(zoneWidth, zoneHeight);
    }

    private void ActivateGameObject()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    private float CalculateZoneWidth()
    {
        return fishingSystem.SliderRect.sizeDelta.x - zoneWidthOffset;
    }

    private void SetZoneHeight()
    {
        if (fishingSystem.IsFever)
        {
            zoneHeight = 50f;
        }
        else
        {
            int halfHeight = zoneBaseHeight / 2;
            zoneHeight = (zoneBaseHeight + Random.Range(0, halfHeight + 1)) * perfectZoneRatio;
        }
        perfectZoneRect.sizeDelta = new Vector2(perfectZoneRect.sizeDelta.x, zoneHeight);
    }

    private void PerfectZoneMove(float zoneWidth, float zoneHeight)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(perfectZoneRect.DOSizeDelta(new Vector2(zoneWidth, zoneHeight), perfectZoneMoveTime));
        sequence.OnComplete(() => perfectZoneRect.sizeDelta = new Vector2(zoneWidth, zoneHeight));
        sequence.Play();
    }

    public E_FishingResultType IsMatchedZone(RectTransform fish)
    {
        float fishPosY = fish.position.y;
        float matchZoneHeight = zoneHeight / 2;

        if (IsFishingInZone(fishPosY, matchZoneHeight))
        {
            return IsFishingInZone(fishPosY, matchZoneHeight / 2) ? E_FishingResultType.Perfect : E_FishingResultType.Good;
        }
        return E_FishingResultType.Fail;
    }

    private bool IsFishingInZone(float fishPosY, float matchZoneHeight)
    {
        float baseY = baseZoneRect.position.y;
        return fishPosY >= baseY - matchZoneHeight && fishPosY <= baseY + matchZoneHeight;
    }
}