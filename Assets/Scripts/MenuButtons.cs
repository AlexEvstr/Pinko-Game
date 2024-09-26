using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject _welcome;
    public GameObject _menu;
    public GameObject _options;
    public GameObject _tutorial;

    public void CloseWelcome()
    {
        _welcome.SetActive(false);
        _menu.SetActive(true);
    }

    public void PressPlayGame()
    {
        SceneManager.LoadScene("GameScreen");
    }

    public void OpenOptions()
    {
        _menu.SetActive(false);
        _options.SetActive(true);
    }

    public void CloseOptions()
    {
        _options.SetActive(false);
        _menu.SetActive(true);
    }

    public void OpenTutorial()
    {
        _menu.SetActive(false);
        _tutorial.SetActive(true);
    }

    public void CloseTutorial()
    {
        _tutorial.SetActive(false);
        _menu.SetActive(true);
    }
}