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
        PlayerPrefs.SetString("ShouldShowLoadAndWelcome", "no");
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

    // Метод для выполнения хода компьютера (волков или овцы)
    private void ExecuteComputerTurn()
    {
        if (boardManager.computerPlaysForSheep)
        {
            // Если компьютер управляет овцой
            PerformSheepMove();
        }
        else
        {
            // Если компьютер управляет волками, ходит только один волк
            PerformWolfMove();
        }

        // После хода компьютера передаем ход игроку
        isPlayerTurn = true;
    }

    // Метод для выполнения хода овцы
    private void PerformSheepMove()
    {
        PieceController sheep = boardManager.GetSheep();
        if (sheep == null) return;

        // Логика для хода овцы
        List<Vector2Int> availableMoves = boardManager.GetHighlightedTiles(sheep);
        if (availableMoves.Count > 0)
        {
            Vector2Int move = availableMoves[Random.Range(0, availableMoves.Count)];
            boardManager.MovePiece(sheep, move);
            Debug.Log($"Компьютер сделал ход овцой на позицию {move}");
        }
    }

    // Метод для выполнения хода одного волка
    private void PerformWolfMove()
    {
        PieceController[] wolves = boardManager.GetWolves();
        if (wolves.Length == 0) return;

        // Делаем ход только текущим волком
        PieceController currentWolf = wolves[currentWolfIndex];
        List<Vector2Int> availableMoves = boardManager.GetHighlightedTiles(currentWolf);
        if (availableMoves.Count > 0)
        {
            Vector2Int move = availableMoves[Random.Range(0, availableMoves.Count)];
            boardManager.MovePiece(currentWolf, move);
        }

        // Обновляем индекс волка для следующего хода
        currentWolfIndex = (currentWolfIndex + 1) % wolves.Length;
    }

    // Вызывается каждый ход
    public void PlayerMove()
    {
        if (isPlayerTurn)
        {
            // Игрок делает ход
            isPlayerTurn = false;

            // После хода игрока передаем ход компьютеру
            Invoke(nameof(ExecuteComputerTurn), 1f); // Добавим небольшую задержку для хода компьютера
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
