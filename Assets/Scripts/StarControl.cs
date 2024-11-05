using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarControl : MonoBehaviour
{
    public static int starCount = 0;
    public StarAnalysis starAnalysis; // Reference to StarAnalysis

    // Index of the level this star belongs to
    public int levelIndex;  // 0 for level 1, 1 for level 2, 2 for level 3

    // Array to store the toggles for this star's level
    public Toggle[] levelToggles;

    // Index of this star's corresponding toggle in the array
    public int toggleIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // Increment the star count
            starCount++;

            // Record the star collection in StarAnalysis
            starAnalysis.RecordStar(levelIndex + 1); // levelIndex is 0-based, so add 1

            // Update the corresponding toggle color to yellow
            ChangeToggleColor(toggleIndex, Color.yellow);

            // Destroy the star object to make it disappear
            Destroy(gameObject);
        }
    }

    // Method to change the color of the toggle
    private void ChangeToggleColor(int index, Color color)
    {
        // Ensure the index is valid
        if (levelToggles != null && index >= 0 && index < levelToggles.Length)
        {
            ColorBlock cb = levelToggles[index].colors;
            cb.normalColor = color;  // Change the normal color to yellow
            levelToggles[index].colors = cb;
        }
        else
        {
            Debug.LogWarning("Toggle index out of range or level toggles not assigned!");
        }
    }
}
