using UnityEngine;

public enum ZoneType
{
    Normal,
    Safe,
    Super
}

[CreateAssetMenu(fileName = "NewZoneConfig", menuName = "VertigoDemo/Zone Configuration")]
public class ZoneConfiguration : ScriptableObject
{
    public ZoneType typeOfZone;

    [Tooltip("Assign exactly 8 RewardData assets here.")]
    public RewardData[] wheelSlices = new RewardData[8];
}