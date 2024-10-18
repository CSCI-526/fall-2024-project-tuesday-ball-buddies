using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformControl : MonoBehaviour
{
    private float sensitivity = 40f;
    private bool isActive = false;
    private Vector3 initialPosition;

    private Vector3 centerPoint;
    private Vector3 currentRotation;

    void Start()
    {
        CalculateCenterPoint();
        // Store the initial position
        initialPosition = transform.position;
        currentRotation = Vector3.zero;
    }

    void Update()
    {
        if (isActive)
        {
            float rotationX = Input.GetKey(KeyCode.UpArrow) ? sensitivity * Time.deltaTime :
                              Input.GetKey(KeyCode.DownArrow) ? -sensitivity * Time.deltaTime : 0f;
            float rotationZ = Input.GetKey(KeyCode.LeftArrow) ? sensitivity * Time.deltaTime :
                              Input.GetKey(KeyCode.RightArrow) ? -sensitivity * Time.deltaTime : 0f;

            currentRotation.x += rotationX;
            currentRotation.z += rotationZ;

            // Keep y Rotation at 0
            Quaternion targetRotation = Quaternion.Euler(currentRotation.x, 0f, currentRotation.z);

            RotateAroundPoint(targetRotation, centerPoint);
        }
    }

    void CalculateCenterPoint()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            sum += child.position;
            count++;
        }
        centerPoint = sum / count;
    }

    void RotateAroundPoint(Quaternion targetRotation, Vector3 point)
    {
        Vector3 centerToPosition = transform.position - point;
        centerToPosition = targetRotation * Quaternion.Inverse(transform.rotation) * centerToPosition;
        transform.position = point + centerToPosition;
        transform.rotation = targetRotation;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void ResetPlatform()
    {
        // Reset platform to its initial position and flat rotation
        SetActive(false);
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity; // Set to flat rotation
        currentRotation = Vector3.zero; // Reset the currentRotation
        CalculateCenterPoint(); // Recalculate the center point
    }
}