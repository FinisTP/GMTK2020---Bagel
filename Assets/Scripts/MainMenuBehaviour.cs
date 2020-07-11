using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehaviour : MonoBehaviour
{
    SceneChanger sceneChanger;
    private void Start()
    {
        sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
    }
    public void LoadScene(string sceneName)
    {
        sceneChanger.LoadScene(sceneName);
    }

    public void LoadNextScene()
    {
        sceneChanger.LoadNextScene();
    }

    public void LoadPreviousScene()
    {
        sceneChanger.PreviousScene();
    }

    public void QuitGame()
    {
        sceneChanger.QuitGame();
    }

}
