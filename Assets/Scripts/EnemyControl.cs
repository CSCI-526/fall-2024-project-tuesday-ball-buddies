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
    private float targetAngle;
    private int frameCounter = 0;
    private int updateInterval = 5;

    private Renderer[] childRenderers;
    private Color colorStart = Color.red;
    private Color colorEnd = new Color(1f, 0.65f, 0f); // Orange color
    private float colorChangeSpeed = 1f;
    private float lerpTime = 0f;

    void Start()
    {
        if (waypoints.Count == 0)
        {
            this.enabled = false;
            Debug.LogError("No waypoints assigned to the enemy!");
        }
        else
        {
            Debug.Log(waypoints);
            Debug.Log(waypoints.Count);
        }

        checkpointManager = CheckpointManager.Instance;
        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager not found in the scene!");
        }

        // Get all Renderer components from child objects
        childRenderers = GetComponentsInChildren<Renderer>();

        if (childRenderers.Length == 0)
        {
            Debug.LogWarning("No Renderer found in child objects! Color change will be skipped.");
        }
    }

    void Update()
    {
        frameCounter++;

        if (frameCounter >= updateInterval)
        {
            frameCounter = 0;
            if (isTurning)
                Turn();
            else
                Move();
        }

        if (childRenderers.Length > 0)
        {
            HandleColorChange();
        }
    }

    private void HandleColorChange()
    {
        lerpTime += Time.deltaTime * colorChangeSpeed;
        float lerpValue = Mathf.PingPong(lerpTime, 1f);
        Color currentColor = Color.Lerp(colorStart, colorEnd, lerpValue);

        foreach (Renderer renderer in childRenderers)
        {
            renderer.material.color = currentColor;
        }
    }

    private bool IsReached(Vector3 targetPos)
    {

        bool res = Vector3.Distance(transform.localPosition, targetPos) < moveSpeed * Time.fixedDeltaTime;
        Debug.Log("Vector3.Distance(transform.localPosition, targetPos)" +
            Vector3.Distance(transform.localPosition, targetPos));
        Debug.Log("moveSpeed * Time.fixedDeltaTime" + moveSpeed * Time.fixedDeltaTime);
        if (res)
        {
            Debug.Log("reached");
        }
        else
        {
            Debug.Log("Not reached");
        }

        return res;
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
        }
        else
        {
            Vector3 v = targetDir * moveSpeed * Time.fixedDeltaTime;
            Debug.Log("Time.deltaTime" + Time.fixedDeltaTime);
            Debug.Log("moveSpeed" + moveSpeed);
            Debug.Log("targetDir" + targetDir);
            transform.localPosition += v;
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