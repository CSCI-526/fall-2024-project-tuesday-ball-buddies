using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeControl : MonoBehaviour
{
    private float shrinkSpeed = 2f;
    private float widthIncreasePerPress = 0.25f;  
    private bool isActive = false;
    private Vector3 initialScale;
    private bool isResetting = false;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (isActive) {
            Vector3 currentScale = transform.localScale;

            /*if (Input.GetKeyDown(KeyCode.J))
            {
                float newWidth = currentScale.x + widthIncreasePerPress;
                transform.localScale = new Vector3(newWidth, currentScale.y, currentScale.z);
            } 
            else 
            {
                float newXScale = Mathf.Max(0, currentScale.x - shrinkSpeed * Time.deltaTime);
                transform.localScale = new Vector3(newXScale, currentScale.y, currentScale.z);
            }*/

            if (transform.localScale.x == 0)
            {
                gameObject.SetActive(false); 
            }

             // If we are in the process of resetting
            if (isResetting)
        {
            // Smoothly transition the scale to the initial scale
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * 2);

            // Stop resetting once close enough to the original size
            if (Vector3.Distance(transform.localScale, initialScale) < 0.01f)
            {
                transform.localScale = initialScale;
                isResetting = false;
                Debug.Log("Bridge reset to original size.");
            }
        }

        } 
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void ResetBridge()
    {
        isResetting = true;  // Trigger the smooth reset
        gameObject.SetActive(true);  // Ensure the bridge is active
        Debug.Log("Bridge reset to its initial state");
        transform.localScale = initialScale;
        SetActive(false);
    }
}
