using DG.Tweening;
using UnityEngine;

public class MenuPlayer : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float delay;
    [SerializeField] private float moveDuration;
    [SerializeField] private Vector3 playerStart;
    [SerializeField] private Vector3 playerTarget;
    [SerializeField] private float playerDuration;
    private Vector3 startPoint;
    private bool toTarget;
    private Tweener tweener;
    private bool initialized;
    private bool lastInitialized;

    private void Start() {

        transform.position = playerStart;
        transform.DOMove(playerTarget, playerDuration).OnComplete(() => {

            startPoint = transform.position;
            toTarget = true;

            initialized = true;

        });
    }

    private void FixedUpdate() {

        if ((tweener != null && tweener.IsActive()) || !initialized) {

            lastInitialized = initialized;
            return;

        }

        if (lastInitialized == initialized) // prevents flip on first time
            transform.Rotate(0f, 180f, 0f);

        tweener = transform.DOMove(toTarget ? targetPoint.position : startPoint, moveDuration).SetDelay(delay).OnComplete(() => {

            toTarget = !toTarget;

        }).SetEase(Ease.Linear);
    }
}
