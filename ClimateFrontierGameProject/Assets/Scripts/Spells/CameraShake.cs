using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Singleton pattern for easy access
    public static CameraShake Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Shake parameters
    public IEnumerator Shake(float duration, float magnitude, int vibrato = 10, float randomness = 90f)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * progress - 3.0f, 0.0f, 1.0f); // Ease out

            float x = Random.Range(-1f, 1f) * magnitude * damper;
            float y = Random.Range(-1f, 1f) * magnitude * damper;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    // Optional: Shaker method with vibrato and randomness parameters
    public void TriggerShake(float duration, float magnitude, int vibrato = 10, float randomness = 90f)
    {
        StartCoroutine(Shake(duration, magnitude, vibrato, randomness));
    }
}
