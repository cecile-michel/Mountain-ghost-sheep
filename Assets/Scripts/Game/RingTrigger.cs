using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTrigger : MonoBehaviour
{
    Vector3 m_Movement;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
     void OnTriggerEnter(Collider other){
        GameObject[] sheep = GameObject.FindGameObjectsWithTag("GhostSheep"); 
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float min = 10000.0f;
        GameObject thePlayer = players[0];

        if(other.transform.parent.gameObject.CompareTag("GhostSheep")){
            for(int i = 0; i < players.Length; i++){
                float dist = Vector3.Distance(sheep[0].transform.position, players[i].transform.position);
                if(dist < min){
                    min = dist;
                    thePlayer = players[i];
                }
            }
            Debug.Log(other.transform.parent.gameObject.name + " triggers.");
            thePlayer.GetComponent<Score>().addScore();
        }
        

    }
 
}
