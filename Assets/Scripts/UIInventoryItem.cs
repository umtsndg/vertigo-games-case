using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Setup(Sprite icon, int amount)
    {
        iconImage.sprite = icon;
        UpdateValue(amount);
    }

    public void UpdateValue(int amount)
    {
        valueText.text = "x" + amount.ToString();
    }
}