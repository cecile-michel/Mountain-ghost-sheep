using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class adds rigid body to a cellulo and manages the consequences of this: steering vector is now a force input on the rigid body, the velocities are then updated correspondingly. 
/// </summary>
public class CelluloAgentRigidBody : CelluloAgent
{
    private Rigidbody _rigidBody;

    protected override void Awake()

    {
        base.Awake();
        _rigidBody = GetComponent<Rigidbody>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(stopMoving){
            _rigidBody.velocity = Vector3.zero;
            return;
        }

        //Linear  
        if (steering.linear.sqrMagnitude == 0.0f)
            _rigidBody.velocity = Vector3.zero; 
        else
        {   //apply force only if we are not at the maximum speed
            if(_rigidBody.velocity.sqrMagnitude<maxSpeed*maxSpeed)
                _rigidBody.AddForce(Vector3.ClampMagnitude(steering.linear, maxAccel));
            else //else break a bit
            {
                Vector3 normalizedLinearVelocity = _rigidBody.velocity.normalized;
                Vector3 brake = normalizedLinearVelocity * maxAccel;  // make the brake Vector3 value
                _rigidBody.AddForce(-brake);  // apply opposing brake force
            }
        }

        //Angular 
        if(steering.angular == 0.0f)
            _rigidBody.angularVelocity = Vector3.zero;
        else{

            if(_rigidBody.angularVelocity.y<maxAngularSpeed){
                _rigidBody.AddTorque(Vector3.ClampMagnitude(steering.angular*Vector3.up, maxAngularAccel));
            }
            else{
                Vector3 normalizedAngularVel = _rigidBody.angularVelocity.normalized;
                Vector3 brake = normalizedAngularVel*maxAngularAccel;
                _rigidBody.AddTorque(-brake);
            }
        }
        

    }
    public override void LateUpdate()
    {
        rotation = _rigidBody.angularVelocity.y;
        velocity = transform.parent.InverseTransformDirection(_rigidBody.velocity);
    }

}
