using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;
public class CurrentPlayerRankManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerRecord
    {
        public string player;
        public string rank;
        public int star;
        public string time;
    }

    public GameObject row;
    public GameObject fetchingText;

    // Start is called before the first frame update

    public void PopulateRecord(string json)
    {
        fetchingText.SetActive(false);
        row.SetActive(true);

        string pattern = @"\{[^}]*\}";
        // Find all matches
        MatchCollection matches = Regex.Matches(json, pattern);

        // Process each match
        foreach (Match match in matches)
        {
            PlayerRecord playerRecord = JsonUtility.FromJson<PlayerRecord>(match.Value);

            row.transform.Find("Rank").GetComponent<TMP_Text>().text = playerRecord.rank;
            row.transform.Find("Player").GetComponent<TMP_Text>().text = playerRecord.player;
            row.transform.Find("Star").GetComponent<TMP_Text>().text = $"{playerRecord.star}";
            row.transform.Find("Time").GetComponent<TMP_Text>().text = playerRecord.time;
        }
    }

}
