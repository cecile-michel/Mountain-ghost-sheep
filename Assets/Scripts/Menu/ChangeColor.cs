using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{

    public Text text;
    public Slider red;
    public Slider green;
    public Slider blue;
    static public Color joueur1 = new Color(0, 0, 1);
    static public Color joueur2 = new Color(1, 0.5f, 0);
    static public Color joueur3 = new Color(1, 0.3f, 1);
    public int player;
    // Start is called before the first frame update
    void Start()
    {
        red.minValue = 0f;
        red.maxValue = 1f;

        green.minValue = 0f;
        green.maxValue = 1f;

        blue.minValue = 0f;
        blue.maxValue = 1f;
        text.color = new Color(red.value, green.value, blue.value);
        update_colors(player, text.color);
        
    }

    // Update is called once per frame
    void Update()
    {
        text.color = new Color(red.value, green.value, blue.value);
        update_colors(player, text.color);
    }

    private static void update_colors(int player, Color color) {
        if (player == 0) {
            joueur1 = color;
        } else if (player == 1) {
            joueur2 = color;
        } else if (player == 3) {
            joueur3 = color;
        }
    }
}
