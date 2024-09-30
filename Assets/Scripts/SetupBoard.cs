using UnityEngine;

public class SetupBoard : MonoBehaviour
{
    public GameObject boardParent;

    void Start()
    {
        AssignTilePositions();
    }

    void AssignTilePositions()
    {
        int columns = 8;
        int rows = 8;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int index = x * rows + y;

                GameObject tile = boardParent.transform.GetChild(index).gameObject;

                TileController tileController = tile.GetComponent<TileController>();
                if (tileController == null)
                {
                    tileController = tile.AddComponent<TileController>();
                }

                tileController.tilePosition = new Vector2Int(x, y);
            }
        }
    }
}