using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TMP_Text player1HUD;
    public TMP_Text player2HUD;
    public BallControl ballControl;
    
    private bool gameWon = false;

    private void Update()
    {
        if (!gameWon)
        {
            UpdatePlayer1HUD();
            UpdatePlayer2HUD();
        }
    }

    private void UpdatePlayer1HUD()
    {
        string controls = ballControl.onBridge ? "[ WASD ]  Move" : "[ Space ]  Jump";
        Color ballColor = ballControl.GetComponent<Renderer>().material.color;
        string colorHex = ColorUtility.ToHtmlStringRGB(ballColor);
        player1HUD.text = $"<color=#{colorHex}>Player 1\n<size=60%>{controls}</size></color>";
    }

    private void UpdatePlayer2HUD()
    {
        string controls = !ballControl.onBridge ? "[ Arrows ]  Tilt" : "[ J ]  Stop Shrinking";
        
        string colorTag = ballControl.onBridge ? "<color=#FF00FF>" : "<color=#66CC66>";
        // string colorTag = ballControl.onBridge ? "<color=white>" : "<color=#66CC66>";
        
        player2HUD.text = $"{colorTag}Player 2\n<size=60%>{controls}</size></color>";
    }

    public void ShowWinMessage()
    {
        gameWon = true;
        player1HUD.text = "<color=#FFD700>You Won!</color>";
        player2HUD.text = "<color=#FFD700>You Won!</color>";
    }
}
