using UnityEngine;

public class UIEntryButton : UIButtonBase
{
    [SerializeField] private bool useCash;

    protected override void OnClickAction()
    {
        bool success = false;

        if (useCash) success = GameManager.Instance.TryStartWithCash();
        else success = GameManager.Instance.TryStartWithGold();


        if (!success)
        {
            ShakeError();
        }
    }
}