using UnityEngine;
using UnityEngine.EventSystems;

public class TileController : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int tilePosition;
    private BoardManager boardManager;

    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        boardManager.OnTileClicked(tilePosition);
    }
}
