using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarControl : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatStrength = 0.5f;
    private float floatSpeed = 5f;
    public float rotationSpeed = 30f;
    private Vector3 localStartPos;

    [Header("Score & Statistics")]
    public static int starCount = 0;
    public StarAnalysis starAnalysis; // Reference to StarAnalysis

    // Index of the level this star belongs to
    public int levelIndex;  // 0 for level 1, 1 for level 2, 2 for level 3

    // Array to store the toggles for this star's level
    public Toggle[] levelToggles;

    // Index of this star's corresponding toggle in the array
    public int toggleIndex;

    void Start()
    {
        localStartPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // Increment the star count
            starCount++;

            // Record the star collection in StarAnalysis
            if (levelIndex != -1)
            {
                starAnalysis.RecordStar(levelIndex + 1, toggleIndex); // levelIndex is 0-based, so add 1

                // Update the corresponding toggle color to yellow
                ChangeToggleColor(toggleIndex, Color.yellow);
            }

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
        else if (index != -1)
        {
            Debug.LogWarning("Toggle index out of range or level toggles not assigned!");
        }
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        Vector3 worldTargetPos = transform.parent.TransformPoint(localStartPos) + Vector3.up * yOffset;
        transform.position = worldTargetPos;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
