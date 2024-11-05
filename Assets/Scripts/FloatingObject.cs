using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatStrength = 1f;
    public float floatSpeed = 1f;
    public float rotationSpeed = 30f;
    private Vector3 localStartPos;

    void Start()
    {
        localStartPos = transform.localPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        Vector3 worldTargetPos = transform.parent.TransformPoint(localStartPos) + Vector3.up * yOffset;
        transform.position = worldTargetPos;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}