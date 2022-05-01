using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int numRemoved = 0;
        for (int i = 0; i < activePowerUps.Count; i++)
        {
            if (activePowerUps[i-numRemoved].Item2 < Time.time)
            {
                activePowerUps.RemoveAt(i-numRemoved);
                numRemoved++;
            }
        }
    }

    public Dictionary<string, float> POWERUP_DURATION = new Dictionary<string, float>{
        { "steal", 10 },
    };
    private List<Tuple<string, float>> activePowerUps = new List<Tuple<string, float>>();


    void OnCollisionEnter(Collision col)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject otherPlayer = players[0];

        for (int i = 0; i < players.Length; i++)
        {
            if (col.gameObject.name == players[i].gameObject.name)
            {
                otherPlayer = players[i];
                int numRemoved = 0;
                for (int p = 0; p < activePowerUps.Count; p++)
                {
                    switch (activePowerUps[p-numRemoved].Item1)
                    {
                        case "steal":
                            int amount = otherPlayer.GetComponent<Score>().removeScore(2);
                            this.GetComponent<Score>().addScore(amount);
                            activePowerUps.RemoveAt(p);
                            numRemoved++;
                            break;
                    }
                }
            }
        }

    }

    public void clearPowerUps()
    {
        activePowerUps = new List<Tuple<string, float>>();
    }

    public void addPowerUp(string powerUpName)
    {
        if (POWERUP_DURATION.TryGetValue(powerUpName, out float value))
        {
            activePowerUps.Add(new Tuple<string, float>(powerUpName, Time.time + value));
        }
    }
}
