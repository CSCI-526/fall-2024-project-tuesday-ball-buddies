using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class DamageEffect : MonoBehaviour
{
    public float flashDuration = 0.5f;  // Total duration of the flash effect
    private List<Image> effectImages = new List<Image>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                effectImages.Add(img);
            }
        }
        SetTransparent();  // Ensure it starts transparent
    }

    public void TriggerFlash()
    {
        StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        // Flash in
        float time = 0;
        while (time < flashDuration / 2)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, time / (flashDuration / 2));
            SetAlpha(alpha);
            yield return null;
        }

        // Flash out
        time = 0;
        while (time < flashDuration / 2)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / (flashDuration / 2));
            SetAlpha(alpha);
            yield return null;
        }

        SetTransparent();  // Ensure it ends transparent
    }

    private void SetAlpha(float alpha)
    {
        foreach (Image img in effectImages)
        {
            var color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }

    private void SetTransparent()
    {
        SetAlpha(0);
    }
}