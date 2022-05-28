using UnityEngine;
using System.Collections.Generic;

/// <summary> 
/// This class represent the basic steering agent, it takes care of applying the total steering on the agent depending on the blending method (none, weight, priority)
/// </summary>

public class SteeringAgent : MonoBehaviour
{
    [Header ("Blend Behaviour Settings")]
    public bool blendWeight = false; //!< If more than one behavior is included and the desired output is a weighted sum of the different behaviors.
    public bool byPriority = false;
    public float maxSpeed  {get; protected set;} = 3f; //!< maximum speed in unity units
    public float maxAccel {get; protected set;}  = 15f; //!< maximum acceleration in unity units
    public float maxAngularSpeed {get; protected set;}  = 2f; //!< maximum angular speed in unity units
    public float maxAngularAccel {get; protected set;}  = 7.5f; //!< maximum angular accel in unity units

    protected float rotation;
    
    protected Vector3 velocity;
    protected Steering steering;
    private AgentBehaviour[] behaviours; 
    private SortedDictionary<int, List<AgentBehaviour>> groups;
    protected bool stopMoving; 

    protected virtual void Start()
    {
        velocity = Vector3.zero;
        steering = new Steering();
        behaviours = GetComponents<AgentBehaviour>();
        groups = new SortedDictionary<int, List<AgentBehaviour>>();

    }

    protected virtual void FixedUpdate(){
        // steering = new Steering();
        if(byPriority)
            SetSteering(behaviours);
        else{
            foreach(AgentBehaviour behaviour in behaviours){
                if(behaviour.isActiveAndEnabled)
                    if (blendWeight)
                        SetSteering(behaviour.GetSteering(), behaviour.weight);
                    else 
                        SetSteering(behaviour.GetSteering());
            }
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
        rotation = rotation > maxAngularSpeed ? maxAngularSpeed : rotation;
        
        rotation = steering.angular == 0.0f ? 0.0f : rotation;
        velocity = steering.linear.sqrMagnitude == 0.0f ? Vector3.zero : velocity;
    }
    public void SetSteering(Steering steering)
    {
        if(steering == null)
            steering = new Steering();
        this.steering = steering;
    }

    public void SetSteering(Steering steering, float weight)
    {
        this.steering.linear += (weight * steering.linear);
        this.steering.angular += (weight * steering.angular);
    }  
    public void SetSteering(AgentBehaviour[] behaviours)
    {
        groups.Clear();
        foreach(AgentBehaviour behaviour in behaviours){
            if(behaviour.isActiveAndEnabled){
                if (!groups.ContainsKey(behaviour.priority))
                {
                    groups.Add(behaviour.priority, new List<AgentBehaviour>());
                }
                groups[behaviour.priority].Add(behaviour);
            }
        }
        foreach(int group in groups.Keys){
            steering = new Steering();
            bool priorityBreak = false; 
            foreach (AgentBehaviour singleBehavior in groups[group])
            {   Steering localSteering = singleBehavior.GetSteering();
                if(localSteering!=null){
                    priorityBreak = true;
                    steering.linear += singleBehavior.weight*localSteering.linear;
                    steering.angular += singleBehavior.weight*localSteering.angular;
                }
            }
            if(priorityBreak) return;
        }   
    
    }   
    public Vector3 GetVelocity(){
        return velocity;
    }
    public Vector3 GetSteeringLinear(){
        return steering.linear;
    }
    public float GetSteeringAngular(){
        return steering.angular;
    }
    public void LockMovement(){
        stopMoving = true; 
    }
    public void UnlockMovement(){
        stopMoving = false;
    }
}

//!< Steering serves as a custom data type for storing the movement and rotation of the agent:
public class Steering
{
    public float angular;
    public Vector3 linear;
    public Steering()
    {
        angular = 0; 
        linear = Vector3.zero;
    }
}