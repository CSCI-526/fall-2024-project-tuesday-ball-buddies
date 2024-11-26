using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Timer timer;
    private bool isPaused = false;
    private StageTimeManager stageTimeManager;
    private GameEndManager gameEndManager;

    void Start()
    {
        stageTimeManager = FindObjectOfType<StageTimeManager>();
        gameEndManager = FindObjectOfType<GameEndManager>();

        timer = FindObjectOfType<Timer>();
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameEndManager.GetIfGameEnded())
                return ;

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        if (timer != null)
            timer.PauseTimer();
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (timer != null)
            timer.ResumeTimer();
        if (pausePanel != null)
            pausePanel.SetActive(false);
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
        SceneManager.LoadScene("Main Menu");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}