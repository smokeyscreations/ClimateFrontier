using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoFadeController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("CanvasGroup component of the black background Image.")]
    public CanvasGroup blackBackgroundCanvasGroup;

    [Tooltip("CanvasGroup component of the Logo Image.")]
    public CanvasGroup logoCanvasGroup;

    [Header("Fade Settings")]
    [Tooltip("Duration of the logo fade-in effect in seconds.")]
    public float logoFadeInDuration = 2f;

    [Tooltip("Delay before transitioning to the main menu, after logo has faded in.")]
    public float delayBeforeTransition = 2f;

    [Tooltip("Duration of the black background fade-out effect in seconds.")]
    public float blackFadeOutDuration = 1f;

    [Header("Scene Settings")]
    [Tooltip("Name of the main menu scene to load.")]
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        // Validate that CanvasGroups are assigned
        if (blackBackgroundCanvasGroup == null || logoCanvasGroup == null)
        {
            Debug.LogError("LogoFadeController: CanvasGroups not assigned.");
            return;
        }

        // Initialize CanvasGroups' alpha values
        blackBackgroundCanvasGroup.alpha = 1f; // Black background starts fully opaque
        logoCanvasGroup.alpha = 0f; // Logo starts fully transparent

        // Begin the fade-in process
        StartCoroutine(FadeInLogo());
    }

    /// <summary>
    /// Coroutine to handle the fade-in of the logo and transition to the main menu.
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeInLogo()
    {
        // Fade in the logo over logoFadeInDuration seconds
        logoCanvasGroup.DOFade(1f, logoFadeInDuration).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(logoFadeInDuration);

        // Wait for a specified delay before transitioning
        yield return new WaitForSeconds(delayBeforeTransition);

        // Fade out the black background to reveal the main menu
        blackBackgroundCanvasGroup.DOFade(0f, blackFadeOutDuration).SetEase(Ease.InQuad);

        // Start loading the main menu scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneName);
        asyncLoad.allowSceneActivation = false; // Prevent automatic activation

        // Wait for the fade-out to complete
        yield return new WaitForSeconds(blackFadeOutDuration);

        // Allow the main menu scene to activate
        asyncLoad.allowSceneActivation = true;

        
        Destroy(gameObject);
    }
}
