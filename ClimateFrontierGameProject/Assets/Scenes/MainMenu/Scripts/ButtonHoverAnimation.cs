using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;

    [Header("UI Components")]
    public Image backgroundImage;
    public TextMeshProUGUI buttonText;

    private Color originalBackgroundColor;
    private Color hoverBackgroundColor;

    private Color originalTextColor;
    private Color hoverTextColor;

    private bool isPointerDown = false;

    private Tween scaleUpTween;
    private Tween scaleDownTween;
    private Tween backgroundColorTween;
    private Tween textColorTween;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        // Get the Image component if not assigned
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }

        // Get the text component if not assigned
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Store original colors
        if (backgroundImage != null)
        {
            originalBackgroundColor = backgroundImage.color;
            hoverBackgroundColor = originalBackgroundColor * 0.8f; // Darker color
            hoverBackgroundColor.a = originalBackgroundColor.a; // Keep original alpha
        }

        if (buttonText != null)
        {
            originalTextColor = buttonText.color;
            hoverTextColor = new Color32(255, 255, 200, 255); // Brighter text color
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPointerDown || rectTransform == null) return; // Prevent hover animation if button is pressed or destroyed

        // Kill any existing tweens to prevent conflicts
        KillTweens();

        // Scale up with a bounce effect
        scaleUpTween = rectTransform.DOScale(originalScale * 1.1f, 0.2f)
            .SetEase(Ease.OutBack)
            .SetTarget(rectTransform);

        // Change background color to a darker shade
        if (backgroundImage != null)
        {
            backgroundColorTween = backgroundImage.DOColor(hoverBackgroundColor, 0.2f)
                .SetTarget(backgroundImage);
        }

        // Animate text color to a brighter color
        if (buttonText != null)
        {
            textColorTween = buttonText.DOColor(hoverTextColor, 0.2f)
                .SetTarget(buttonText);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPointerDown || rectTransform == null) return;

        // Kill any existing tweens to prevent conflicts
        KillTweens();

        // Scale back to original size
        scaleDownTween = rectTransform.DOScale(originalScale, 0.2f)
            .SetEase(Ease.OutBack)
            .SetTarget(rectTransform);

        // Reset background color to original
        if (backgroundImage != null)
        {
            backgroundColorTween = backgroundImage.DOColor(originalBackgroundColor, 0.2f)
                .SetTarget(backgroundImage);
        }

        // Reset text color to original color
        if (buttonText != null)
        {
            textColorTween = buttonText.DOColor(originalTextColor, 0.2f)
                .SetTarget(buttonText);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        isPointerDown = true;

        // Kill any existing tweens to prevent conflicts
        KillTweens();

        // Scale down slightly to indicate click
        scaleDownTween = rectTransform.DOScale(originalScale * 0.95f, 0.1f)
            .SetEase(Ease.InOutQuad)
            .SetTarget(rectTransform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        isPointerDown = false;

        // Kill any existing tweens to prevent conflicts
        KillTweens();

        // Return to hover scale or original scale based on pointer position
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            // Pointer is still over the button
            scaleUpTween = rectTransform.DOScale(originalScale * 1.1f, 0.2f)
                .SetEase(Ease.OutBack)
                .SetTarget(rectTransform);
        }
        else
        {
            // Pointer has left the button
            scaleDownTween = rectTransform.DOScale(originalScale, 0.2f)
                .SetEase(Ease.OutBack)
                .SetTarget(rectTransform);
        }
    }

    void OnDisable()
    {
        // Kill all tweens when the object is disabled to prevent accessing destroyed objects
        KillTweens();
    }

    void OnDestroy()
    {
        // Ensure all DOTween animations are killed on destroy
        DOTween.Kill(rectTransform);
        DOTween.Kill(backgroundImage);
        DOTween.Kill(buttonText);
    }

    private void KillTweens()
    {
        if (scaleUpTween != null && scaleUpTween.IsActive())
        {
            scaleUpTween.Kill();
        }
        if (scaleDownTween != null && scaleDownTween.IsActive())
        {
            scaleDownTween.Kill();
        }
        if (backgroundColorTween != null && backgroundColorTween.IsActive())
        {
            backgroundColorTween.Kill();
        }
        if (textColorTween != null && textColorTween.IsActive())
        {
            textColorTween.Kill();
        }
    }
}
