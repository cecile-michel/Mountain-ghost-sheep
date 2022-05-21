using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{

    public Slider slider;
    public static float time = 5;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        time = slider.value;
        text.text = "Length of a round : 2 min";
    }

    // Update is called once per frame
    void Update()
    {
        time = slider.value;
        text.text = "Length of a round : " + time + " min";
    }
}
