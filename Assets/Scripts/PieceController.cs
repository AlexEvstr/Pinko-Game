using UnityEngine;
using UnityEngine.EventSystems;

public class PieceController : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int currentPosition;
    public bool isSheep;
    [SerializeField] private GameSoundManager gameSoundManager;

    private BoardManager boardManager;

    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        boardManager.SelectPiece(this);
    }

    public void MoveToTile(Transform newTile)
    {
        gameSoundManager.PlayMoveSound();
        transform.SetParent(newTile);

        transform.localPosition = Vector3.zero;

        currentPosition = newTile.GetComponent<TileController>().tilePosition; // Обновляем позицию на основе нового родителя
    }
}