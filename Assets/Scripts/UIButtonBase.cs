using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class UIButtonBase : MonoBehaviour
{
    [SerializeField] protected Button buttonComponent;


    private void OnValidate()
    {
        if (buttonComponent == null)
        {
            buttonComponent = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (buttonComponent != null)
        {

            buttonComponent.onClick.AddListener(OnClickAction);
        }
    }

    private void OnDisable()
    {
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveListener(OnClickAction);
        }
    }

    protected abstract void OnClickAction();
}