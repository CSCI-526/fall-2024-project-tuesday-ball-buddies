using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{
    // Arrays to hold the toggles for each level
    public Toggle[] level1Toggles;
    public Toggle[] level2Toggles;
    public Toggle[] level3Toggles;

    void Start()
    {
        // Find all objects with StarControl in the scene
        StarControl[] allStars = FindObjectsOfType<StarControl>();

        // Loop through each star and assign the correct level's toggles
        foreach (StarControl star in allStars)
        {
            if (star.levelIndex == 0) // Level 1
            {
                star.levelToggles = level1Toggles;
            }
            else if (star.levelIndex == 1) // Level 2
            {
                star.levelToggles = level2Toggles;
            }
            else if (star.levelIndex == 2) // Level 3
            {
                star.levelToggles = level3Toggles;
            }
            else if (star.levelIndex != -1)
            {
                Debug.LogError("Invalid level index for star: " + star.name);
            }
        }
    }
}
