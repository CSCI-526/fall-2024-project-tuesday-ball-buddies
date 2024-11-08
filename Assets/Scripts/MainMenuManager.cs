using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
