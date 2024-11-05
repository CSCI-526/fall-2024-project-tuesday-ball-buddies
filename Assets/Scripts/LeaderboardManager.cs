using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class LeaderboardManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerRecord
    {
        public string player;
        public string rank;
        public string star;
        public string time;
    }

    public GameObject leaderboardEntryPrefab; // Assign your prefab for each leaderboard entry
    public Transform leaderboardContainer;

    // Start is called before the first frame update
    public void PopulateBoard(string json)
    {
        string pattern = @"\{[^}]*\}";
        // Find all matches
        MatchCollection matches = Regex.Matches(json, pattern);

        // Process each match
        foreach (Match match in matches)
        {
            PlayerRecord playerRecord = JsonUtility.FromJson<PlayerRecord>(match.Value);

            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            entry.transform.Find("Rank").GetComponent<TMP_Text>().text = playerRecord.rank;
            entry.transform.Find("Player").GetComponent<TMP_Text>().text = playerRecord.player;
            entry.transform.Find("Star").GetComponent<TMP_Text>().text = playerRecord.star;
            entry.transform.Find("Time").GetComponent<TMP_Text>().text = playerRecord.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
