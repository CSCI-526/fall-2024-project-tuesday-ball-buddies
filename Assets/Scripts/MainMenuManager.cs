using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject leaderboard;
    public TMP_Text title;
    private FirestoreApiManager firestoreApiManager;

    // Start is called before the first frame update
    void Start()
    {
        firestoreApiManager = FindObjectOfType<FirestoreApiManager>();
    }

    public void BackToMenu(){
        gameObject.SetActive(true);
        leaderboard.SetActive(false);
    }

    public void OpenLeaderboard(string world){
        gameObject.SetActive(false);
        leaderboard.SetActive(true);
        title.text = world;
        firestoreApiManager.GetLeaderboardWrap(world, SessionManager.sessionID);
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenWorld1()
    {
        SceneManager.LoadScene("World1");
    }

    public void OpenWorld2()
    {
        SceneManager.LoadScene("World2");
    }
}
