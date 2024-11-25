using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimCallback : MonoBehaviour
{
    void OnCameraAnimExit()
    {
        GameObject.Find("IntroManager").GetComponent<IntroManager>().StartFirstPanelAnim();
    }

    void OnPanelAnimHalf()
    {
        GameObject.Find("IntroManager").GetComponent<IntroManager>().StartNextPanelAnim(gameObject);
    }
}
