using UnityEngine;

public class UIReviveButton : UIButtonBase
{
    protected override void OnClickAction()
    {
        bool success = GameManager.Instance.Revive();

        if (!success)
        {
            ShakeError();
        }
    }
}