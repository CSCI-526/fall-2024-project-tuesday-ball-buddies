using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public List<GameObject> waypoints;
    public float moveSpeed = 1f;
    public float turnSpeed = 90f;

    private int curWaypointId = 0;
    private bool isTurning = false;
    private Vector3 targetDir, targetEulerAngle;
    private Quaternion targetRot;
    private float targetAngle;

    // Start is called before the first frame update
    void Start()
    {
        if (waypoints.Count == 0)
            this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurning)
            Turn();
        else
            Move();
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
        }
        else
            transform.localPosition += targetDir * moveSpeed * Time.deltaTime;
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
}
