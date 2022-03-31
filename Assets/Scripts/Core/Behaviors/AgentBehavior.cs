using UnityEngine;
using System.Collections;
public class AgentBehaviour : MonoBehaviour
{
    public float weight = 1.0f;
    protected CelluloAgent agent;

    public virtual void Awake()
    {
        agent = gameObject.GetComponent<CelluloAgent>();
    }
    public virtual void FixedUpdate()
    {
        if (agent.blendWeight)
            agent.SetSteering(GetSteering(), weight);
        else
            agent.SetSteering(GetSteering());
    }
    public virtual Steering GetSteering()
    {
        return new Steering();
    }
}
