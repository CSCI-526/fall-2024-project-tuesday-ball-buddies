using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatStrength = 1f;  
    public float floatSpeed = 1f;     
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * floatSpeed) * floatStrength, 0);
    }
}
