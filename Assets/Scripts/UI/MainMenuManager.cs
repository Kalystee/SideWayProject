using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{

    public void PlayAction()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void ExitAction()
    {
        Application.Quit();
    }
}
