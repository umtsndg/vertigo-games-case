using UnityEngine;

public enum RewardType
{
    Currency,
    ItemPoint,
    Bomb,
    Weapon,
    Consumable,
    Armor,
    Chest
}

[CreateAssetMenu(fileName = "NewRewardData", menuName = "VertigoDemo/Reward Data")]
public class RewardData : ScriptableObject
{
    public RewardType rewardType;
    public string rewardName;
    public int amount;
    public Sprite icon;
}