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
                // TODO increase the score
            } else if (other.transform.CompareTag("RedGems")) {
                other.transform.gameObject.SetActive(false);
                // TODO make the ghosts afraid
            } else if (other.transform.CompareTag("Star")) {
                other.transform.gameObject.SetActive(false);
                // make the ghost immobilized
                ghosts[0].GetComponent<MoveWithKeyboardBehavior>().immobilize();
                ghosts[1].GetComponent<MoveWithKeyboardBehavior>().immobilize();
            } else if (other.transform.CompareTag("Cherry")) {
                other.transform.gameObject.SetActive(false);
                //go to the next level
                GoToNextLevel.changeScene(SceneManager.GetActiveScene().buildIndex+1);
                
            }
        } else if (player.CompareTag("Ghost")) {
            if (other.transform.CompareTag("GreenGem")) {
                other.transform.gameObject.SetActive(false);
                // immobiliser le pacman
                pacman.GetComponent<MoveWithKeyboardBehavior>().immobilize();
            }
        }
    }

}
