using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Smaller, Bigger, HigherJump }
    public PowerUpType powerUpType;

    void Start()
    {
        // Ensure proper setup
        gameObject.tag = "PowerUp";
        
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BallControl ball = other.GetComponent<BallControl>();
        if (ball != null)
        {
            ActivatePowerUp(ball);
            // Destroy the parent enemy object instead of just the power-up
            Destroy(transform.parent.gameObject);
        }
    }

    public void ActivatePowerUp(BallControl ball)
    {
        switch (powerUpType)
        {
            case PowerUpType.Smaller:
                ball.transform.localScale *= 0.5f;
                break;
            case PowerUpType.Bigger:
                ball.transform.localScale *= 1.5f;
                break;
            case PowerUpType.HigherJump:
                ball.SetJumpForce(8000f);
                break;
        }
    }
}
