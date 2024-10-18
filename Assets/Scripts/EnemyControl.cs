using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public List<GameObject> waypoints;
    public float moveSpeed = 5f;
    public float turnSpeed = 90f;

    private CheckpointManager checkpointManager;
    private int curWaypointId = 0;
    private bool isTurning = false;
    private Vector3 targetDir, targetEulerAngle;
    private Quaternion targetRot;
    private float targetAngle;
    private int frameCounter = 0; // Frame counter
    private int updateInterval = 5; // Refresh every 100 frames
    

    // Start is called before the first frame update
    void Start()
    {
        if (waypoints.Count == 0)
        {
            this.enabled = false;
            Debug.LogError("No waypoints assigned to the enemy!");
        }
        
        checkpointManager = CheckpointManager.Instance;
        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager not found in the scene!");
        }
        // Debug.Log("EnemyControl initialized");
    }

    // Update is called once per frame
    void Update()
    {
        frameCounter++;

        // Only update logic every 'updateInterval' frames
        if (frameCounter >= updateInterval)
        {
            frameCounter = 0; // Reset counter
            if (isTurning)
                Turn();
            else
                Move();
        }
    }

    private bool IsReached(Vector3 targetPos)
    {
        return Vector3.Distance(transform.localPosition, targetPos) < moveSpeed * Time.deltaTime;
    }

    private void ComputeTurn(Vector3 targetPos)
    {
        targetDir = (targetPos - transform.localPosition).normalized;
        targetEulerAngle = transform.localEulerAngles;
        float targetYAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
        targetEulerAngle.y = targetYAngle;
        targetAngle = Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.y, targetEulerAngle.y));
        isTurning = true;
    }

    private void Move()
    {
        Vector3 targetPos = waypoints[curWaypointId].transform.localPosition;
        if (IsReached(targetPos))
        {
            transform.localPosition = targetPos;
            curWaypointId = (curWaypointId + 1) % waypoints.Count;
            ComputeTurn(waypoints[curWaypointId].transform.localPosition);
            // Debug.Log("Reached waypoint " + curWaypointId);
        }
        else
        {
            // Debug.Log("Moving Speed is" + moveSpeed);
            Vector3 v = targetDir * moveSpeed * Time.deltaTime;
            transform.localPosition += v;
            // Debug.Log("Moving to waypoint " + "by" + targetDir +  moveSpeed +"x"+ Time.deltaTime);
        }
    }

    private void Turn()
    {
        if (turnSpeed * Time.deltaTime > targetAngle)
        {
            transform.localEulerAngles = targetEulerAngle;
            targetAngle = 0f;
            isTurning = false;
        }
        else
        {
            float angleToTurn = turnSpeed * Time.deltaTime;
            float newYAngle = Mathf.MoveTowardsAngle(transform.localEulerAngles.y, targetEulerAngle.y, angleToTurn);
            Vector3 newEulerAngles = transform.localEulerAngles;
            newEulerAngles.y = newYAngle;
            transform.localEulerAngles = newEulerAngles;

            targetAngle = Mathf.Abs(Mathf.DeltaAngle(newYAngle, targetEulerAngle.y));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BallControl ball = other.GetComponent<BallControl>();
        if (ball != null)
        {
            if (checkpointManager != null && checkpointManager.HasCheckpoint())
            {
                Debug.Log("Restarting from checkpoint");
                ball.RestartFromCheckpoint();
            }
            else
            {
                Debug.Log("Restarting game");
                ball.RestartGame();
            }
        }
    }
}
