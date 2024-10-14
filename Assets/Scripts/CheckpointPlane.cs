using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointPlane : MonoBehaviour
{
    private Renderer checkpointRenderer;

    void Start()
    {
        checkpointRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        BallControl ball = other.GetComponent<BallControl>();
        if (ball != null)
        {
            // Set the checkpoint for the ball
            ball.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint set at: " + transform.position);

            // Change the color of the checkpoint to green
            ChangeCheckpointColor(Color.green);
        }
    }

    // Method to change the color of the checkpoint plane
    private void ChangeCheckpointColor(Color color)
    {
        if (checkpointRenderer != null)
        {
            checkpointRenderer.material.color = color;
        }
        else
        {
            Debug.LogWarning("No Renderer found on the CheckpointPlane!");
        }
    }
}
