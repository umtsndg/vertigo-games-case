using UnityEngine;
using DG.Tweening;

public class UILobbyRotator : MonoBehaviour
{
    [Tooltip("How many seconds it takes to complete one full spin.")]
    [SerializeField] private float spinDuration = 10f;

    private void Start()
    {
        // Infinite, smooth, linear rotation using DOTween
        transform.DORotate(new Vector3(0, 0, -360), spinDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}