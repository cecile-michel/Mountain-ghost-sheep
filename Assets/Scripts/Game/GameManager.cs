using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject endMenu;
    public GameObject player1Text;
    public GameObject player2Text;
    public GameObject bothText;

    // Start is called before the first frame update
    void Start()
    {
        endMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
                
    }

    public void endGame()
    {
        int winner = -1;
        int maxScore = int.MinValue;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
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
