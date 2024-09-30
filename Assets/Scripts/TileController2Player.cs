using UnityEngine;
using UnityEngine.EventSystems;

public class TileController2Player : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int tilePosition;
    private SecondModeBoard boardManager;

    private void Start()
    {
        boardManager = FindObjectOfType<SecondModeBoard>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        boardManager.OnTileClicked(tilePosition);
    }
}