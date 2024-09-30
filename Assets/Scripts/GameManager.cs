using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private BoardManager boardManager;
    private bool isPlayerTurn = true;
    private int currentWolfIndex = 0;
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _game;
    private OptionsController _optionsController;

    private void Start()
    {
        _optionsController = GetComponent<OptionsController>();
        boardManager = FindObjectOfType<BoardManager>();
        PlayerPrefs.SetString("ShouldShowLoadAndWelcome", "yes");
    }

    public void OpenOptions()
    {
        _game.SetActive(false);
        _options.SetActive(true);
        _optionsController.HideShowBtn();
    }

    public void CloseOptions()
    {
        _options.SetActive(false);
        _game.SetActive(true);
    }

    private void ExecuteComputerTurn()
    {
        if (boardManager.computerPlaysForSheep)
        {
            PerformSheepMove();
        }
        else
        {
            PerformWolfMove();
        }

        isPlayerTurn = true;
    }

    private void PerformSheepMove()
    {
        PieceController sheep = boardManager.GetSheep();
        if (sheep == null) return;

        List<Vector2Int> availableMoves = boardManager.GetHighlightedTiles(sheep);
        if (availableMoves.Count > 0)
        {
            Vector2Int move = availableMoves[Random.Range(0, availableMoves.Count)];
            boardManager.MovePiece(sheep, move);
        }
    }

    private void PerformWolfMove()
    {
        PieceController[] wolves = boardManager.GetWolves();
        if (wolves.Length == 0) return;

        PieceController currentWolf = wolves[currentWolfIndex];
        List<Vector2Int> availableMoves = boardManager.GetHighlightedTiles(currentWolf);
        if (availableMoves.Count > 0)
        {
            Vector2Int move = availableMoves[Random.Range(0, availableMoves.Count)];
            boardManager.MovePiece(currentWolf, move);
        }

        currentWolfIndex = (currentWolfIndex + 1) % wolves.Length;
    }

    public void PlayerMove()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;

            Invoke(nameof(ExecuteComputerTurn), 1f);
        }
    }

    public void BackToMenu()
    {
        StartCoroutine(WaitToBack());
    }

    private IEnumerator WaitToBack()
    {
        PlayerPrefs.SetString("ShouldShowLoadAndWelcome", "no");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MenuScreen");
    }

    public void RestartGame()
    {
        StartCoroutine(WaitTOReload());
    }

    private IEnumerator WaitTOReload()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}