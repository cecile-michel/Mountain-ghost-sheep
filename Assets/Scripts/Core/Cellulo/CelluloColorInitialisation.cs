using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelluloColorInitialisation : AgentBehaviour
{

    public Color color = new Color();
    public int player;
    public ChangeColor ChangeColor;
    // Start is called before the first frame update
    void Start()
    {
        if (player == 0) {
            // colorPlayer = ChangeColor.joueur1;
            agent.SetVisualEffect(0, ChangeColor.joueur1, 0);
        } else {
            color = ChangeColor.joueur2;
            agent.SetVisualEffect(0, ChangeColor.joueur2, 0);
        }
        
    }

}
