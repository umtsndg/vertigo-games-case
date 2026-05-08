using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Success Screen System")]
    [SerializeField] private GameObject successOverlay;
    [SerializeField] private Transform successGridContainer;

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

    [Header("Currency System")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private TextMeshProUGUI lobbyCashText;
    [SerializeField] private TextMeshProUGUI lobbyGoldText;

    [Header("Main Game UI")]
    [SerializeField] private TextMeshProUGUI gameCashValue;
    [SerializeField] private TextMeshProUGUI gameGoldValue;

    [Header("Revive System")]
    [SerializeField] private TextMeshProUGUI reviveButtonText;
    private int currentReviveCost = 20;
    private int currentZone = 1;

    private bool isSpinning = false;

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
        UpdateUI();
        lobbyPanel.SetActive(true); // Turn on the lobby when the game starts           
    }


    private void UpdateUI()
    {
        string cashStr = CurrencyManager.Instance.TotalCash.ToString();
        string goldStr = CurrencyManager.Instance.TotalGold.ToString();

        // Update Lobby
        if (lobbyCashText != null) lobbyCashText.text = cashStr;
        if (lobbyGoldText != null) lobbyGoldText.text = goldStr;

        // Update Main Game 
        if (gameCashValue != null) gameCashValue.text = cashStr;
        if (gameGoldValue != null) gameGoldValue.text = goldStr;

        // Update Revive Button
        if (reviveButtonText != null) reviveButtonText.text = $"REVIVE ({currentReviveCost})";
    }
    public bool TryStartWithCash()
    {
        if (CurrencyManager.Instance.TotalCash >= 100)
        {
            CurrencyManager.Instance.TotalCash -= 100;
            BeginRun();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void BeginRun()
    {
        lobbyPanel.SetActive(false); // Hide the lobby
        currentReviveCost = 20; // Reset revive cost for this run
        UpdateUI();            // Refresh the text to show the missing money
        LoadZone(currentZone);       // NOW we build the wheel!
    }
    public bool TryStartWithGold()
    {
        if (CurrencyManager.Instance.TotalGold >= 10)
        {
            CurrencyManager.Instance.TotalGold -= 10;
            BeginRun();
            return true;
        }
        else
        {
            return false;
        }

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
                // Weapon Duplicate Protection
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
        if (isSpinning) return;

        isSpinning = true;

        // 3. Spin the wheel
        wheelController.SpinWheel(HandleSpinResult); // Or whatever your HandleResult method is called
    }

    private void HandleSpinResult(int winningSliceIndex)
    {
        // Read from our generated runtime list!
        RewardData wonReward = currentActiveSlices[winningSliceIndex];

        isSpinning = false;

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
        ResetRunData(); // Clears all UI and data
        LoadZone(currentZone); // Starts fresh at Zone 1
        losePopupPanel.SetActive(false);
    }
    public void CashOut()
    {
        successOverlay.SetActive(true);

        // Loop through everything you won
        foreach (var reward in collectedAmounts)
        {
            // 1. Add to permanent bank
            if (reward.Key == "Cash") CurrencyManager.Instance.TotalCash += reward.Value;
            else if (reward.Key == "Gold") CurrencyManager.Instance.TotalGold += reward.Value;

            // 2. Spawn the visual icon in the success grid
            UIInventoryItem itemUI = Instantiate(inventoryItemPrefab, successGridContainer);

            // 3. Find the correct picture for this item and set it up!
            Sprite rewardIcon = GetIconForReward(reward.Key);
            itemUI.Setup(rewardIcon, reward.Value);
        }

        UpdateUI();
    }

    private Sprite GetIconForReward(string itemName)
    {
        foreach (var data in masterRewardPool)
        {
            if (data.rewardName == itemName) return data.icon;
        }
        return null; // Returns nothing if it can't find it
    }

    public void ExitToLobby()
    {
        ResetRunData(); // Clears all UI and data
        successOverlay.SetActive(false);
        losePopupPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        UpdateUI();
    }

    public bool Revive()
    {
        // 1. Check if they have enough permanent Gold
        if (CurrencyManager.Instance.TotalGold >= currentReviveCost)
        {
            // 2. Take the gold
            CurrencyManager.Instance.TotalGold -= currentReviveCost;

            // 3. Double the cost for the *next* time they hit a bomb in this run
            currentReviveCost *= 2;

            // 4. Hide the lose screen so they can see the wheel again
            losePopupPanel.SetActive(false);

            // 5. Update the UI text
            UpdateUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ResetRunData()
    {
        // 1. Clear the data dictionaries
        collectedAmounts.Clear();
        inventoryUIElements.Clear();

        // 2. Physically destroy icons in the Main Game Grid
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        // 3. Physically destroy icons in the Success Popup Grid
        foreach (Transform child in successGridContainer)
        {
            Destroy(child.gameObject);
        }

        // 4. Reset game state variables
        currentZone = 1;
        currentReviveCost = 20;
    }
}