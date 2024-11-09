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
                    powerUpMaterial.color = Color.magenta;
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
        Renderer ballRenderer = ball.GetComponent<Renderer>();
        switch (powerUpType)
        {
            case PowerUpType.Smaller:
                ball.transform.localScale *= 0.75f;
                ball.GetComponent<Rigidbody>().mass *= 4f;
                ball.SetJumpForce(ball.jumpForce * 4f);
                if (ballRenderer != null)
                {
                    ballRenderer.material.color = Color.blue;
                }
                break;
            case PowerUpType.Bigger:
                ball.transform.localScale *= 1.75f;
                ball.GetComponent<Rigidbody>().mass *= 0.5f;
                ball.SetJumpForce(ball.jumpForce * 0.5f);
                if (ballRenderer != null)
                {
                    ballRenderer.material.color = Color.green;
                }
                break;
            case PowerUpType.HigherJump:
                ball.SetJumpForce(7000f);
                break;
                if (ballRenderer != null)
                {
                    ballRenderer.material.color = Color.magenta;
                }
        }
    }
}
