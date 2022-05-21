using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EatObject : MonoBehaviour
{

    public GameObject player;

    private GameObject[] ghosts;
    private GameObject pacman;
    private ChangeScene changeScene;

    // Start is called before the first frame update
    void Start()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        pacman = GameObject.FindGameObjectWithTag("Pacman");

    }
    
    void OnTriggerEnter(Collider other){
        print(other.transform.tag);
        if (player.CompareTag("Pacman")) {
            if (other.transform.CompareTag("Pac-gommes")) {
                other.transform.gameObject.SetActive(false);
                // increase the score
                pacman.GetComponent<Score>().addScore();
            } else if (other.transform.CompareTag("RedGems")) {
                other.transform.gameObject.SetActive(false);
                pacman.GetComponent<Score>().addScore(10);
                // TODO make the ghosts afraid
                
            } else if (other.transform.CompareTag("Star")) {
                other.transform.gameObject.SetActive(false);
                pacman.GetComponent<Score>().addScore(10);

                // make the ghost immobilized
                ghosts[0].GetComponent<MoveWithKeyboardBehavior>().immobilize();
                ghosts[1].GetComponent<MoveWithKeyboardBehavior>().immobilize();
            } else if (other.transform.CompareTag("Cherry")) {
                other.transform.gameObject.SetActive(false);
                pacman.GetComponent<Score>().addScore(10);
                // go to the next level
                ChangeScene.moveToScene(SceneManager.GetActiveScene().buildIndex+1);
                
            }
        } else if (player.CompareTag("Ghost")) {
            if (other.transform.CompareTag("GreenGem")) {
                other.transform.gameObject.SetActive(false);
                // immobiliser le pacman
                pacman.GetComponent<MoveWithKeyboardBehavior>().immobilize();
                pacman.GetComponent<Score>().removeScore(5);
            }
        }
    }

}
