using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject _welcome;
    public GameObject _menu;
    public GameObject _options;
    public GameObject _tutorial;
    private OptionsController _optionsController;
    [SerializeField] private GameObject _content;

    private void Start()
    {
        _optionsController = GetComponent<OptionsController>();
        PlayerPrefs.SetString("ShouldShowChoosePanel", "yes");
    }

    public void CloseWelcome()
    {
        _welcome.SetActive(false);
        _menu.SetActive(true);
    }

    public void PressPlayGame()
    {
        StartCoroutine(WaitBeforePlay());
    }

    private IEnumerator WaitBeforePlay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameVsComputerScreen");
    }

    public void PressPlayGameVsPlayer()
    {
        StartCoroutine(WaitBeforePlay2());
    }

    private IEnumerator WaitBeforePlay2()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameVsPlayerScreen");
    }

    public void OpenOptions()
    {
        _menu.SetActive(false);
        _options.SetActive(true);
        _optionsController.HideShowBtn();
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
        _content.transform.position = new Vector2(_content.transform.position.x, 0);
        _tutorial.SetActive(false);
        _menu.SetActive(true);
    }
}