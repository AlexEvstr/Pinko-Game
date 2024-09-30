using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressLoader : MonoBehaviour
{
    public Image progressBar;
    public GameObject _loading;
    public GameObject _welcome;
    public GameObject _menu;
    private float totalLoadTime = 3f;

    private float currentTime = 0f;

    void Start()
    {
        string loadAndWelcome = PlayerPrefs.GetString("ShouldShowLoadAndWelcome", "yes");
        if (loadAndWelcome != "yes")
        {
            _loading.SetActive(false);
            _welcome.SetActive(false);
            _menu.SetActive(true);
        }
        else
        {
            progressBar.fillAmount = 0f;
            StartCoroutine(FillProgressBar());
        }
        PlayerPrefs.SetString("ShouldShowLoadAndWelcome", "yes");
    }

    IEnumerator FillProgressBar()
    {
        while (currentTime < totalLoadTime)
        {
            currentTime += Time.deltaTime;
            progressBar.fillAmount = Mathf.Lerp(0, 1, currentTime / totalLoadTime);
            yield return null;
        }

        _loading.SetActive(false);
        _welcome.SetActive(true);
    }
}