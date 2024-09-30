using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private PieceController selectedPiece = null;
    public Transform boardParent;
    private GameObject[,] board = new GameObject[8, 8];
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _selectionPanel;
    public bool playerIsSheep;
    public bool computerPlaysForSheep;
    public List<Vector2Int> sheepMoveHistory = new List<Vector2Int>();
    private GameManager gameManager;
    public int boardSize = 8;

    private bool playerTurn = true;
    private bool gameOver = false;

    private ComputerPlayer computerPlayer;

    private void Start()
    {
        computerPlayer = FindObjectOfType<ComputerPlayer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SelectPiece(PieceController piece)
    {
        if (gameOver) return;
        if (playerTurn && piece.isSheep == playerIsSheep)
        {
            selectedPiece = piece;
            ResetHighlights();
            HighlightPossibleMoves(piece);
        }
    }

    public void HighlightPossibleMoves(PieceController piece)
    {
        Vector2Int currentPos = piece.currentPosition;

        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        if (piece.isSheep)
        {
            // Овца может ходить по диагонали во все 4 направления
            possibleMoves.Add(currentPos + new Vector2Int(-1, -1)); // Вверх-влево
            possibleMoves.Add(currentPos + new Vector2Int(-1, 1));  // Вверх-вправо
            possibleMoves.Add(currentPos + new Vector2Int(1, -1));  // Вниз-влево
            possibleMoves.Add(currentPos + new Vector2Int(1, 1));   // Вниз-вправо
        }
        else
        {
            // Волки могут ходить по диагонали только вперед
            possibleMoves.Add(currentPos + new Vector2Int(1, 1));  // Вперед-вправо
            possibleMoves.Add(currentPos + new Vector2Int(1, -1)); // Вперед-влево
        }

        // Подсветка возможных ходов
        foreach (Vector2Int move in possibleMoves)
        {
            if (IsWithinBounds(move) && !IsPositionOccupied(move))
            {
                HighlightTile(move);
            }
        }
    }

    private void HighlightTile(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            Transform tile = boardParent.GetChild(position.x * 8 + position.y);
            var tileImage = tile.GetComponent<UnityEngine.UI.Image>();

            if (tileImage != null)
            {
                tileImage.color = new Color(1, 0, 0.75f, 1); // Подсвечиваем клетку розовым цветом
            }
            else
            {
                Debug.LogWarning($"Клетка {position} не содержит компонента Image для подсветки.");
            }
        }
        else
        {
            Debug.LogWarning($"Позиция {position} вне границ доски.");
        }
    }



    public List<Vector2Int> GetHighlightedTiles(PieceController piece)
    {
        List<Vector2Int> highlightedTiles = new List<Vector2Int>();
        Vector2Int currentPos = piece.currentPosition;

        // Для овцы
        if (piece.isSheep)
        {
            CheckAndAddTile(currentPos + new Vector2Int(1, 1), highlightedTiles);
            CheckAndAddTile(currentPos + new Vector2Int(-1, 1), highlightedTiles);
            CheckAndAddTile(currentPos + new Vector2Int(1, -1), highlightedTiles);
            CheckAndAddTile(currentPos + new Vector2Int(-1, -1), highlightedTiles);
        }
        else // Для волков
        {
            CheckAndAddTile(currentPos + new Vector2Int(1, 1), highlightedTiles);
            CheckAndAddTile(currentPos + new Vector2Int(1, -1), highlightedTiles);
        }

        Debug.Log("Возможные ходы для " + (piece.isSheep ? "овцы" : "волка") + ": " + string.Join(", ", highlightedTiles));

        return highlightedTiles;
    }

    public List<Vector2Int> GetAvailableMovesForSheep(PieceController sheep)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] diagonalMoves = {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

        foreach (Vector2Int move in diagonalMoves)
        {
            Vector2Int newPosition = sheep.currentPosition + move;
            if (IsWithinBounds(newPosition) && !IsPositionOccupied(newPosition))
            {
                possibleMoves.Add(newPosition);
            }
        }

        return possibleMoves;
    }

    private void CheckAndAddTile(Vector2Int position, List<Vector2Int> highlightedTiles)
    {
        if (IsWithinBounds(position))
        {
            Transform tile = boardParent.GetChild(position.x * 8 + position.y);
            if (tile.childCount == 0)
            {
                highlightedTiles.Add(position);
            }
            else
            {
                Debug.Log($"Клетка {position} занята.");
            }
        }
    }

    public void OnTileClicked(Vector2Int tilePosition)
    {
        if (gameOver || selectedPiece == null || !playerTurn) return;

        // Передаем третий параметр для проверки, овца это или нет
        if (IsMoveValid(selectedPiece.currentPosition, tilePosition, selectedPiece.isSheep))
        {
            MovePiece(selectedPiece, tilePosition);
            ResetHighlights();

            if (selectedPiece.isSheep && HasSheepReachedTop(selectedPiece))
            {
                Debug.Log("Овца дошла до верха. Победа!");
                ShowWinPanel();
                gameOver = true;
                return;
            }

            playerTurn = false;
            Invoke("InvokeComputerMove", 1f);
            selectedPiece = null;
        }
        else
        {
            Debug.LogError("Невалидный ход!");
        }
    }


    public bool IsDiagonalMove(Vector2Int fromPos, Vector2Int toPos)
    {
        int deltaX = Mathf.Abs(fromPos.x - toPos.x);
        int deltaY = Mathf.Abs(fromPos.y - toPos.y);

        return deltaX == 1 && deltaY == 1;
    }

    public bool IsMoveValid(Vector2Int currentPos, Vector2Int targetPos, bool isSheep)
    {
        if (!IsWithinBounds(targetPos))
        {
            Debug.Log($"Клетка {targetPos} вне границ доски.");
            return false;
        }

        if (IsPositionOccupied(targetPos))
        {
            Debug.Log($"Клетка {targetPos} занята.");
            return false;
        }

        // Проверяем, что ход диагональный
        if (!IsDiagonalMove(currentPos, targetPos))
        {
            Debug.Log($"Ход {currentPos} -> {targetPos} не является диагональным.");
            return false;
        }

        // Проверка направления для волков (волки могут ходить только вперед)
        if (!isSheep)
        {
            // Волки могут ходить только вверх (вперед)
            if (targetPos.x <= currentPos.x)
            {
                Debug.Log("Волк не может двигаться назад.");
                return false;
            }
        }

        return true;
    }



    private void InvokeComputerMove()
    {
        if (gameOver) return;

        computerPlayer.MakeComputerMove();

        if (IsSheepBlocked())
        {
            Debug.Log("Lose! Овца заблокирована.");
            ShowLosePanel();
            gameOver = true;
        }

        playerTurn = true;
    }

    public bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }

    public bool IsPositionOccupied(Vector2Int position)
    {
        foreach (var piece in GetAllPieces())
        {
            if (piece.currentPosition == position)
            {
                return true;
            }
        }
        return false;
    }

    private List<PieceController> GetAllPieces()
    {
        List<PieceController> allPieces = new List<PieceController>();
        allPieces.AddRange(GetWolves());
        allPieces.Add(GetSheep());
        return allPieces;
    }

    public PieceController GetSheep()
    {
        foreach (Transform tile in boardParent)
        {
            var piece = tile.GetComponentInChildren<PieceController>();
            if (piece != null && piece.isSheep)
            {
                return piece;
            }
        }
        return null;
    }

    public PieceController[] GetWolves()
    {
        List<PieceController> wolves = new List<PieceController>();
        foreach (Transform tile in boardParent)
        {
            var piece = tile.GetComponentInChildren<PieceController>();
            if (piece != null && !piece.isSheep)
            {
                wolves.Add(piece); // Добавляем волка в список
            }
        }
        return wolves.ToArray(); // Возвращаем массив волков
    }


    public void MovePiece(PieceController piece, Vector2Int newPosition)
    {
        if (!IsWithinBounds(newPosition))
        {
            Debug.LogError($"Позиция {newPosition} выходит за пределы доски.");
            return;
        }

        Transform newTile = boardParent.GetChild(newPosition.x * 8 + newPosition.y);
        piece.MoveToTile(newTile);
        piece.currentPosition = newPosition;
        Debug.Log("Позиция овцы обновлена: " + newPosition);
    }

    public void ResetHighlights()
    {
        foreach (Transform tile in boardParent)
        {
            tile.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0);
        }
    }

    public bool HasSheepReachedTop(PieceController piece)
    {
        return piece.currentPosition.x == 0;
    }

    public bool IsSheepBlocked()
    {
        PieceController sheep = GetSheep();
        Vector2Int currentPos = sheep.currentPosition;

        bool topRightBlocked = IsTileBlocked(currentPos + new Vector2Int(1, 1));
        bool topLeftBlocked = IsTileBlocked(currentPos + new Vector2Int(-1, 1));
        bool bottomRightBlocked = IsTileBlocked(currentPos + new Vector2Int(1, -1));
        bool bottomLeftBlocked = IsTileBlocked(currentPos + new Vector2Int(-1, -1));

        return topRightBlocked && topLeftBlocked && bottomRightBlocked && bottomLeftBlocked;
    }

    private bool IsTileBlocked(Vector2Int position)
    {
        if (!IsWithinBounds(position))
        {
            return true;
        }

        Transform tile = boardParent.GetChild(position.x * 8 + position.y);
        return tile.childCount > 0;
    }

    public void ShowWinPanel()
    {
        _winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        _losePanel.SetActive(true);
    }

    public void OnSheepSelected()
    {
        playerIsSheep = true;
        computerPlaysForSheep = false;
        gameManager.PlayerMove();

        // Теперь нужно отключить одного волка, если игрок выбрал овцу
        if (computerPlaysForSheep)
        {
            DisableOneWolf();
        }

        HideSelectionPanel(); // Закрываем панель с выбором
    }

    // Когда игрок выбирает волков
    public void OnWolvesSelected()
    {
        playerIsSheep = false;
        computerPlaysForSheep = true;
        gameManager.PlayerMove();

        // Отключаем одного волка, так как компьютер играет за овцу
        if (computerPlaysForSheep)
        {
            DisableOneWolf();
        }

        HideSelectionPanel(); // Закрываем панель с выбором
    }

    private void DisableOneWolf()
    {
        PieceController[] wolves = GetWolves();
        if (wolves.Length > 3) // Проверяем, что волков больше трёх
        {
            PieceController wolfToDisable = wolves[3]; // Отключаем 4-го волка
            wolfToDisable.gameObject.SetActive(false);
            Debug.Log("Четвертый волк отключен для облегчения игры.");
        }
    }


    public void HideSelectionPanel()
{
    _selectionPanel.SetActive(false); // Скрываем панель выбора
}

}
