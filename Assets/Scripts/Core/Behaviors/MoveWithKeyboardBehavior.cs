﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Input Keys
public enum InputKeyboard{
    arrows = 0, 
    wasd = 1
}

public class MoveWithKeyboardBehavior : AgentBehaviour
{
    Vector3 m_Movement;
    public InputKeyboard inputKeyboard;
    public bool isPaused;
    public GameObject pauseMenuUI;
    
    void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space") && isPaused == false){
            pause();
        
        } else if(Input.GetKeyDown("space") && isPaused == true){
            unPause();
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
           Time.timeScale = 1f;
           
        } else if (InputKeyboard.arrows == inputKeyboard) {
            horizontal = Input.GetAxis ("Horizontal_arrows");
            vertical = Input.GetAxis ("Vertical_arrows");
            Time.timeScale = 1f;
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

    }
    public void unPause()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

    }
}
