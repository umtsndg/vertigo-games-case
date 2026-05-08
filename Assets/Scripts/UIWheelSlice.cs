using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWheelSlice : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;

    public void SetupSlice(RewardData data)
    {
        iconImage.sprite = data.icon;

        if (data.rewardType == RewardType.Bomb)
        {
            valueText.text = "BOMB";
        }
        else
        {
            valueText.text = "x" + data.amount.ToString();
        }
    }
}