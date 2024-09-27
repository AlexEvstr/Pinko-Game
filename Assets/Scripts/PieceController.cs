using UnityEngine;
using UnityEngine.EventSystems;

public class PieceController : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int currentPosition; // Текущая позиция фигуры
    public bool isSheep; // Это овца или волк?

    private BoardManager boardManager;

    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        boardManager.SelectPiece(this); // Передаем текущую фигуру в BoardManager
    }

    public void MoveToTile(Transform newTile)
    {
        // Меняем родителя на новую клетку
        transform.SetParent(newTile);

        // Обновляем позицию
        transform.localPosition = Vector3.zero; // Центрируем фигуру внутри клетки

        // Обновляем текущую позицию
        currentPosition = newTile.GetComponent<TileController>().tilePosition; // Обновляем позицию на основе нового родителя
    }
}