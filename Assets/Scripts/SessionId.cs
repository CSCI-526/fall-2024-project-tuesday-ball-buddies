using UnityEngine;

public class SessionManager : MonoBehaviour
{
    // Static variable to hold the session ID
    public static string sessionID;

    void Awake()
    {
        // Check if the session ID is empty
        if (string.IsNullOrEmpty(sessionID))
        {
            sessionID = System.Guid.NewGuid().ToString();
            Debug.Log("Generated New Session ID: " + sessionID);
        }
        else
        {
            Debug.Log("Existing Session ID: " + sessionID);
        }

        // Make sure this object persists across scenes
        DontDestroyOnLoad(this.gameObject);
    }
}
