using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private PieceController selectedPiece = null; // Выбранная фигура
    public Transform boardParent; // Родительский объект для клеток доски

    // Метод для выбора фигуры
    public void SelectPiece(PieceController piece)
    {
        selectedPiece = piece;
        Debug.Log("Выбрана фигура: " + piece.name);
        ResetHighlights(); // Сбросить предыдущие подсветки
        HighlightPossibleMoves(piece); // Подсветить возможные ходы
    }

    // Метод для обработки клика по клетке
    public void OnTileClicked(Vector2Int tilePosition)
    {
        if (selectedPiece != null)
        {
            Debug.Log("Выбрана клетка для перемещения: " + tilePosition);
            if (IsMoveValid(tilePosition)) // Проверяем, можно ли переместить
            {
                MovePiece(selectedPiece, tilePosition);
            }
            else
            {
                Debug.LogError("Невалидный ход!");
            }
            ResetHighlights(); // Сбросить подсветки после хода
            selectedPiece = null; // Сбросить выбранную фигуру
        }
    }

    // Метод для проверки валидности хода
    private bool IsMoveValid(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            Transform tile = boardParent.GetChild(position.x * 8 + position.y);
            return tile.GetComponent<UnityEngine.UI.Image>().color == Color.green; // Проверяем цвет
        }
        return false;
    }

    private bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
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

        // Получаем новую клетку и перемещаем фигуру
        Transform newTile = boardParent.GetChild(childIndex);
        piece.MoveToTile(newTile); // Передаем новую клетку
        piece.currentPosition = newPosition; // Обновляем текущую позицию фигуры

        Debug.Log($"Фигура {piece.name} перемещена на {newPosition}");
    }

    // Метод для подсветки возможных ходов
    public void HighlightPossibleMoves(PieceController piece)
    {
        Vector2Int currentPos = piece.currentPosition;

        if (piece.isSheep)
        {
            HighlightTile(currentPos + new Vector2Int(1, 1)); // Diagonal forward right
            HighlightTile(currentPos + new Vector2Int(-1, 1)); // Diagonal forward left
            HighlightTile(currentPos + new Vector2Int(1, -1)); // Diagonal backward right
            HighlightTile(currentPos + new Vector2Int(-1, -1)); // Diagonal backward left
        }
        else // Для волков
        {
            HighlightTile(currentPos + new Vector2Int(1, 1)); // Diagonal forward right
            HighlightTile(currentPos + new Vector2Int(1, -1)); // Diagonal forward left
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
            tile.GetComponent<UnityEngine.UI.Image>().color = Color.green; // Подсветить клетку
        }
    }
}
