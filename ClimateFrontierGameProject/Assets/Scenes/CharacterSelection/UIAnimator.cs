using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class UIAnimator : MonoBehaviour
{
    [System.Serializable]
    public class AnimatedBar
    {
        public RectTransform bar;       // The RectTransform of the bar
        public float maxSize = 700f;    // Maximum size for the bar
    }

    [Header("Animated Bars")]
    public List<AnimatedBar> animatedBars; // List of bars with their respective properties

    [Header("Info Panel")]
    public RectTransform infoPanel;              // The info panel RectTransform
    public CanvasGroup infoPanelCanvasGroup;     // CanvasGroup for controlling alpha
    public float animationDuration = 1.5f;       // Duration for animations

    // Desired final position for the info panel
    private Vector2 infoPanelFinalPosition = new Vector2(48.79498f, -188f);

    // Initial offset position (slightly to the left and higher)
    private Vector2 infoPanelInitialOffset = new Vector2(-50f, 50f); // Adjust the X and Y offsets as needed

    private void Start()
    {
        // Initialize each bar's pivot and size
        foreach (var animatedBar in animatedBars)
        {
            if (animatedBar.bar != null)
            {
                animatedBar.bar.pivot = new Vector2(0, 0.5f); // Pivot to the left
                animatedBar.bar.sizeDelta = new Vector2(0, animatedBar.bar.sizeDelta.y); // Start size
            }
            else
            {
                Debug.LogError("AnimatedBar is missing its RectTransform.");
            }
        }

        // Configure the info panel's initial state
        if (infoPanel != null && infoPanelCanvasGroup != null)
        {
            // Set the initial anchored position with the offset
            infoPanel.anchoredPosition = infoPanelFinalPosition + infoPanelInitialOffset;

            // Set the initial alpha to 0 (fully transparent)
            infoPanelCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("Info Panel or CanvasGroup is not assigned in UIAnimator.");
        }
    }

    public void PlayBarsAnimation()
    {
        int completedAnimations = 0;

        foreach (var animatedBar in animatedBars)
        {
            if (animatedBar.bar != null)
            {
                // Reset bar size to 0 before starting
                animatedBar.bar.sizeDelta = new Vector2(0, animatedBar.bar.sizeDelta.y);

                // Animate bar to maxSize
                animatedBar.bar
                    .DOSizeDelta(new Vector2(animatedBar.maxSize, animatedBar.bar.sizeDelta.y), animationDuration / 2)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        // Animate bar back to 0
                        animatedBar.bar
                            .DOSizeDelta(new Vector2(0, animatedBar.bar.sizeDelta.y), animationDuration / 2)
                            .SetEase(Ease.InQuad)
                            .OnComplete(() =>
                            {
                                completedAnimations++;
                                // Optionally, handle when all bars have completed their animations
                                if (completedAnimations == animatedBars.Count)
                                {
                                    // You can add a callback here if needed
                                }
                            });
                    });
            }
            else
            {
                Debug.LogError("AnimatedBar is missing its RectTransform.");
            }
        }
    }


    public void ShowInfoPanel()
    {
        if (infoPanel != null && infoPanelCanvasGroup != null)
        {
            // Animate the alpha from 0 to 1 (fade in)
            infoPanelCanvasGroup.DOFade(1f, animationDuration)
                .SetEase(Ease.OutQuad);

            // Animate the position from offset to final position
            infoPanel.DOAnchorPos(infoPanelFinalPosition, animationDuration)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            Debug.LogError("Info Panel or CanvasGroup is not assigned in UIAnimator.");
        }
    }


    public void HideInfoPanel()
    {
        if (infoPanel != null && infoPanelCanvasGroup != null)
        {
            // Animate the alpha from 1 to 0 (fade out)
            infoPanelCanvasGroup.DOFade(0f, animationDuration)
                .SetEase(Ease.InQuad);

            // Animate the position from final position back to offset
            infoPanel.DOAnchorPos(infoPanelFinalPosition + infoPanelInitialOffset, animationDuration)
                .SetEase(Ease.InQuad);
        }
        else
        {
            Debug.LogError("Info Panel or CanvasGroup is not assigned in UIAnimator.");
        }
    }


}
