using System.Linq;
using UnityEngine;
using System;

public class GhostSheepBehavior : AgentBehaviour
{    
    public GameObject[] players;

    // implementer que le sheep bouge dans les coins
    public void Start() {
        print("Test");
    }

    public override Steering GetSteering()
    {
        
        Steering steering = new Steering();
        //implement your code here.

        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) {
            return steering;
        }
        float minDist = 10000;
        int index = 0;
        for (int i = 0; i < players.Length; i++) {
            float dist = Vector3.Distance(agent.transform.position, players[i].transform.position);
            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }
        // print("Index horizontal: " + minDistIndexPlayerX);
        // print("Index vertical: " + minDistIndexPlayerZ);
        float x = linear_distance(agent.transform.position[0], players[index].transform.position[0]);
        float z = linear_distance(agent.transform.position[2], players[index].transform.position[2]);
        steering.linear = new Vector3(x, 0, z)* agent.maxAccel;
        steering.linear = this.transform.parent.TransformDirection (Vector3.ClampMagnitude(steering.linear , agent.maxAccel));
        return steering;
    }

    public float linear_distance(float x, float y) {
        return x - y;
    }


}