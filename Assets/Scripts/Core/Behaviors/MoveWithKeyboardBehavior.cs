using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Input Keys
public enum InputKeyboard{
    arrows = 0, 
    wasd = 1
}

public class MoveWithKeyboardBehavior : AgentBehaviour
{
    Vector3 m_Movement;
    public InputKeyboard inputKeyboard;
    private bool isPaused = false;

    public override Steering GetSteering()
    {
        Steering steering = new Steering();

        if (isPaused)
        {
            return steering;
        }
        float horizontal = 0;
        float vertical = 0;
        
       if(InputKeyboard.wasd == inputKeyboard){
           horizontal = Input.GetAxis ("Horizontal_wasd");
           vertical = Input.GetAxis ("Vertical_wasd");
           
        } else if (InputKeyboard.arrows == inputKeyboard) {
            horizontal = Input.GetAxis ("Horizontal_arrows");
            vertical = Input.GetAxis ("Vertical_arrows");
        }
        
        steering.linear = new Vector3(horizontal, 0, vertical)* agent.maxAccel; 
        steering.linear = this.transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));

        return steering;
    }

    public void pause()
    {
        isPaused = true;
    }
    public void unPause()
    {
        isPaused = false;
    }
}
