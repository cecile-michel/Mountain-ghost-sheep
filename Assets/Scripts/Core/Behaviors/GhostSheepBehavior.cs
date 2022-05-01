using System.Linq;
using UnityEngine;

public class GhostSheepBehavior : AgentBehaviour
{    
    public GameObject[] players;
    private int state = 0;
    private bool isPaused = false;
    // private AudioSource[] audios;
    public AudioSource transformIntoLamb;
    public AudioSource transformIntoWolf;

    private Vector3 initPosition;
    private Quaternion initRotation;

    // implementer que le sheep bouge dans les coins
    public void Start() {
        initPosition = this.gameObject.transform.position;
        initRotation = this.gameObject.transform.rotation;

        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.green, 255);
    }

    public void restartGame()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        this.gameObject.transform.SetPositionAndRotation(initPosition, initRotation);

        CancelInvoke();
        BecomeSheep();

        Invoke("BecomeGhost", Random.Range(15, 25));
    }

    public override Steering GetSteering()
    {
        
        Steering steering = new Steering();
        //implement your code here.

        // players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0 || isPaused) {
            return steering;
        }
        float minDist = 10000;
        int index = 0;
        for (int i = 0; i < players.Length; i++) {
            float dist = Vector3.Distance(agent.transform.position, players[i].transform.position);
            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }
        // print("Index horizontal: " + minDistIndexPlayerX);
        // print("Index vertical: " + minDistIndexPlayerZ);
        float x = linear_distance(agent.transform.position[0], players[index].transform.position[0]);
        float z = linear_distance(agent.transform.position[2], players[index].transform.position[2]);
        if (state == 1)
        {
            x = -x;
            z = -z;
        }
        steering.linear = new Vector3(x, 0, z)* agent.maxAccel;
        steering.linear = this.transform.parent.TransformDirection (Vector3.ClampMagnitude(steering.linear , agent.maxAccel));
        return steering;
    }

    public float linear_distance(float x, float y) {
        return x - y;
    }

    private void BecomeGhost()
    {
        state = 1;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.red, 255);
        players[0].GetComponent<MoveWithKeyboardBehavior>().changeDrivability(false);
        players[0].GetComponent<MoveWithKeyboardBehavior>().changeWalkOnTexture(1);
        players[1].GetComponent<MoveWithKeyboardBehavior>().changeDrivability(false);
        players[1].GetComponent<MoveWithKeyboardBehavior>().changeWalkOnTexture(1);
        transformIntoWolf.Play();
        Invoke("BecomeSheep", Random.Range(10, 20));
    }

    private void BecomeSheep()
    {
        state = 0;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.green, 255);
        players[0].GetComponent<MoveWithKeyboardBehavior>().changeDrivability(true);
        players[0].GetComponent<MoveWithKeyboardBehavior>().changeWalkOnTexture(0);
        players[1].GetComponent<MoveWithKeyboardBehavior>().changeDrivability(true);
        players[1].GetComponent<MoveWithKeyboardBehavior>().changeWalkOnTexture(0);
        transformIntoLamb.Play();
        Invoke("BecomeGhost", Random.Range(15, 25));
    }

    public int getState() {
        return state;
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