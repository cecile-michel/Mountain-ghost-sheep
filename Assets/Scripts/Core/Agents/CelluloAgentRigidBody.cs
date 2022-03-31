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
        if (steering.linear.sqrMagnitude == 0.0f)
            _rigidBody.velocity = Vector3.zero; 
        else
        {
            if(_rigidBody.velocity.sqrMagnitude<maxSpeed*maxSpeed)
                _rigidBody.AddForce(Vector3.ClampMagnitude(steering.linear, maxAccel));
        }

        base.FixedUpdate();

    }
    public override void LateUpdate()
    {

        rotation += steering.angular * Time.deltaTime;
        rotation = rotation > maxRotation ? maxRotation : rotation;

        rotation = steering.angular == 0.0f ? 0.0f : rotation;
        velocity = transform.parent.InverseTransformDirection(_rigidBody.velocity);

    }

    

}
