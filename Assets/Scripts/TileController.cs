using UnityEngine;
using UnityEngine.EventSystems;

public class TileController : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int tilePosition; // Координаты клетки на доске
    private BoardManager boardManager;

    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    // Обрабатываем клики по клетке
    public void OnPointerClick(PointerEventData eventData)
    {
        // Сообщаем BoardManager, что клетка была нажата
        Debug.Log("Клик на клетке с позицией: " + tilePosition); // Для отладки
        boardManager.OnTileClicked(tilePosition);
    }
}
