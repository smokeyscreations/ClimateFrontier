using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogoGlowAnimation : MonoBehaviour
{
    [Header("Outline Effect Settings")]
    public Outline outlineEffect;
    public float glowDuration = 1f;
    public float glowStrength = 5f;
    public Color glowColor = Color.white;

    private Tween colorTween;
    private Tween distanceTween;

    void Start()
    {
        if (outlineEffect == null)
        {
            outlineEffect = GetComponent<Outline>();
        }

        if (outlineEffect != null)
        {
            // Set initial outline properties
            outlineEffect.effectColor = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
            outlineEffect.effectDistance = Vector2.zero;

            // Start the glow animation
            AnimateGlow();
        }
        else
        {
            Debug.LogWarning("Outline component is missing on this GameObject.");
        }
    }

    void AnimateGlow()
    {
        if (outlineEffect == null) return;

        // Animate the outline effect color alpha from 0 to 1 and back
        colorTween = DOTween.ToAlpha(
            () => outlineEffect.effectColor,
            x => {
                if (outlineEffect != null)
                    outlineEffect.effectColor = x;
            },
            1f, // Target alpha
            glowDuration / 2
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo)
        .SetTarget(outlineEffect); // Link the tween to the outlineEffect

        // Animate the effect distance for the glow strength
        distanceTween = DOTween.To(
            () => outlineEffect.effectDistance,
            x => {
                if (outlineEffect != null)
                    outlineEffect.effectDistance = x;
            },
            new Vector2(glowStrength, glowStrength),
            glowDuration / 2
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo)
        .SetTarget(outlineEffect); // Link the tween to the outlineEffect
    }

    void OnDisable()
    {
        // Kill the tweens to prevent accessing destroyed objects
        if (colorTween != null && colorTween.IsActive())
        {
            colorTween.Kill();
        }
        if (distanceTween != null && distanceTween.IsActive())
        {
            distanceTween.Kill();
        }
    }

    void OnDestroy()
    {
        // Ensure all DOTween animations are killed on destroy
        if (outlineEffect != null)
        {
            DOTween.Kill(outlineEffect);
        }
    }
}
