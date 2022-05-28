using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithKeyboardBehaviorDemo : AgentBehaviour
{
    Steering steering = new Steering();
    bool collisionBehavior;

    public override Steering GetSteering()
    {
        if(!collisionBehavior){
            Vector3 move = Vector3.zero;
            move =
                    Vector2.up * (Input.GetAxis("Vertical")) +
                    Vector2.right * (Input.GetAxis("Horizontal"));
                    
            
            steering.linear = new Vector3(move.x, 0, move.y)* agent.maxAccel;
            steering.linear = this.transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));

            Vector3 rotate = Vector3.up * ((Input.GetKey(KeyCode.Q) ? 1 : 0) - (Input.GetKey(KeyCode.E) ? 1 : 0));
            steering.angular = -agent.maxAngularAccel * rotate.y;

            if(Input.GetKeyDown(KeyCode.Z)){
                agent.SetGoalPosition(6,-5,5);
            }
            if(Input.GetKeyDown(KeyCode.X)){
                agent.SetGoalPose(6,-5,0,5,5);
            }
        }
        return steering;
    }

    void OnCollisionEnter(Collision collision)
    {
        agent.ActivateDirectionalHapticFeedback();
        foreach (ContactPoint contact in collision.contacts)
        steering.linear = collision.contacts[0].normal.normalized*agent.maxAccel;
        collisionBehavior = true; 
    }

    void OnCollisionExit(Collision collision){
        agent.DeActivateDirectionalHapticFeedback();
        collisionBehavior = false; 
    }
}
