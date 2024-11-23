using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour, IPoolable
{
    private TextMeshPro textMesh;
    private Transform cameraTransform;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();

        if (textMesh == null)
        {
            Debug.LogError("FloatingText requires a TextMeshPro component.");
        }

        // Ensure the GameObject is inactive initially
        gameObject.SetActive(false);
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (cameraTransform != null)
        {
            // Make the floating text face the camera
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }

    public void OnObjectSpawn()
    {
        // Reset the text color to fully opaque
        Color originalColor = textMesh.color;
        originalColor.a = 1f;
        textMesh.color = originalColor;

        // Ensure the text is active
        gameObject.SetActive(true);

        // Play the floating animation
        PlayFloatingAnimation();
    }

    public void OnObjectReturn()
    {
        // Ensure the object is inactive when returned to the pool
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the text to display.
    /// </summary>
    /// <param name="text">The damage number to display.</param>
    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }

    /// <summary>
    /// Plays the floating animation using DOTween.
    /// </summary>
    private void PlayFloatingAnimation()
    {
        // Define the duration of the animation
        float duration = 1f;

        // Move the text upwards by 2 units over the duration
        transform.DOMoveY(transform.position.y + 2f, duration).SetEase(Ease.OutQuad);

        // Optional: Add a shake effect for impact
        transform.DOShakePosition(duration, strength: 0.2f, vibrato: 10, randomness: 90, fadeOut: true);

        // Optional: Scale up briefly for a pop effect
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).SetLoops(2, LoopType.Yoyo);

        // Fade out the text over the duration by tweening the TextMeshPro color alpha
        textMesh.DOFade(0f, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            // Return to pool after animation completes
            ObjectPooler.Instance.ReturnToPool("FloatingText", gameObject);
        });
    }

    private void OnDisable()
    {
        // Ensure all tweens are killed when the object is disabled to prevent lingering animations
        DOTween.Kill(transform);
    }
}
