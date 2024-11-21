using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { None, Smaller, Bigger, HigherJump }
    public PowerUpType powerUpType;

    [Header("Button Press Effect")]
    public float bounceForce = 2000f; // Adjustable in inspector

    void Start()
    {
        // Get components once
        Renderer renderer = GetComponent<Renderer>();
        Collider collider = GetComponent<Collider>();

        if (powerUpType == PowerUpType.None)
        {
            if (renderer != null) renderer.enabled = false;
            if (collider != null) collider.enabled = false;
            return;
        }

        // Regular power-up setup
        gameObject.tag = "PowerUp";
        
        if (collider != null)
        {
            collider.isTrigger = true;
        }

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
            // Scale down the Y axis to make it look pressed
            Vector3 newScale = transform.localScale;
            newScale.y *= 0.25f;
            transform.localScale = newScale;
            
            // Add upward and backward bounce force to the ball
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                Vector3 bounceDirection = (Vector3.back).normalized;
                ballRb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
            
            // Activate the power-up effect
            ActivatePowerUp(ball);
            
            // Disable the collider
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
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
                if (ballRenderer != null)
                {
                    ballRenderer.material.color = Color.magenta;
                }
                break;
        }
    }
}
