using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { None, Smaller, Bigger, HigherJump }
    public PowerUpType powerUpType;

    void Start()
    {
        // Get components once
        Renderer renderer = GetComponent<Renderer>();
        Collider collider = GetComponent<Collider>();

        if (powerUpType == PowerUpType.None)
        {
            // Disable the renderer and collider for "None" type
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            if (collider != null)
            {
                collider.enabled = false;
            }
            return;  // Skip the rest of the setup for "None" type
        }

        // Regular power-up setup
        gameObject.tag = "PowerUp";
        
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        // Create and set a new material that won't be shared
        if (renderer != null)
        {
            Material powerUpMaterial = new Material(Shader.Find("Standard"));
            
            switch (powerUpType)
            {
                case PowerUpType.Smaller:
                    powerUpMaterial.color = Color.blue;
                    break;
                case PowerUpType.Bigger:
                    powerUpMaterial.color = Color.green;
                    break;
                case PowerUpType.HigherJump:
                    powerUpMaterial.color = Color.yellow;
                    break;
            }
            
            renderer.material = powerUpMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BallControl ball = other.GetComponent<BallControl>();
        if (ball != null)
        {
            ActivatePowerUp(ball);
            Destroy(transform.parent.gameObject);
        }
    }

    public void ActivatePowerUp(BallControl ball)
    {
        switch (powerUpType)
        {
            case PowerUpType.Smaller:
                ball.transform.localScale *= 0.75f;
                ball.GetComponent<Rigidbody>().mass *= 2.5f;
                ball.SetJumpForce(ball.jumpForce * 2.5f);
                break;
            case PowerUpType.Bigger:
                ball.transform.localScale *= 2f;
                ball.GetComponent<Rigidbody>().mass *= 2f;
                ball.SetJumpForce(ball.jumpForce * 2f);
                break;
            case PowerUpType.HigherJump:
                ball.SetJumpForce(8000f);
                break;
        }
    }
}
