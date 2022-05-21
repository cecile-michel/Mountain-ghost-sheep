using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanCollision : MonoBehaviour
{
    public PacmanGameManager pacmanGameManager;


    void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Ghost")) {
            
            pacmanGameManager.endOfMaze();        
        }
    }
}
