using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBallAnim : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        float rand = Random.value;
        if (rand < 0.0001f)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * 2000, ForceMode.Impulse);
        }
    }
}
