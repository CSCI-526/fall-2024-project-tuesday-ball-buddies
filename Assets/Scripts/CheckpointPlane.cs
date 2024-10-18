using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointPlane : MonoBehaviour
{
    private Renderer checkpointRenderer;
    private StageTimeManager stageTimeManager;
    private bool checkpointEntered = false;

    void Start()
    {
        checkpointRenderer = GetComponent<Renderer>();
        stageTimeManager = FindObjectOfType<StageTimeManager>();
    }



    private void OnTriggerEnter(Collider other)
    {
        BallControl ball = other.GetComponent<BallControl>();
        if (ball != null && !checkpointEntered)
        {
            // Set the checkpoint for the ball
            ball.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint set at: " + transform.position);

            // Checkpoint could only entered once
            checkpointEntered = true;

            // Record the time
            stageTimeManager.EnterCheckpoint();

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
