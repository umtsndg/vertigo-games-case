using UnityEngine;
using DG.Tweening;

public class WheelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform wheelVisualTransform;
    [SerializeField] private UIWheelSlice slicePrefab;

    private const int TOTAL_SLICES = 8;
    private const float SLICE_ANGLE = 360f / TOTAL_SLICES;

    public void BuildWheel(ZoneConfiguration zoneData)
    {
        // 1. Clear any old slices if we are regenerating
        foreach (Transform child in wheelVisualTransform)
        {
            Destroy(child.gameObject);
        }

        // 2. Spawn exactly 8 slices based on the ScriptableObject
        for (int i = 0; i < TOTAL_SLICES; i++)
        {
            UIWheelSlice newSlice = Instantiate(slicePrefab, wheelVisualTransform);

            // Rotate each slice so they form a perfect circle
            newSlice.transform.localRotation = Quaternion.Euler(0, 0, -i * SLICE_ANGLE);

            // Pass the ScriptableObject data to the slice
            newSlice.SetupSlice(zoneData.wheelSlices[i]);
        }
    }

    public void SpinWheel()
    {
        // Calculate a random rotation (spin 3 to 5 full times, plus a random slice angle)
        int randomSpins = Random.Range(3, 6);
        int targetSliceIndex = Random.Range(0, TOTAL_SLICES);

        float targetAngle = (randomSpins * 360f) + (targetSliceIndex * SLICE_ANGLE);

        // DOTween magic: Rotate the visual transform, not the root!
        wheelVisualTransform.DORotate(new Vector3(0, 0, -targetAngle), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                Debug.Log($"Wheel landed on slice index: {targetSliceIndex}");
                // We will add the logic to claim the reward or explode next!
            });
    }
}