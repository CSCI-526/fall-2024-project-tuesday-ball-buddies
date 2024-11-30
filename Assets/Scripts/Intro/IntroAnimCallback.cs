using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimCallback : MonoBehaviour
{
    private IntroManager introManager;
    private static int panelExitCount;

    private void Start()
    {
        introManager = FindObjectOfType<IntroManager>();
        panelExitCount = 0;
    }

    void OnCameraAnimExit()
    {
        introManager.StartFirstPanelAnim();
    }

    void OnPanelAnimHalf()
    {
        introManager.StartNextPanelAnim(gameObject);
    }

    void OnPanelAnimExit()
    {
        panelExitCount++;
        if (panelExitCount == 3)
            introManager.StartInstructionAnim();
    }
}
