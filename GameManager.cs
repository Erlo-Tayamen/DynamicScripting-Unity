using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (!panel.activeInHierarchy)
            {
                Pause();
            }
            else if (panel.activeInHierarchy)
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        panel.SetActive(true);
        Debug.Log("Pause");
    }

    public void Resume(){
        Time.timeScale = 1;
        panel.SetActive(false);
        Debug.Log("UnPause");
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
