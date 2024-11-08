using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform level2Waypoint;  // Reference to the waypoint for level 2
    public float movementSpeed = 2f;   // Speed of movement

    private float currentTargetPosition;  // Target position the elevator is moving towards
    private bool isMoving;                // Flag to track if the elevator is currently moving
    private float startingPosition;       // Initial starting Y position of the elevator

    void Start()
    {
        startingPosition = transform.position.y;  // Set starting position to the object's initial Y position
        currentTargetPosition = level2Waypoint.position.y;   // Set the initial target to the waypoint's Y position
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        float step = movementSpeed * Time.deltaTime;
        Vector3 targetPosition = new Vector3(transform.position.x, currentTargetPosition, transform.position.z);
        Debug.Log("Target Position: " + targetPosition); // Log the target position
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // Check if reached the current target position with an exact y-axis comparison
        if (Mathf.Approximately(transform.position.y, currentTargetPosition))
        {
            StartCoroutine(PauseAndChangeLevel());
        }
    }

    IEnumerator PauseAndChangeLevel()
    {
        isMoving = false;
        yield return new WaitForSeconds(2f);  // Pause for 2 seconds

        // Toggle target position between starting position and level 2 waypoint
        if (Mathf.Approximately(currentTargetPosition, startingPosition))
        {
            currentTargetPosition = level2Waypoint.position.y;  // Move to level 2 waypoint
        }
        else
        {
            currentTargetPosition = startingPosition;  // Move back to starting position
        }

        isMoving = true;  // Resume movement towards the new target
    }
}