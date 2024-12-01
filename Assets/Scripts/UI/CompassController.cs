using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    public GameObject compass, compassFloater;
    private BallControl ball;

    // Start is called before the first frame update
    void Start()
    {
        ball = FindAnyObjectByType<BallControl>();
    }

    // Update is called once per frame
    void Update()
    {
        PlatformControl currentPlatform = ball.GetCurrentPlatformControl();
        if (currentPlatform == null)
            compass.SetActive(false);
        else
        {
            compass.SetActive(true);
            compassFloater.transform.rotation = currentPlatform.transform.rotation;
        }
    }
}
