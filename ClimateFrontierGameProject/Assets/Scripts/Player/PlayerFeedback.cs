using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerFeedback : MonoBehaviour
{
    public MMFeedbacks Feedbacks;
    public void PlayBlinkFeedback()
    {
        Feedbacks.PlayFeedbacks();
    }
}
