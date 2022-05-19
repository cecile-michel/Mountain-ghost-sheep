using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatObject : MonoBehaviour
{

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        print("Player");
    }
    
    void OnTriggerEnter(Collider other){
        print(other.transform.tag);
        if (player.CompareTag("Pacman")) {
            if (other.transform.CompareTag("Pac-gommes")) {
                other.transform.gameObject.SetActive(false);
                player.GetComponent<Score>().addScore();
            } else if (other.transform.CompareTag("RedGems")) {
                other.transform.gameObject.SetActive(false);
                // TODO make the ghosts afraid
            } else if (other.transform.CompareTag("Star")) {
                other.transform.gameObject.SetActive(false);
                // TODO make the pacman faster
            } else if (other.transform.CompareTag("Cherry")) {
                other.transform.gameObject.SetActive(false);
                //TODO go to the next level
            }
        } else if (player.CompareTag("Ghost")) {
            if (other.transform.CompareTag("GreenGem")) {
                other.transform.gameObject.SetActive(false);
                // TODO immobiliser le pacman
            }
        }
    }

}
