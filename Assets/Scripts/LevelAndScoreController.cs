using UnityEngine;
using TMPro;
using System.Collections;

public class LevelAndScoreController : MonoBehaviour
{
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _totalScoreText;
    [SerializeField] private TMP_Text _winScoreText;
    [SerializeField] private TMP_Text _loseScoreText;
    private int _currentLevel;
    private int _totalScore;

    private void Start()
    {
        _currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        _level.text = $"LEVEL {_currentLevel}";
        _totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        _totalScoreText.text = _totalScore.ToString();
        _winScoreText.text = _totalScore.ToString();
        _loseScoreText.text = _totalScore.ToString();
    }

    public void Plus5Score()
    {
        StartCoroutine(WaitTOChangeSize(5));
    }

    public void Plus10Score()
    {
        StartCoroutine(WaitTOChangeSize(10));
    }

    private IEnumerator WaitTOChangeSize(int scorePlus)
    {
        _totalScoreText.transform.localScale = new Vector2(1.2f, 1.2f);
        _totalScore += scorePlus;
        _winScoreText.text = _totalScore.ToString();
        _loseScoreText.text = _totalScore.ToString();
        _totalScoreText.text = _totalScore.ToString();
        PlayerPrefs.SetInt("TotalScore", _totalScore);
        yield return new WaitForSeconds(0.15f);
        _totalScoreText.transform.localScale = new Vector2(1f, 1f);
    }
}