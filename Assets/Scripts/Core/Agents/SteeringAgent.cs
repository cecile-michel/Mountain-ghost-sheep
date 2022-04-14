using UnityEngine;
using System.Collections.Generic;

/// <summary> 
/// This class represent the basic steering agent, it takes care 
/// </summary>

public class SteeringAgent : MonoBehaviour
{
    public bool blendWeight = false; //!< If more than one behavior is included and the desired output is a weighted sum of the different behaviors.

    public float maxSpeed  {get; protected set;} = 3f; //!< maximum speed in unity units
    public float maxAccel {get; protected set;}  = 15f; //!< maximum acceleration in unity units
    public float maxRotation {get; protected set;}  = 360f; //!< maximum rotation in unity units
    public float maxAngularAccel {get; protected set;}  = 7.5f; //!< maximum angular accel in unity units

    protected float rotation;
    protected Vector3 velocity;
    protected Steering steering;
    private AgentBehaviour[] behaviours; 

    protected virtual void Start()
    {
        velocity = Vector3.zero;
        steering = new Steering();
        behaviours = GetComponents<AgentBehaviour>();

    }

    protected virtual void FixedUpdate(){
        steering = new Steering();
        foreach(AgentBehaviour behaviour in behaviours){
            if (blendWeight)
                SetSteering(behaviour.GetSteering(), behaviour.weight);
            else
                SetSteering(behaviour.GetSteering());
        }
    }

    /// <summary> 
    /// Basic pointlike model velocity update. 
    /// </summary>
    public virtual void LateUpdate()
    {
        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        rotation = rotation > maxRotation ? maxRotation : rotation;
        
        rotation = steering.angular == 0.0f ? 0.0f : rotation;
        velocity = steering.linear.sqrMagnitude == 0.0f ? Vector3.zero : velocity;
    }
    public void SetSteering(Steering steering)
    {
        this.steering = steering;
    }

    public void SetSteering(Steering steering, float weight)
    {
        this.steering.linear += (weight * steering.linear);
        this.steering.angular += (weight * steering.angular);
    }   
}

//!< Steering serves as a custom data type for storing the movement and rotation of the agent:
public class Steering
{
    public float angular;
    public Vector3 linear;
    public Steering()
    {
        angular = 0.0f;
        linear = Vector3.zero;
    }
}