using UnityEngine;

public class SetupBoard : MonoBehaviour
{
    public GameObject boardParent; // Объект, который содержит все клетки доски

    void Start()
    {
        AssignTilePositions();
    }

    void AssignTilePositions()
    {
        int columns = 8; // Количество столбцов
        int rows = 8; // Количество рядов

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Рассчитываем индекс клетки в иерархии
                int index = x * rows + y;

                // Получаем клетку по индексу
                GameObject tile = boardParent.transform.GetChild(index).gameObject;

                // Добавляем компонент TileController, если его ещё нет
                TileController tileController = tile.GetComponent<TileController>();
                if (tileController == null)
                {
                    tileController = tile.AddComponent<TileController>();
                }

                // Устанавливаем координаты клетки
                tileController.tilePosition = new Vector2Int(x, y);
            }
        }
    }
}
