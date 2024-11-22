using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerRecord
    {
        public string player;
        public string rank;
        public int star;
        public string time;
    }

    public GameObject leaderboardEntryPrefab; // Assign your prefab for each leaderboard entry
    public Transform leaderboardContainer;

    // Start is called before the first frame update
    public void PopulateBoard(string json)
    {
        // hello
        
        leaderboardContainer.Find("StatusText").gameObject.SetActive(false);
        leaderboardContainer.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperCenter;

        string pattern = @"\{[^}]*\}";
        // Find all matches
        MatchCollection matches = Regex.Matches(json, pattern);

        ClearGrid();

        GameObject headerRow = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
        headerRow.transform.Find("Rank").GetComponent<TMP_Text>().text = "Rank";
        headerRow.transform.Find("Player").GetComponent<TMP_Text>().text = "Player";
        headerRow.transform.Find("Star").GetComponent<TMP_Text>().text = "Star";
        headerRow.transform.Find("Time").GetComponent<TMP_Text>().text = "Time";

        // Process each match
        foreach (Match match in matches)
        {
            PlayerRecord playerRecord = JsonUtility.FromJson<PlayerRecord>(match.Value);

            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            entry.transform.Find("Rank").GetComponent<TMP_Text>().text = playerRecord.rank;
            entry.transform.Find("Player").GetComponent<TMP_Text>().text = playerRecord.player;
            entry.transform.Find("Star").GetComponent<TMP_Text>().text = $"{playerRecord.star}";
            entry.transform.Find("Time").GetComponent<TMP_Text>().text = playerRecord.time;
        }
    }

    public void ClearGrid()
    {
        // Loop through all child objects of the GridLayoutGroup.
        foreach (Transform child in leaderboardContainer.gameObject.transform)
        {
            if (child.gameObject.name == "StatusText")
                continue;

            Destroy(child.gameObject); // Destroy each child GameObject.
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
