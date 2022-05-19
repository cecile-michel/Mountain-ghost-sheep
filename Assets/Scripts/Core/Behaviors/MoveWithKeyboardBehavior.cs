using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Input Keys
public enum InputKeyboard{
    wasd = 0, 
    arrows = 1,
    ijkl = 2,
    bot = 3
}

public class MoveWithKeyboardBehavior : AgentBehaviour
{
    Vector3 m_Movement;
    public InputKeyboard inputKeyboard;
    public bool isPaused;
    public bool longPressed = false;
    public bool isRunning = false;
    public GameObject pauseMenuUI;
    public bool gameOver;

    // to change the color of the cellulo (doesn't work if I put it in another script idk why)
    public int player;
    private ChangeColor ChangeColor;

    // to assign the right keys for the players
    private NumberPlayers numberPlayers;

    private Vector3 initPosition;
    private Quaternion initRotation;
    
    void Start()
    {
        gameOver = false;
        isPaused = true;
        initPosition = this.gameObject.transform.position;
        initRotation = this.gameObject.transform.rotation;
        // initialise color of the cellulos
        if (player == 0) {
            agent.SetVisualEffect(0, ChangeColor.joueur1, 0);
            inputKeyboard = (InputKeyboard) NumberPlayers.player1;
        } else if (player == 1) {
            agent.SetVisualEffect(0, ChangeColor.joueur2, 0);
            inputKeyboard = (InputKeyboard) NumberPlayers.player2;

        } else {
            agent.SetVisualEffect(0, ChangeColor.joueur3, 0);
            inputKeyboard = (InputKeyboard) NumberPlayers.player3;

        }

    }

    public void restartGame()
    {
        this.gameObject.transform.SetPositionAndRotation(initPosition, initRotation);
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputKeyboard == InputKeyboard.wasd)
        {
            if (Input.GetKeyDown("space") && isPaused == false)
            {
                pause();

            }
            else if (Input.GetKeyDown("space") && isPaused == true)
            {
                unPause();
            }
        }
    }    


    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        
        float horizontal = 0;
        float vertical = 0;
        
        if(InputKeyboard.wasd == inputKeyboard){
           horizontal = Input.GetAxis ("Horizontal_wasd");
           vertical = Input.GetAxis ("Vertical_wasd");
           
        } else if (InputKeyboard.arrows == inputKeyboard) {
            horizontal = Input.GetAxis ("Horizontal_arrows");
            vertical = Input.GetAxis ("Vertical_arrows");
        } else if (InputKeyboard.ijkl == inputKeyboard) {
            horizontal = Input.GetAxis ("Horizontal_ijkl");
            vertical = Input.GetAxis ("Vertical_ijkl");
        }
        
        steering.linear = new Vector3(horizontal, 0, vertical)* agent.maxAccel; 
        steering.linear = this.transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));

        return steering;
    }

    public void pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        GameObject ghostSheep = GameObject.FindGameObjectWithTag("GhostSheep");
        ghostSheep.GetComponent<GhostSheepBehavior>().pause();
    }
    public void unPause()
    {
        if (!isRunning)
        {
            pauseMenuUI.transform.Find("StartButton").gameObject.GetComponent<Button>().onClick.Invoke();
        }
        else
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            GameObject ghostSheep = GameObject.FindGameObjectWithTag("GhostSheep");
            ghostSheep.GetComponent<GhostSheepBehavior>().unPause();
        }
    }

    // functions for step 1 milestone 2 (on ice and on stone)
    public void changeDrivability(bool b) {
        agent.SetCasualBackdriveAssistEnabled(b);
    }

    public void changeWalkOnTexture(int choice) {
        // if choice == 1 move on stone else move on ice
        if (choice == 1) {
            agent.MoveOnStone();
        } else {
            agent.MoveOnIce();
        }
    }

    public override void OnCelluloLongTouch(int key)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject otherPlayer = players[0];

        for (int i = 0; i < players.Length; i++)
        {
            if (this.gameObject.name != players[i].gameObject.name)
            {
                otherPlayer = players[i];
            }
        }

        MoveWithKeyboardBehavior otherBehavior = otherPlayer.GetComponent<MoveWithKeyboardBehavior>();
        if (otherBehavior.longPressed)
        {
            if (isPaused || otherBehavior.isPaused)
            {
                unPause();
                otherBehavior.unPause();
                longPressed = false;
                otherBehavior.longPressed = false;
            }
        }
        else if (isPaused || otherBehavior.isPaused)
        {
            longPressed = true;
        }
        base.OnCelluloLongTouch(key);
    }
}
