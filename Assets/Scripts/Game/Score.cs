using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Score : MonoBehaviour {

    // keeps it for the ghost sheep game
    public int score;

    public static int pacmanScore = 0;

    public Text text;
    public enum Dog {
        dog1 = 0,
        dog2 = 1,
        null_ = 2,
        pacman = 3
    }
    public Dog dog;
    public AudioSource losePoint;
    public AudioSource winPoint;
    
    public void Start() {
        displayScore();
        if (dog == Dog.pacman) {
            score = pacmanScore;
        }
    }

    public void resetScore()
    {
        if (dog == Dog.pacman) {
            score = pacmanScore;
        } else {
            score = 0;
        }
        displayScore();
    }

    public void addScore(int amount = 1){
        score = score + amount;
        pacmanScore += amount;
        winPoint.Play();
        displayScore();
    }
    
    public int removeScore(int amount = 1){
        int diff = Mathf.Min(score, amount);
        if (diff > 0) {
            score = score - amount;
            pacmanScore -= amount;
            losePoint.Play();
        }
        displayScore();

        return diff;
    }

    public void displayScore() {
        if (dog == Dog.dog1) {
            text.text = "Score joueur 1: " + score;
        } else if (dog == Dog.dog2) {
            text.text = "Score joueur 2: " + score;
        } else if (dog == Dog.pacman) {
            text.text = "pacman: " + pacmanScore;
        }
    }
}
