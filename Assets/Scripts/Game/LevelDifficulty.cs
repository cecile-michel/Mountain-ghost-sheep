using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDifficulty : MonoBehaviour
{
    public static int difficulty = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
        easy => 0
        medium => 1
        hard => 2
    */
    public void setDifficulty(int number) {
        difficulty = number;
    }
}
