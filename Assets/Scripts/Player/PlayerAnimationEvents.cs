using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public void FinishHitReaction()
    {
        if (playerHealth != null)
            playerHealth.FinishHitReaction();
    }

    public void FinishRespawn()
    {
        if (playerHealth != null)
            playerHealth.FinishRespawn();
    }
}