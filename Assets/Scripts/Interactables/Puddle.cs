
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Puddle : Interactable {

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    [Header("Color")]
    [SerializeField][Range(0, 1)] private float alphaRemoval;
    [SerializeField] private float fadeDuration;
    private float targetAlpha;
    private Tweener fadeTweener;

    [Header("Interacting")]
    [SerializeField] private float interactDelay;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();

        targetAlpha = spriteRenderer.color.a;

    }

    public override void Interact() {

        if (targetAlpha <= 0f) // target alpha is already 0, don't reduce it anymore, let current tween finish
            return;

        targetAlpha -= alphaRemoval;

        if (fadeTweener != null)
            fadeTweener.Kill();

        fadeTweener = spriteRenderer.DOFade(targetAlpha - alphaRemoval, fadeDuration).OnComplete(() => {

            if (targetAlpha <= 0f) {

                taskManager.CompleteCurrentTask();
                Destroy(gameObject);
                return;

            }
        });

        StartCoroutine(InteractCooldown());

    }

    private IEnumerator InteractCooldown() {

        isInteractable = false;
        yield return new WaitForSeconds(interactDelay);
        isInteractable = true;

    }
}