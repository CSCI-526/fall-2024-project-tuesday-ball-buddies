using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private StageTimeManager stageTimeManager;
    private GameEndManager gameEndManager;
    private CanvasManager canvasManager;
    private Timer timer;
    private bool isPaused = false;

    void Start()
    {
        stageTimeManager = FindObjectOfType<StageTimeManager>();
        gameEndManager = FindObjectOfType<GameEndManager>();
        canvasManager = FindObjectOfType<CanvasManager>();
        timer = FindObjectOfType<Timer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }

    public void TogglePause()
    {
        if (gameEndManager.IsGameEnded())
            return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        timer.ToggleTimer(isPaused);
        canvasManager.PausePanelTogglePause(isPaused);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        StarControl.starCount = 0; //clear starCount back to 0
        stageTimeManager.ResetTimestamp();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        StarControl.starCount = 0;
        SceneManager.LoadScene("Main Menu");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}