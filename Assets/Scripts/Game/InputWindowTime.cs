using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class InputWindowTime : MonoBehaviour
{

    private Button_UI okButton;

    private void Awake()
    {
        Hide();

    }
    public void Show()
    {
        gameObject.SetActive(true);
        
    }

    public void Hide()
    {
        gameObject.SetActive(false);

    }
}
