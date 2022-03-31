using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public int score;
    
    public void addScore(){
        score = score + 1;
    }
    
    public void removeScore(){
        score = score - 1;

    }
}
