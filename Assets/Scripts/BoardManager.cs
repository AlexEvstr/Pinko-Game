using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public GameObject[,] tiles = new GameObject[8, 8]; // Ссылки на клетки доски
    public Color highlightColor; // Цвет для подсветки клеток

    // Инициализация клеток доски (теперь уже вручную настроенных)
    public void InitializeBoard(GameObject[] allTiles)
    {
        int index = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                // Привязываем клетки к массиву tiles
                tiles[x, y] = allTiles[index];
                index++;
            }
        }
    }

    // Подсветка возможных ходов
    public void HighlightPossibleMoves(Vector2Int[] possibleMoves)
    {
        foreach (var move in possibleMoves)
        {
            // Подсвечиваем доступные клетки
            tiles[move.x, move.y].GetComponent<Image>().color = highlightColor;
        }
    }

    // Сбрасываем подсветку всех клеток
    public void ResetHighlights()
    {
        foreach (var tile in tiles)
        {
            // Возвращаем цвет клеток к их изначальному состоянию
            tile.GetComponent<Image>().color = new Color(1, 1, 1, 0); // Прозрачный цвет
        }
    }

    // Проверяем, пуста ли клетка
    public bool IsTileEmpty(Vector2Int position)
    {
        // Логика для проверки, пуста ли клетка (например, проверка на наличие фигуры)
        // Это можно дополнить, если потребуется логика для расстановки и перемещения фигур
        return true;
    }
}
