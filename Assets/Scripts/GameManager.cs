using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private WheelController wheelController;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textZoneValue;

    [Header("Zone Data")]
    [SerializeField] private ZoneConfiguration normalZoneData;
    [SerializeField] private ZoneConfiguration safeZoneData;
    [SerializeField] private ZoneConfiguration superZoneData;

    public static GameManager Instance { get; private set; }

    private int currentZone = 1;

    private void Start()
    {
        // Start the game by loading the very first zone
        LoadZone(currentZone);
    }

    private void LoadZone(int zoneNumber)
    {
        // 1. Update the UI Text
        textZoneValue.text = "ZONE " + zoneNumber;

        // 2. Determine what type of zone this is based on the rules
        ZoneConfiguration currentDataToLoad;

        if (zoneNumber % 30 == 0) // Every 30th zone is Super 
        {
            currentDataToLoad = superZoneData;
            Debug.Log("Entering SUPER Zone!");
        }
        else if (zoneNumber % 5 == 0) // Every 5th zone is Safe 
        {
            currentDataToLoad = safeZoneData;
            Debug.Log("Entering SAFE Zone!");
        }
        else // Otherwise, it's a normal zone with a bomb [cite: 9, 10]
        {
            currentDataToLoad = normalZoneData;
            Debug.Log("Entering Normal Zone.");
        }

        // 3. Tell the WheelController to physically build the slices
        wheelController.BuildWheel(currentDataToLoad);
    }

    // We will call this from your Spin Button shortly!
    public void AdvanceToNextZone()
    {
        currentZone++;
        LoadZone(currentZone);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerSpin()
    {
        Debug.Log("Spinning the wheel...");
        wheelController.SpinWheel(HandleSpinResult);
    }

    private void HandleSpinResult(int winningSliceIndex)
    {
        // For now, we will just print the result. 
        // Next, we will check if it's a bomb or a reward!
        Debug.Log($"The wheel stopped! You landed on slice index: {winningSliceIndex}");
    }
}