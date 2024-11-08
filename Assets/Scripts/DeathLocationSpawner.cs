using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class DeathLocationSpawner : MonoBehaviour
{
    [SerializeField] private GameObject redMarkerPrefab;
    [SerializeField] private GameObject blueMarkerPrefab;
    [SerializeField] private string jsonUrl = "https://get-death-cause-814677926917.us-central1.run.app";

    private void OnEnable()
    {
        // Register event listener when the object is enabled
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unregister event listener when the object is disabled
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FetchAndSpawnDeathLocations());
    }

    private IEnumerator FetchAndSpawnDeathLocations()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(jsonUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching JSON: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log("Fetched JSON: " + json);
                ParseAndSpawnDeathLocations(json);
            }
        }
    }

private void ParseAndSpawnDeathLocations(string json)
{
    // Wrap the JSON data in a root object to parse with JsonUtility
    json = "{\"entries\":" + json + "}";

    // Parse JSON into the DeathLocationRoot structure
    DeathLocationRoot deathLocationRoot = JsonUtility.FromJson<DeathLocationRoot>(json);
    if (deathLocationRoot != null && deathLocationRoot.entries != null)
    {
        // Loop through each entry
        foreach (var entry in deathLocationRoot.entries)
        {
            foreach (var location in entry.death_locations)
            {
                Vector3 localPosition = new Vector3(float.Parse(location.x), float.Parse(location.y), float.Parse(location.z));
                GameObject prefabToSpawn = location.reason == "falling" ? redMarkerPrefab : blueMarkerPrefab;

                if (!string.IsNullOrEmpty(location.scene) && !string.IsNullOrEmpty(location.level))
                {
                    if (SceneManager.GetActiveScene().name == location.scene)
                    {
                        // Find the level GameObject in the scene
                        GameObject parentObject = GameObject.Find(location.level);
                        
                        if (parentObject != null)
                        {
                            // Instantiate the marker as a child of the parentObject
                            GameObject instance = Instantiate(prefabToSpawn, parentObject.transform);
                            instance.name = $"Marker_{location.reason}_{location.timestamp}";
                            
                            // Set localPosition relative to the level parent object
                            instance.transform.localPosition = localPosition;
                        }
                        else
                        {
                            Debug.LogWarning($"Level '{location.level}' not found for marker at {localPosition}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Scene '{location.scene}' does not match the active scene '{SceneManager.GetActiveScene().name}'");
                    }
                }
            }
        }
    }
    else
    {
        Debug.LogError("Failed to parse JSON.");
    }
}
}

// Root class to contain the list of death location entries
[System.Serializable]
public class DeathLocationRoot
{
    public List<DeathLocationEntry> entries;
}

[System.Serializable]
public class DeathLocationEntry
{
    public string uuid;
    public List<DeathLocation> death_locations;
}

[System.Serializable]
public class DeathLocation
{
    public string x;
    public string y;
    public string z;
    public string scene;
    public string level;
    public string enemyname;
    public string reason;
    public string timestamp;
}
