using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    public void CompleteLevel()
    {
        // Complete the level, and save the progress
        SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel);

        // focus the level selection when we return to the menu scene
        Manager.Instance.menuFocus = 1;

        ExitScene();
    }

    public void ExitScene()
    {
        SceneManager.LoadScene("Menu");
    }


}
