using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EatObject : MonoBehaviour
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
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
                // TODO make the pacman faster
            } else if (other.transform.CompareTag("Cherry")) {
                other.transform.gameObject.SetActive(false);
                //go to the next level
                GoToNextLevel.changeScene(SceneManager.GetActiveScene().buildIndex+1);
                
            }
        } else if (player.CompareTag("Ghost")) {
            if (other.transform.CompareTag("GreenGem")) {
                other.transform.gameObject.SetActive(false);
                // TODO immobiliser le pacman
            }
        }
    }

}
