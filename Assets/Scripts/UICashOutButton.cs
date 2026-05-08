using UnityEngine;

public class UICashOutButton : UIButtonBase
{
    protected override void OnClickAction()
    {
        GameManager.Instance.CashOut();
    }
}