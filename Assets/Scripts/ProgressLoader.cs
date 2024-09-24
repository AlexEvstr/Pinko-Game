using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressLoader : MonoBehaviour
{
    public Image progressBar;
    public float totalLoadTime = 3f;

    private float currentTime = 0f;

    void Awake()
    {
        progressBar.fillAmount = 0f;
        StartCoroutine(FillProgressBar());
    }

    IEnumerator FillProgressBar()
    {
        while (currentTime < totalLoadTime)
        {
            currentTime += Time.deltaTime;
            progressBar.fillAmount = Mathf.Lerp(0, 1, currentTime / totalLoadTime);
            yield return null;
        }

        SceneManager.LoadScene("MenuScreen");
    }
}
