using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    [SerializeField]
    private readonly KeyCode openKey = KeyCode.Escape;


    // Start is called before the first frame update
    void Start()
    {
        this.pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            this.pauseMenu.SetActive(!pauseMenu.activeSelf);
        }

        if (this.pauseMenu.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ResumeAction()
    {
        this.pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitAction()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
