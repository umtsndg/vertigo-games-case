using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class WheelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform wheelVisualTransform;
    [SerializeField] private UIWheelSlice slicePrefab;

    [Header("Wheel Visuals")]
    [SerializeField] private Image wheelBaseImage;
    [SerializeField] private Image wheelIndicatorImage;

    [Header("Wheel Sprites")]
    [SerializeField] private Sprite bronzeBase;
    [SerializeField] private Sprite bronzeIndicator;
    [SerializeField] private Sprite silverBase;
    [SerializeField] private Sprite silverIndicator;
    [SerializeField] private Sprite goldBase;
    [SerializeField] private Sprite goldIndicator;

    private const int TOTAL_SLICES = 8;
    private const float SLICE_ANGLE = 360f / TOTAL_SLICES;

    public void BuildWheel(List<RewardData> generatedSlices, bool isSafe, bool isSuper)
    {
        // 1. Change the wheel visuals based on the zone!
        if (isSuper)
        {
            wheelBaseImage.sprite = goldBase;
            wheelIndicatorImage.sprite = goldIndicator;
        }
        else if (isSafe)
        {
            wheelBaseImage.sprite = silverBase;
            wheelIndicatorImage.sprite = silverIndicator;
        }
        else
        {
            wheelBaseImage.sprite = bronzeBase;
            wheelIndicatorImage.sprite = bronzeIndicator;
        }

        // 2. Reset rotation and clear old slices
        wheelVisualTransform.localRotation = Quaternion.identity;
        foreach (Transform child in wheelVisualTransform)
        {
            Destroy(child.gameObject);
        }

        // 3. Spawn the new slices
        for (int i = 0; i < TOTAL_SLICES; i++)
        {
            UIWheelSlice newSlice = Instantiate(slicePrefab, wheelVisualTransform);
            newSlice.transform.localRotation = Quaternion.Euler(0, 0, -i * SLICE_ANGLE);
            newSlice.SetupSlice(generatedSlices[i]);
        }
    }

    public void SpinWheel(System.Action<int> onSpinComplete)
    {
        int randomSpins = Random.Range(3, 6);
        int targetSliceIndex = Random.Range(0, TOTAL_SLICES);

        float targetAngle = (randomSpins * 360f) - (targetSliceIndex * SLICE_ANGLE);

        wheelVisualTransform.DORotate(new Vector3(0, 0, -targetAngle), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                // When the animation is 100% finished, tell the GameManager which slice won
                onSpinComplete?.Invoke(targetSliceIndex);
            });
    }
}