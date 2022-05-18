using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberPlayers : MonoBehaviour
{

    public Text text;
    public Slider slider;

    /*
        value of the players range from 0 to 3
        0 = wasd
        1 = ijkl
        2 = arrows
        3 = bot
    */
    static public int player1 = 0;
    static public int player2 = 0;
    static public int player3 = 0;
    // Start is called before the first frame update
    void Start()
    {
        text.text = slider.value.ToString();
        numberOfPlayers(slider.value);

    }

    // Update is called once per frame
    void Update()
    {
        text.text = slider.value.ToString();
        numberOfPlayers(slider.value);
    }

    /*
        numbers range from 1 to 3 (number of players)
    */
    private void numberOfPlayers(float number) {
        if (number == 1) {
            player1 = 0;
            player2 = 3;
            player3 = 3;
        } else if (number == 2) {
            player1 = 0;
            player2 = 1;
            player3 = 3;
        } else {
            player1 = 0;
            player2 = 1;
            player3 = 2;
        }
    }
}
