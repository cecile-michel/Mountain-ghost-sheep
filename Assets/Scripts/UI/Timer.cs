using UnityEngine;
using UnityEngine.UI;
using System;

/**
	This class is the implementation of the timer used in the game and how it is handled in it
*/
public class Timer : MonoBehaviour
{
    private float initTimerValue;
    private Text timerText;
    private float maxMinutes = 2;


    public GameManager gameManager;
    public PacmanGameManager PacmanGameManager;

    private float timeLeft;

    private TimeManager TimeManager;

    public enum TypeOfGame {
        spaceGhostSheep = 0,
        pacman = 1
    }
    public TypeOfGame gameType;

    public void Awake() {
        initTimerValue = Time.time; 
    }

    // Start is called before the first frame update
    public void Start() {
        maxMinutes = TimeManager.time;
        timerText = GetComponent<Text>();
        timerText.text = "temps restant " + string.Format("{0:00}:{1:00}", 0, 0);
        timeLeft = maxMinutes*60;
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    public void Update() {
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            timerText = GetComponent<Text>();
            timerText.text = "temps restant " + string.Format("{0:00}:{1:00}", Math.Truncate(timeLeft/60), timeLeft - Math.Truncate(timeLeft/60)*60);

        } else if (Time.timeScale > 0f){
            Time.timeScale = 0f;
            if (gameType == TypeOfGame.spaceGhostSheep) {
                gameManager.endGame();
            } else if (gameType == TypeOfGame.pacman) {
                // TODO implement for the last maze (end) and switch scene if the time is finished
            }
            
        }        
    }

    public void restart()
    {
        maxMinutes = TimeManager.time;
        timerText = GetComponent<Text>();
        timerText.text = "temps restant " + string.Format("{0:00}:{1:00}", 0, 0);
        timeLeft = maxMinutes * 60;
        Time.timeScale = 1f;
    }
}
