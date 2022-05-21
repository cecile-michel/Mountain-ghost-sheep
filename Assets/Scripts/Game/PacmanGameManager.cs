using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PacmanGameManager : MonoBehaviour
{
    public GameObject endMenu;
    public GameObject pauseMenu;
    public GameObject player1Text;
    public GameObject player2Text;
    public GameObject bothText;
    public GameObject timer;
    // Start is called before the first frame update
    void Start()
    {
        // endMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    public void startGame() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Pacman"))
        {
            print(player);
            player.GetComponent<MoveWithKeyboardBehavior>().restartGame();
            player.GetComponent<Score>().resetScore();
        }

        foreach (GameObject ghost in GameObject.FindGameObjectsWithTag("Ghost")) {
            print(ghost);
            ghost.GetComponent<MoveWithKeyboardBehavior>().restartGame();
        }
        
        
        timer.GetComponent<Timer>().restart();
    }

    public void preStartGame()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Pacman"))
        {
            player.GetComponent<MoveWithKeyboardBehavior>().gameOver = false;
        }

        foreach (GameObject ghost in GameObject.FindGameObjectsWithTag("Ghost")) {
            ghost.GetComponent<MoveWithKeyboardBehavior>().gameOver = false;
        }
    }

    public void endGame()
    {
        int winner = -1;
        int maxScore = int.MinValue;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Pacman");
        foreach (GameObject player in players)
        {
            player.GetComponent<MoveWithKeyboardBehavior>().isRunning = false;
            player.GetComponent<MoveWithKeyboardBehavior>().gameOver = true;
            player.GetComponent<MoveWithKeyboardBehavior>().pause();
            if (player.GetComponent<Score>().score > maxScore)
            {
                maxScore = player.GetComponent<Score>().score;
                winner = (int)player.GetComponent<Score>().dog;
            } else if (player.GetComponent<Score>().score == maxScore)
            {
                winner = -1;
            }
        }

        GameObject ghostSheep = GameObject.FindGameObjectWithTag("GhostSheep");
        ghostSheep.GetComponent<GhostSheepBehavior>().pause();

        endMenu.SetActive(true);
        pauseMenu.SetActive(false);
        if (winner == 0)
        {
            player1Text.SetActive(true);
            player2Text.SetActive(false);
            bothText.SetActive(false);
        }
        else if (winner == 1)
        {
            player2Text.SetActive(true);
            player1Text.SetActive(false);
            bothText.SetActive(false);
        }
        else if (winner == -1)
        {
            bothText.SetActive(true);
            player1Text.SetActive(false);
            player2Text.SetActive(false);
        }
    }
}
