using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanCollision : MonoBehaviour
{
    public PacmanGameManager pacmanGameManager;


    void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Ghost")) {
            if (collision.gameObject.GetComponent<MoveWithKeyboardBehavior>().isFeared())
            {
                GetComponent<Score>().addScore(15);
            }
            else
            {
                pacmanGameManager.endOfMaze();
            }
        }
    }
}
