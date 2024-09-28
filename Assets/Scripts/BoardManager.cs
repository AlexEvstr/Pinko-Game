using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private PieceController selectedPiece = null; // Выбранная фигура
    public Transform boardParent; // Родительский объект для клеток доски
    private GameObject[,] board = new GameObject[8, 8]; // Массив для хранения фигур на доске
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    // Метод для выбора фигуры
    public void SelectPiece(PieceController piece)
    {
        selectedPiece = piece;
        ResetHighlights(); // Сбросить предыдущие подсветки
        HighlightPossibleMoves(piece); // Подсветить возможные ходы
    }

    // Метод для обработки клика по клетке
    // Метод для обработки клика по клетке
    public void OnTileClicked(Vector2Int tilePosition)
    {
        if (selectedPiece != null)
        {

            if (IsMoveValid(tilePosition)) // Проверяем, можно ли переместить
            {
                MovePiece(selectedPiece, tilePosition);

                // После хода проверяем, не достигла ли овца верха (если ходит овца)
                if (selectedPiece.isSheep && HasSheepReachedTop(selectedPiece))
                {
                    Debug.Log("Win! Овца дошла до верха.");
                    ResetHighlights();
                    // Логика для окна Win
                    return;
                }
            }

            ResetHighlights(); // Сбросить подсветки после хода

            // Если ходит волк, проверяем, заблокирована ли овца
            if (!selectedPiece.isSheep && IsSheepBlocked())
            {
                Debug.Log("Lose! Овца заблокирована.");
                // Логика для окна Lose
            }

            selectedPiece = null; // Сбрасываем выбранную фигуру после хода
        }
    }


    // Метод для проверки заблокирована ли овца
    private bool IsSheepBlocked()
    {
        PieceController sheep = FindObjectOfType<PieceController>(); // Находим овцу
        if (sheep == null || !sheep.isSheep)
        {
            Debug.LogError("Овца не найдена или объект не является овцой.");
            return false;
        }

        Vector2Int currentPos = sheep.currentPosition;

        // Проверяем все 4 диагонали
        bool topRightBlocked = IsTileBlocked(currentPos + new Vector2Int(1, 1));
        bool topLeftBlocked = IsTileBlocked(currentPos + new Vector2Int(-1, 1));
        bool bottomRightBlocked = IsTileBlocked(currentPos + new Vector2Int(1, -1));
        bool bottomLeftBlocked = IsTileBlocked(currentPos + new Vector2Int(-1, -1));

        // Возвращаем true, если все диагонали заблокированы
        bool isBlocked = topRightBlocked && topLeftBlocked && bottomRightBlocked && bottomLeftBlocked;
        return isBlocked;
    }



    private bool IsTileBlocked(Vector2Int position)
    {
        if (!IsWithinBounds(position))
        {
            Debug.Log($"Клетка {position} находится вне границ доски.");
            return true; // Стены блокируют
        }

        Transform tile = boardParent.GetChild(position.x * 8 + position.y);
        bool isBlocked = tile.childCount > 0; // Проверяем наличие фигуры на клетке
        return isBlocked;
    }


    private bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }








    // Проверка, достигла ли овца верха
    private bool HasSheepReachedTop(PieceController piece)
    {
        return piece.currentPosition.x == 0; // Если овца находится на верхней строке (0)
    }

    // Проверка наличия доступных ходов для овцы
    //private bool HasAvailableMoves(PieceController piece)
    //{
    //    Vector2Int currentPos = piece.currentPosition;

    //    // Проверяем доступные ходы (по диагонали)
    //    if (piece.isSheep)
    //    {
    //        if (IsMoveValid(currentPos + new Vector2Int(1, 1)) || // Diagonal forward right
    //            IsMoveValid(currentPos + new Vector2Int(-1, 1)) || // Diagonal forward left
    //            IsMoveValid(currentPos + new Vector2Int(1, -1)) || // Diagonal backward right
    //            IsMoveValid(currentPos + new Vector2Int(-1, -1)))   // Diagonal backward left
    //        {
    //            return true;
    //        }
    //    }
    //    else // Для волков
    //    {
    //        if (IsMoveValid(currentPos + new Vector2Int(1, 1)) || // Diagonal forward right
    //            IsMoveValid(currentPos + new Vector2Int(1, -1)) // Diagonal forward left
    //        )
    //        {
    //            return true;
    //        }
    //    }
    //    return false; // Если доступных ходов нет
    //}





    private bool IsMoveValid(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            Transform tile = boardParent.GetChild(position.x * 8 + position.y);

            // Проверяем наличие других фигур на клетке
            foreach (Transform child in tile)
            {
                if (child.GetComponent<PieceController>() != null)
                {
                    return false; // Если на клетке есть фигура, клетка недоступна
                }
            }

            // Теперь проверяем, является ли клетка доступной для перемещения (должна быть розовая или прозрачная)
            Color tileColor = tile.GetComponent<UnityEngine.UI.Image>().color;
            return tileColor == new Color(1, 0, 0.75f, 1); // Проверяем, что клетка подсвечена розовым
        }
        return false;
    }


    public void MovePiece(PieceController piece, Vector2Int newPosition)
    {
        // Проверяем, находится ли клетка в пределах доски
        if (newPosition.x < 0 || newPosition.x >= 8 || newPosition.y < 0 || newPosition.y >= 8)
        {
            Debug.LogError($"Позиция {newPosition} выходит за пределы доски.");
            return;
        }

        // Рассчитываем индекс клетки
        int childIndex = newPosition.x * 8 + newPosition.y;

        // Проверяем, что индекс находится в пределах количества клеток
        if (childIndex < 0 || childIndex >= boardParent.childCount)
        {
            Debug.LogError($"Индекс {childIndex} выходит за пределы иерархии объектов.");
            return;
        }

        Transform newTile = boardParent.GetChild(newPosition.x * 8 + newPosition.y);
        piece.MoveToTile(newTile); // Передаем новую клетку
        piece.currentPosition = newPosition; // Обновляем текущую позицию фигуры

    }

    public void HighlightPossibleMoves(PieceController piece)
    {
        Vector2Int currentPos = piece.currentPosition;

        if (piece.isSheep)
        {
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, 1)); // Diagonal forward right
            HighlightTileIfAvailable(currentPos + new Vector2Int(-1, 1)); // Diagonal forward left
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, -1)); // Diagonal backward right
            HighlightTileIfAvailable(currentPos + new Vector2Int(-1, -1)); // Diagonal backward left
        }
        else // Для волков
        {
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, 1)); // Diagonal forward right
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, -1)); // Diagonal forward left
        }
    }

    // Метод для подсветки конкретной клетки, если на ней нет других фигур
    private void HighlightTileIfAvailable(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            // Проверяем, есть ли уже фигура на клетке
            if (boardParent.GetChild(position.x * 8 + position.y).childCount == 0)
            {
                HighlightTile(position); // Подсветить клетку
            }
        }
    }


    // Метод для сброса подсветки
    public void ResetHighlights()
    {
        foreach (Transform tile in boardParent)
        {
            tile.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0); // Сброс цвета
        }
    }

    // Метод для подсветки конкретной клетки
    private void HighlightTile(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            Transform tile = boardParent.GetChild(position.x * 8 + position.y);
            tile.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0.75f, 1); // Подсветить клетку
        }
    }
}
