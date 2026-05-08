using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Core References")]
    [SerializeField] private WheelController wheelController;
    [SerializeField] private GameObject losePopupPanel;

    [Header("Inventory UI References")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private UIInventoryItem inventoryItemPrefab;
    [SerializeField] private TextMeshProUGUI textZoneValue;

    [Header("Dynamic Loot System")]
    [Tooltip("Drag EVERY RewardData asset in your project into this list")]
    [SerializeField] private List<RewardData> masterRewardPool;

    private int currentZone = 1;

    // We need to store the generated slices so we know what we landed on
    private List<RewardData> currentActiveSlices = new List<RewardData>();

    private Dictionary<string, int> collectedAmounts = new Dictionary<string, int>();
    private Dictionary<string, UIInventoryItem> inventoryUIElements = new Dictionary<string, UIInventoryItem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadZone(currentZone);
    }

    private void LoadZone(int zoneNumber)
    {
        textZoneValue.text = "ZONE " + zoneNumber;

        // Figure out what zone type this is
        bool isSuper = zoneNumber % 30 == 0;
        bool isSafe = zoneNumber % 5 == 0 && !isSuper;

        currentActiveSlices = GenerateSlicesForZone(zoneNumber);

        // --- UPDATED: Pass the safe/super info to the wheel! ---
        wheelController.BuildWheel(currentActiveSlices, isSafe, isSuper);
    }

    // --- THE DYNAMIC GENERATOR ALGORITHM ---
    private List<RewardData> GenerateSlicesForZone(int zone)
    {
        List<RewardData> generated = new List<RewardData>();

        bool isSuper = zone % 30 == 0;
        bool isSafe = zone % 5 == 0 && !isSuper;
        int multiplier = isSuper ? 10 : (isSafe ? 3 : 1);

        List<RewardData> allowedStandard = new List<RewardData>();
        List<RewardData> chests = new List<RewardData>();
        List<RewardData> weaponsAndArmor = new List<RewardData>();
        RewardData bombData = null;

        // 1. Sort the Master Pool based on rules and inventory
        foreach (RewardData data in masterRewardPool)
        {
            if (data.rewardType == RewardType.Bomb)
            {
                bombData = data;
            }
            else if (data.rewardType == RewardType.Chest)
            {
                chests.Add(data);
            }
            else if (data.rewardType == RewardType.Weapon || data.rewardType == RewardType.Armor)
            {
                // Weapon Duplicate Protection!
                if (data.rewardType == RewardType.Weapon && collectedAmounts.ContainsKey(data.rewardName))
                    continue; // Skip this weapon, we already own it

                weaponsAndArmor.Add(data);
            }
            else
            {
                allowedStandard.Add(data); // Cash, Gold, etc.
            }
        }

        // 2. Fulfill Guarantees & Drawbacks
        if (isSuper)
        {
            if (weaponsAndArmor.Count > 0)
                generated.Add(CloneAndMultiply(weaponsAndArmor[Random.Range(0, weaponsAndArmor.Count)], multiplier));
            if (chests.Count > 0)
                generated.Add(CloneAndMultiply(chests[Random.Range(0, chests.Count)], multiplier));
        }
        else if (!isSafe && bombData != null)
        {
            // Normal zones get exactly 1 bomb
            generated.Add(bombData);
        }

        // 3. Build the pool of valid remaining items for this zone
        List<RewardData> validFillers = new List<RewardData>(allowedStandard);
        if (isSafe || isSuper) validFillers.AddRange(chests);
        if (isSuper) validFillers.AddRange(weaponsAndArmor);

        // 4. Fill the rest of the 8 slots randomly
        while (generated.Count < 8)
        {
            RewardData randomPick = validFillers[Random.Range(0, validFillers.Count)];
            generated.Add(CloneAndMultiply(randomPick, multiplier));
        }

        // 5. Shuffle the list so Bombs/Guarantees aren't always in the same slot!
        for (int i = 0; i < generated.Count; i++)
        {
            RewardData temp = generated[i];
            int randomIndex = Random.Range(i, generated.Count);
            generated[i] = generated[randomIndex];
            generated[randomIndex] = temp;
        }

        return generated;
    }

    // Safely applies multipliers without corrupting your project files
    private RewardData CloneAndMultiply(RewardData original, int multiplier)
    {
        RewardData clone = Instantiate(original); // Create a safe runtime copy

        if (clone.rewardType == RewardType.Weapon)
            clone.amount = 1; // Weapons strictly fixed to 1
        else if (clone.rewardType != RewardType.Bomb)
            clone.amount *= multiplier; // Multiply everything else (except bombs)

        return clone;
    }

    public void TriggerSpin()
    {
        wheelController.SpinWheel(HandleSpinResult);
    }

    private void HandleSpinResult(int winningSliceIndex)
    {
        // Read from our generated runtime list!
        RewardData wonReward = currentActiveSlices[winningSliceIndex];

        if (wonReward.rewardType == RewardType.Bomb)
        {
            losePopupPanel.SetActive(true);
        }
        else
        {
            AddRewardToInventory(wonReward);
            AdvanceToNextZone();
        }
    }

    private void AddRewardToInventory(RewardData reward)
    {
        if (collectedAmounts.ContainsKey(reward.rewardName))
        {
            collectedAmounts[reward.rewardName] += reward.amount;
            inventoryUIElements[reward.rewardName].UpdateValue(collectedAmounts[reward.rewardName]);
        }
        else
        {
            collectedAmounts.Add(reward.rewardName, reward.amount);
            UIInventoryItem newItemUI = Instantiate(inventoryItemPrefab, inventoryContainer);
            newItemUI.Setup(reward.icon, reward.amount);
            inventoryUIElements.Add(reward.rewardName, newItemUI);
        }
    }

    public void AdvanceToNextZone()
    {
        currentZone++;
        LoadZone(currentZone);
    }

    public void RestartGame()
    {
        currentZone = 1;
        collectedAmounts.Clear();
        foreach (var uiItem in inventoryUIElements.Values) Destroy(uiItem.gameObject);
        inventoryUIElements.Clear();

        LoadZone(currentZone);
        losePopupPanel.SetActive(false);
    }
}