using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatStrength = 1f;
    public float floatSpeed = 1f;
    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startLocalPos + new Vector3(0, Mathf.Sin(Time.time * floatSpeed) * floatStrength, 0);
    }
}
