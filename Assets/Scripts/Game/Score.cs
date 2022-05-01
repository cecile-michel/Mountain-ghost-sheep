using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Score : MonoBehaviour {

    public int score;
    public Text text;
    public enum Dog {
        dog1 = 0,
        dog2 = 1,
        null_ = 2
    }
    public Dog dog;
    public AudioSource losePoint;
    public AudioSource winPoint;
    
    public void Start() {
        displayScore();
    }

    public void resetScore()
    {
        score = 0;
        displayScore();
    }

    public void addScore(int amount = 1){
        score = score + amount;
        winPoint.Play();
        displayScore();
    }
    
    public int removeScore(int amount = 1){
        int diff = Mathf.Min(score, amount);
        if (diff > 0) {
            score = score - amount;
            losePoint.Play();
        }
        displayScore();

        return diff;
    }

    public void displayScore() {
        if (dog == 0) {
            text.text = "Score joueur 1: " + score;
        } else {
            text.text = "Score joueur 2: " + score;
        }
    }
}
