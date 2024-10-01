using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0, 50, -40);
    public Canvas hudCanvas;
    public HUDManager hudManager;

    void LateUpdate() {
        transform.position = player.transform.position + offset;

        if (hudCanvas != null)
        {
            hudCanvas.transform.position = transform.position + transform.forward * 10;
            hudCanvas.transform.rotation = transform.rotation;
        }
    }
}
