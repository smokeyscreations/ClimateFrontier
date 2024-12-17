using UnityEngine;
using TMPro;

public class GoldUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGoldChanged += HandleGoldChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGoldChanged -= HandleGoldChanged;
        }
    }

    private void Start()
    {
        // Initialize the text if needed
        if (GameManager.Instance != null)
        {
            goldText.text = GameManager.Instance.levelGold.ToString();
        }
    }

    private void HandleGoldChanged(int newGold)
    {
        goldText.text = newGold.ToString();
    }
}
