using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PacmanGameManager : MonoBehaviour
{
    public GameObject endMenu;
    public GameObject pauseMenu;
    public GameObject timer;

    public Text textFin;

    private Score score;
    // Start is called before the first frame update
    void Start()
    {
        endMenu.SetActive(false);
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
        }
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<MoveWithKeyboardBehavior>().isRunning = false;
            ghost.GetComponent<MoveWithKeyboardBehavior>().gameOver = true;
            ghost.GetComponent<MoveWithKeyboardBehavior>().pause();
        }
        
        textFin.text = "Pacman a eu " + Score.pacmanScore + " points";
        endMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
}
