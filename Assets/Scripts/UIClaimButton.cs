using UnityEngine;

public class UIClaimButton : UIButtonBase
{
    protected override void OnClickAction()
    {
        GameManager.Instance.ExitToLobby();
    }
}