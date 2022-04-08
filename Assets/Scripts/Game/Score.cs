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
    
    public void Start() {
        displayScore();
    }

    public void addScore(){
        score = score + 1;
        displayScore();
    }
    
    public void removeScore(){
        score = score - 1;
        displayScore();

    }

    public void displayScore() {
        if (dog == 0) {
            text.text = "Score joueur 1: " + score;
        } else {
            text.text = "Score joueur 2: " + score;
        }
    }
}
