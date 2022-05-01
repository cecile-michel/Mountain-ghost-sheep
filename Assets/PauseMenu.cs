using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool IsGamePaused;
    public GameObject pauseMenuUI;
    public string MenuScene;


    void Start()
     {
         IsGamePaused = false;
         pauseMenuUI.SetActive(false);
     }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && IsGamePaused == false)
      {
          pauseMenuUI.SetActive(true);
          IsGamePaused = true;
      }
      else if (Input.GetKeyDown("space") && IsGamePaused == true)
      {
          pauseMenuUI.SetActive(false);
          IsGamePaused = false;
      } else{

      }
            
    } 


    
   

    public void returnToMenu(){
        SceneManager.LoadScene(MenuScene);

    }
}
