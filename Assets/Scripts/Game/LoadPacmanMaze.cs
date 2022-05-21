using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPacmanMaze : MonoBehaviour
{

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;

    public GameObject cherry;

    private LevelDifficulty ld;
    // Start is called before the first frame update
    void Start()
    {
        LevelDifficulty.difficulty = 3;
       if (LevelDifficulty.difficulty == 0) {
           loadLevel(true, false, false);
       } else if (LevelDifficulty.difficulty == 1) {
           loadLevel(false, true, false); 
       } else {
           loadLevel(false, false, true);
       }

    }

    void Update() {
        canLoadCherry();
    }
    private void loadLevel(bool l1, bool l2, bool l3) {
        level1.SetActive(l1);
        level2.SetActive(l2);
        level3.SetActive(l3);
    }

    private void canLoadCherry() {
        GameObject[] pac_gommes = GameObject.FindGameObjectsWithTag("Pac-gommes");
        if (pac_gommes.Length == 0) {
            cherry.SetActive(true);
        }
    }
}
