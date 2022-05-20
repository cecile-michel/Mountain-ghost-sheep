using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallDisappearance : MonoBehaviour
{
    private GameObject[] walls;

    private int DISAPPEARANCE_TIME = 15;
    // Start is called before the first frame update
    void Start()
    {
        walls = GameObject.FindGameObjectsWithTag("MurAdd");
        Invoke("disappear", DISAPPEARANCE_TIME);
    }

    private void disappear() {
        int index = Random.Range(0, walls.Length-1);
        walls[index].SetActive(false);
        walls = GameObject.FindGameObjectsWithTag("MurAdd");
        if (walls.Length > 0) {
            Invoke("disappear", DISAPPEARANCE_TIME);
        }
    }
}
