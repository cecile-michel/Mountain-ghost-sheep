using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    void OnCollisionEnter(Collision col) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject thePlayer = players[0];
        print(col.gameObject.name);
        print(players[0].gameObject.name);
        
        for(int i = 0; i < players.Length; i ++){
            if(col.gameObject.name == players[i].gameObject.name && this.GetComponent<GhostSheepBehavior>().getState() == 1){
                thePlayer = players[i];
                thePlayer.GetComponent<Score>().removeScore();
            }
        }

    }
}
