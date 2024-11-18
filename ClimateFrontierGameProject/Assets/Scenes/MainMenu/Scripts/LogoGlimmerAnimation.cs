using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogoGlimmerAnimation : MonoBehaviour
{
    public RectTransform glimmerRect;
    public float glimmerDuration = 2f;
    public float delayBetweenGlimmers = 5f;

    void Start()
    {
        if (glimmerRect != null)
        {
            StartGlimmerAnimation();
        }
    }

    void StartGlimmerAnimation()
    {
        // Get the width of the logo
        RectTransform logoRect = GetComponent<RectTransform>();
        float logoWidth = logoRect.rect.width;

        // Set the starting position of the glimmer
        glimmerRect.anchoredPosition = new Vector2(-logoWidth, 0);

        // Animate the glimmer across the logo
        glimmerRect.DOAnchorPosX(logoWidth, glimmerDuration).SetEase(Ease.InOutSine)
            .SetDelay(delayBetweenGlimmers)
            .OnComplete(() => StartGlimmerAnimation()); // Loop the animation
    }
}
