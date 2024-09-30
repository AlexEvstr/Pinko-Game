using UnityEngine;

public class SetupBoard2Player : MonoBehaviour
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

                TileController2Player tileController = tile.GetComponent<TileController2Player>();
                if (tileController == null)
                {
                    tileController = tile.AddComponent<TileController2Player>();
                }

                tileController.tilePosition = new Vector2Int(x, y);
            }
        }
    }
}
