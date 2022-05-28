using UnityEngine;
using System.Collections;
[RequireComponent(typeof(SteeringAgent))]
public class AgentBehaviour : MonoBehaviour
{
    public float weight = 1.0f;
    public int priority = 1;

    // [Header("RayCast")]
    public bool enableRaycast;
    [ConditionalHide("enableRaycast",true)] 
    public float distanceRaycast = 5;
    [ConditionalHide("enableRaycast",true)] 
    public LayerMask layerMask = 1;
    protected CelluloAgent agent;

    public virtual void Awake()
    {
        agent = gameObject.GetComponent<CelluloAgent>();
        if(agent==null){
            Debug.LogWarning("An active CelluloAgent should be attached to the same gameobject.");
        }
    }
    protected virtual void FixedUpdate(){
        if(enableRaycast){
            RaycastHit hit;
            NoObstacleInFront();
            Vector3 dir = agent.GetSteeringLinear().normalized; 
            Vector3 [] offset; 
            if(dir.x !=0)
                offset = new Vector3[3]{Vector3.zero, new Vector3(-dir.z/dir.x,0,1).normalized,-new Vector3(-dir.z/dir.x,0,1).normalized};
            else 
                offset = new Vector3[3]{Vector3.zero, new Vector3(1,0,dir.x/dir.z).normalized,-new Vector3(1,0,dir.x/dir.z).normalized};
            
            for (int i = 0; i<3 ; i++){
                Vector3 center = transform.position - offset[i] * agent.transform.localScale.magnitude/2;
                if (Physics.Raycast(center, dir, out hit, distanceRaycast, layerMask))
                {
                    Debug.DrawRay(center,dir * hit.distance, Color.red);
                    OnObstacleInFront(hit.distance,hit.transform.gameObject.layer);
                }
                else{ 
                    Debug.DrawRay(center,dir*distanceRaycast, Color.red);
                }
            }  
        } 
    }

    /// <summary>
    /// Function called when an obstacle is detected infront of the agent 
    /// </summary>
    /// <param name="distance">
    /// the distance between the agent and the obstacle
    /// </param>
    /// <param name="layer">
    /// the layer to whcih the obstacle belongs
    /// </param>
    protected virtual void OnObstacleInFront(float distance, int layer){ }

    /// <summary>
    /// Function called when no obstacle is detected infront of the agent 
    /// </summary>    
    protected virtual void NoObstacleInFront(){ }

    public virtual Steering GetSteering()
    {
        return new Steering();
    }

    /// <summary>
    /// Virtual function, to be implemented if OnTouchBegan from Real robot is needed
    /// </summary>
    /// <param name="key">
    /// the number of the key touch sensor 
    /// </param>
    public virtual void OnCelluloTouchBegan(int key){
    }
    /// <summary>
    /// Virtual function, to be implemented if OnTouchReleased from Real robot is needed
    /// </summary>
    /// <param name="key">
    /// the number of the key touch sensor 
    /// </param>
    public virtual void OnCelluloTouchReleased(int key){
    }
        /// <summary>
    /// Virtual function, to be implemented if OnLongTouch from Real robot is needed
    /// </summary>
    /// <param name="key">
    /// the number of the key touch sensor 
    /// </param>
    public virtual void OnCelluloLongTouch(int key){}
    
    /// <summary>
    /// Event signaled when robot kidnapping ends 
    /// </summary>
    public virtual void OnCelluloUnKidnapped(){}
    /// <summary>
    /// Event signaled when robot is kidnapped
    /// </summary>
    public virtual void OnCelluloKidnapped(){}

}
