using UnityEngine;

public class UIRestartButton : UIButtonBase
{
    protected override void OnClickAction()
    {
        GameManager.Instance.RestartGame();
    }
}