using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueGame : MonoBehaviour
{
    public GameObject dog1;
    public GameObject dog2;
    public GameObject dog3;
    public Button resume_btn;

    void Start() {
        Button btn = resume_btn.GetComponent<Button>();
		btn.onClick.AddListener(OnClick);
    }
    void OnClick()
        {
            //Output this to console when Button1 or Button3 is clicked
            Debug.Log("You have clicked the button!");
            dog1.GetComponent<MoveWithKeyboardBehavior>().unPause();
            dog2.GetComponent<MoveWithKeyboardBehavior>().unPause();
            dog3.GetComponent<MoveWithKeyboardBehavior>().unPause();
        }
}
