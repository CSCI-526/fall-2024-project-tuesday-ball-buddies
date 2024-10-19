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

        string colorTag = !ballControl.onBridge ? "<color=#FFFFFF>" : "<color=#66CC66>";

        string fontSize = ballControl.onBridge ? "130%" : "80%";
        player1HUD.text = $"{colorTag}<size={fontSize}>Player 1</size>\n<size=70%>{controls}</size></color>";
    }

    private void UpdatePlayer2HUD()
    {
        string controls = !ballControl.onBridge ? "[ Arrows ]  Tilt" : "- DISABLED -";
        
        string colorTag = ballControl.onBridge ? "<color=#FFFFFF>" : "<color=#66CC66>";
        
        string fontSize = !ballControl.onBridge ? "130%" : "80%";
        player2HUD.text = $"{colorTag}<size={fontSize}>Player 2</size>\n<size=70%>{controls}</size></color>";
    }

    public void ShowWinMessage()
    {
        gameWon = true;
        player1HUD.text = "<color=#FFD700>You Won!</color>";
        player2HUD.text = "<color=#FFD700>You Won!</color>";
    }
}
