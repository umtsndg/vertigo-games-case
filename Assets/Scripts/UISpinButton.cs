using UnityEngine;

public class UISpinButton : UIButtonBase
{
    protected override void OnClickAction()
    {
        GameManager.Instance.TriggerSpin();
    }
}