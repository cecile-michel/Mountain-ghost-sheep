using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextLevel : MonoBehaviour
{

    // TODO utiliser cette classe pour sauver les objets etc et passer à la prochaine scène
    public static void changeScene(int sceneID) {
        SceneManager.LoadScene(sceneID);
    }
}
