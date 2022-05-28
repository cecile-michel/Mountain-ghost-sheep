using System.Linq;
using UnityEngine;

public class StopBeforeObstacleBehavior : AgentBehaviour
{
    private Steering steering;
    public override Steering GetSteering()
    {
        return steering;
    }
    protected override void OnObstacleInFront(float distance, int layer){
        agent.LockMovement();
    }
    protected override void NoObstacleInFront(){
        agent.UnlockMovement();
        steering = null;
    }
}