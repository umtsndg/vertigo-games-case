using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public abstract class UIButtonBase : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    protected Image buttonImage;
    private Vector3 originalScale;

    protected virtual void Awake()
    {
        buttonImage = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickAction();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * 0.95f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    protected abstract void OnClickAction();

    public void ShakeError()
    {
        // Kills any current tweens on this object so it doesn't glitch if spammed
        transform.DOKill(true);

        // Shakes horizontally by 15 pixels, vibrating 25 times over 0.4 seconds
        transform.DOShakePosition(0.4f, new Vector3(15, 0, 0), 25, 90, false, true);
    }
}