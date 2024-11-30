using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    float elapsedTime;
    private bool isTimerRunning = false;

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public string getTime()
    {
        return timerText.text;
    }

    public void ToggleTimer(bool isPaused)
    {
        isTimerRunning = !isPaused;
    }
}
